using System;
using GoPlay.Demo;
using GoPlay.Services;
using UnityEngine;
using UnityEngine.UI;

public class AirPlaneController : MonoBehaviour, IPoolable
{
    public float speed = 10f;
    public Text airName;
    
    private Vector2 curPos;

    private const int xMaxLimit = 1920;
    private const int yMaxLimit = 1080;

    private PlayerData playerData;
    public PlayerData PlayerData => playerData;

    public Action<PlayerData> CallBack;
    private bool canUpdate;

    public void Init(PlayerData _playerData, Action<PlayerData> _callBack, bool _canUpdate)
    {
        playerData = _playerData;
        CallBack += _callBack;
        curPos = new Vector2(_playerData.DVector2.XPos, _playerData.DVector2.YPos);
        Debug.Log($"初始位置: {curPos.x}, {curPos.y}");
        canUpdate = _canUpdate;
        airName.text = _playerData.Name;
    }

    public void SetPos(DVector2 dVector2)
    {
        Debug.Log($"当前位置: {dVector2.XPos}, {dVector2.YPos}");
        ((RectTransform)transform).anchoredPosition = new Vector2(dVector2.XPos, dVector2.YPos);
    }

    private void Update()
    {
        if (!canUpdate) return;
        CheckAirPlanePos();
    }

    private void CheckAirPlanePos()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if (x != 0)
        {
            curPos.x += x * speed;
            
            if (curPos.x >= xMaxLimit)
            {
                curPos.x = xMaxLimit;
            }
            else if (curPos.x <= 0f)
            {
                curPos.x = 0f;
            }
        }

        if (y != 0)
        {
            curPos.y += y * speed;
            
            if (curPos.y >= yMaxLimit)
            {
                curPos.y = yMaxLimit;
            } else if (curPos.y <= 0f)
            {
                curPos.y = 0f;
            }
        }

        if (x != 0 || y != 0)
        {
            SendData(curPos);
        }
    }

    private void SendData(Vector2 vector2)
    {
        CallBack.Invoke(new PlayerData()
        {
            HumanId = playerData.HumanId,
            Name = playerData.Name,
            DVector2 = new DVector2()
            {
                XPos = vector2.x,
                YPos = vector2.y
            }
        });
    }

    public void UnSet()
    {
        PoolService.Instance.Despawn(this);
    }

    public PoolData Pool { get; set; }
    public GameObject GameObject { get; }
    public Transform Transform { get; }
    public void OnSpawn(params object[] args)
    {
    }

    public void OnDespawn()
    {
    }

    public void Despawn()
    {
    }
}