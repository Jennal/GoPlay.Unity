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
        
        _client.AddListener(ProtocolConsts.Push_AirplanePush_join, (PlayerData data) =>
        {
            JoinAir(data);
        });
        
        _client.AddListener(ProtocolConsts.Push_AirplanePush_changepos, (PlayerData data) =>
        {
            if (curPlayerData == null) return;
            OnChangeAirPos(data);
        });
        
        _client.AddListener(ProtocolConsts.Push_AirplanePush_offline, (PlayerData data) =>
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

        var ok = await GetClient().AriPlane_Register(new RegisterAccount()
        {
            Name = inputField.text
        });
        curPlayerData = ok.Item2.CurPlayerData;
        InitAirPos(ok.Item2);
        airLoginUI.gameObject.SetActive(false);
        Debug.Log($"Register Success");
    }

    public void InitAirPos(GameData gameData)
    {
        airPlaneControllers =  new List<AirPlaneController>();
        foreach (var playerData in gameData.PlayerData)
        {
            var airPlane = PoolService.Instance.Spawn(airPlanePrefab);
            airPlane.GameObject.transform.parent = airPlaneRoot;
            airPlane.GameObject.transform.localScale = Vector3.one;
            var airController = airPlane.GameObject.GetComponent<AirPlaneController>();
            if (airController != null)
            {
                airController.Init(playerData, ChangeAirPos, playerData.HumanId == curPlayerData.HumanId);
                airController.SetPos(playerData.DVector2);
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
            airController.Init(data, ChangeAirPos, data.HumanId == curPlayerData.HumanId);
            airController.SetPos(data.DVector2);
            airPlaneControllers.Add(airController);
        }
    }
    
    public void OnChangeAirPos(PlayerData data)
    {
        var airPlaneController = airPlaneControllers.FirstOrDefault(o => o.PlayerData != null && o.PlayerData.HumanId == data.HumanId);
        if (airPlaneController != null)
        {
            airPlaneController.SetPos(data.DVector2);
        }
        else
        {
            var airPlane = PoolService.Instance.Spawn(airPlanePrefab);
            airPlane.GameObject.transform.parent = airPlaneRoot;
            airPlane.GameObject.transform.localScale = Vector3.one;
            var airController = airPlane.GameObject.GetComponent<AirPlaneController>();
            if (airController != null)
            {
                airController.Init(data, ChangeAirPos, data.HumanId == curPlayerData.HumanId);
                airController.SetPos(data.DVector2);
                airPlaneControllers.Add(airController);
            }
        }
    }

    public void RemoveAir(PlayerData data)
    {
        foreach (var airPlaneController in airPlaneControllers)
        {
            if (airPlaneController.PlayerData.HumanId == data.HumanId)
            {
                airPlaneControllers.Remove(airPlaneController);
                airPlaneController.UnSet();
                break;
            }
        }
    }
    
    public async void ChangeAirPos(PlayerData playerData)
    {
        if (playerData.HumanId != curPlayerData.HumanId) return;
        var ok = await GetClient().AriPlane_ChangeAirPos(playerData);
        Debug.Log($"ChangeAirPos Success: {playerData.DVector2.XPos}, {playerData.DVector2.YPos}");
    }

}
