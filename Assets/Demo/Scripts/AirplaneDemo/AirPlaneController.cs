using System;
using GoPlay.Demo;
using GoPlay.Services;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = GoPlay.Demo.Vector2;

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

    public void Init(PlayerData data, Action<PlayerData> callBack, bool isUpdate)
    {
        playerData = data;
        CallBack += callBack;
        curPos = data.Pos;
        Debug.Log($"初始位置: {curPos.X}, {curPos.Y}");
        canUpdate = isUpdate;
        airName.text = data.Name;
    }

    public void SetPos(Vector2 pos)
    {
        Debug.Log($"当前位置: {pos.X}, {pos.Y}");
        ((RectTransform)transform).anchoredPosition = new UnityEngine.Vector2(pos.X, pos.Y);
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
            curPos.X += x * speed;
            
            if (curPos.X >= xMaxLimit)
            {
                curPos.X = xMaxLimit;
            }
            else if (curPos.X <= 0f)
            {
                curPos.X = 0f;
            }
        }

        if (y != 0)
        {
            curPos.Y += y * speed;
            
            if (curPos.Y >= yMaxLimit)
            {
                curPos.Y = yMaxLimit;
            } else if (curPos.Y <= 0f)
            {
                curPos.Y = 0f;
            }
        }

        if (x != 0 || y != 0)
        {
            SendData(curPos);
        }
    }

    private void SendData(Vector2 vector2)
    {
        CallBack.Invoke(new PlayerData
        {
            Id = playerData.Id,
            Name = playerData.Name,
            Pos = new Vector2
            {
                X = vector2.X,
                Y = vector2.Y
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