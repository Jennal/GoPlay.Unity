using GoPlay;
using GoPlay.Core.Protocols;
using GoPlay.Core.Transport.NetCoreServer;
using GoPlay.Demo;
using GoPlayProj.Extension.Frontend;
using UnityEngine;
using UnityEngine.UI;

public class GoPlayClient : MonoBehaviour
{
    public string name = "Me";
    public InputField inputField;
    public Text chatText;
    
    private Client<NcClient> _client;
    public Client<NcClient> Client => _client;

    private Client<NcClient> GetClient()
    {
        if (_client != null) return _client;
        
        _client = new Client<NcClient>();
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
        
        _client.AddListener(ProtocolConsts.Push_TestPush, (PbString data) =>
        {
            Debug.Log($"Recv Push: {data.Value}");
        });
        
        _client.AddListener(ProtocolConsts.Push_ChatPush, (ChatData data) =>
        {
            Debug.Log($"Recv Chat Push: {data}");
            chatText.text += $"[{data.Name}]: {data.Content}\n";
        });
        
        return _client;
    }

    public async void Connect()
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
    
    public async void Echo()
    {
        var (status, resp) = await GetClient().Test_Echo(new PbString
        {
            Value = "Test Echo"
        });
        Debug.Log($"Response: {status}, {resp.Value}");
    }
    
    public async void Inc()
    {
        var (status, resp) = await GetClient().Test_Inc(new PbLong
        {
            Value = 1
        });
        Debug.Log($"Response: {status}, {resp.Value}");
    }
    
    public async void Error()
    {
        var (status, resp) = await GetClient().Test_Error(new PbString
        {
            Value = "Test Error"
        });
        Debug.Log($"Response: {status}, {resp}");
    }
    
    public async void Notify()
    {
        GetClient().Test_Notify(new PbString
        {
            Value = "Test Notify"
        });
    }

    public async void Send()
    {
        var status = await GetClient().Chat_Send(new ChatData
        {
            Name = "Test",
            Content = inputField.text
        });
        Debug.Log($"Response: {status}");
        if (status.Code == StatusCode.Success)
        {
            inputField.text = "";
        }
    }
}
