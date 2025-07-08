using System.Collections.Generic;
using System.Linq;
using GoPlay;
using GoPlay.Core.Transport.NetCoreServer;
using GoPlay.Demo;
using GoPlay.Network;
using GoPlay.Services;
using GoPlayProj.Extension.Frontend;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Asyncoroutine;
using GoPlay.Core.Protocols;

public class AirPlaneClient : MonoBehaviour
{
    public Transform airPlaneRoot;
    public Button btnConnect;
    public Button btnLogin;
    public GameObject airPlanePrefab;
    public InputField inputField;
    public GameObject airLoginUI;
    
    private PlayerData curPlayerData;
    
    private Client<NcClient> _client;
    public Client<NcClient> Client => _client;
    
    private List<AirPlaneController> airPlaneControllers = new List<AirPlaneController>();
    
    private Client<NcClient> GetClient()
    {
        if (_client != null) return _client;
        
        _client = new Client<NcClient>();
        _client.MainThreadActionRunner = UnityMainThreadActionRunner.Instance;
        _client.OnConnected += () =>
        {
            Debug.Log("Connected!");
        };
        
        _client.OnDisconnected += () =>
        {
            Debug.Log("Disconnected!");
        };
        
        _client.OnError += (error) =>
        {
            Debug.Log($"Error: {error}");
        };
        
        _client.AddListener(ProtocolConsts.Push_AirplaneJoin, (PlayerData data) =>
        {
            JoinAir(data);
        });
        
        _client.AddListener(ProtocolConsts.Push_AirplanePos, (PlayerData data) =>
        {
            if (curPlayerData == null) return;
            OnChangeAirPos(data);
        });
        
        _client.AddListener(ProtocolConsts.Push_AirplaneOffline, (PlayerData data) =>
        {
            RemoveAir(data);
        });
        
        return _client;
    }

    public async Task Connect()
    {
        var ok = await GetClient().Connect("localhost", 8888);
        Debug.Log($"Connected: {ok}");
    }
    
    public async void Disconnect()
    {
        if (_client == null) return;
        
        await _client.DisconnectAsync();
        _client.Dispose();
        _client = null;
    }

    public async void OnConnect()
    {
        await Connect();
        btnConnect.gameObject.SetActive(false);
        btnLogin.gameObject.SetActive(true);
        inputField.gameObject.SetActive(true);
    }

    public async void Register()
    {
        if (inputField.text == "") return;

        var ok = await GetClient().AirPlane_Register(new RegisterAccount()
        {
            Name = inputField.text
        });
        curPlayerData = ok.Item2.CurPlayer;
        InitAirPos(ok.Item2);
        airLoginUI.gameObject.SetActive(false);
        Debug.Log($"Register Success");
    }

    public void InitAirPos(GameData gameData)
    {
        airPlaneControllers =  new List<AirPlaneController>();
        foreach (var playerData in gameData.PlayerList)
        {
            var airPlane = PoolService.Instance.Spawn(airPlanePrefab);
            airPlane.GameObject.transform.parent = airPlaneRoot;
            airPlane.GameObject.transform.localScale = Vector3.one;
            var airController = airPlane.GameObject.GetComponent<AirPlaneController>();
            if (airController != null)
            {
                airController.Init(playerData, ChangeAirPos, playerData.Id == curPlayerData.Id);
                airController.SetPos(playerData.Pos);
                airPlaneControllers.Add(airController);
            }
        }
    }

    public void JoinAir(PlayerData data)
    {
        var airPlane = PoolService.Instance.Spawn(airPlanePrefab);
        airPlane.GameObject.transform.parent = airPlaneRoot;
        airPlane.GameObject.transform.localScale = Vector3.one;
        var airController = airPlane.GameObject.GetComponent<AirPlaneController>();
        if (airController != null)
        {
            airController.Init(data, ChangeAirPos, data.Id == curPlayerData.Id);
            airController.SetPos(data.Pos);
            airPlaneControllers.Add(airController);
        }
    }
    
    public void OnChangeAirPos(PlayerData data)
    {
        var airPlaneController = airPlaneControllers.FirstOrDefault(o => o.PlayerData != null && o.PlayerData.Id == data.Id);
        if (airPlaneController != null)
        {
            airPlaneController.SetPos(data.Pos);
        }
        else
        {
            var airPlane = PoolService.Instance.Spawn(airPlanePrefab);
            airPlane.GameObject.transform.parent = airPlaneRoot;
            airPlane.GameObject.transform.localScale = Vector3.one;
            var airController = airPlane.GameObject.GetComponent<AirPlaneController>();
            if (airController != null)
            {
                airController.Init(data, ChangeAirPos, data.Id == curPlayerData.Id);
                airController.SetPos(data.Pos);
                airPlaneControllers.Add(airController);
            }
        }
    }

    public void RemoveAir(PlayerData data)
    {
        foreach (var airPlaneController in airPlaneControllers)
        {
            if (airPlaneController.PlayerData.Id == data.Id)
            {
                airPlaneControllers.Remove(airPlaneController);
                airPlaneController.UnSet();
                break;
            }
        }
    }
    
    public async void ChangeAirPos(PlayerData playerData)
    {
        if (playerData.Id != curPlayerData.Id) return;
        var ok = await GetClient().AirPlane_UpdatePos(playerData);
        Debug.Log($"ChangeAirPos Success: {playerData.Pos.X}, {playerData.Pos.Y}");
    }

}
