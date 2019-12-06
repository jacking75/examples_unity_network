using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameNetwork;

//일일히 로그인하기는 번거로우므로 게임화면에서 테스트할때 자동적으로 서버에 로그인 시켜주는 시스템

public class TestConnect : MonoBehaviour
{
    GameNetworkServer.CLIENT_STATUS clientStatus;
    bool IsConnected = false;
    string[] TestNameArr = { "Iron", "Bronze", "Silver","Gold", "Platinum", "Diamond","Master", "GrandMaster","Challenger"};
    float WaitTime;
    int ReqRoomIdx = -1;
    // Start is called before the first frame update
    void Start() {
        clientStatus = GameNetworkServer.Instance.ClientStatus;

        if (clientStatus == GameNetworkServer.CLIENT_STATUS.NONE)
        {
            GameNetworkServer.Instance.ConnectToServer("10.14.0.81",11020);
            if (GameNetworkServer.Instance.GetIsConnected() == true)
            {
                IsConnected = true;
                GameNetworkServer.Instance.RequestLogin(TestNameArr[Random.Range(0,9)]+ Random.Range(0, 9999), "1234");
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        clientStatus = GameNetworkServer.Instance.ClientStatus;
        if (IsConnected && clientStatus == GameNetworkServer.CLIENT_STATUS.LOGIN)
        {
            if (Time.time - WaitTime > 0.3f)
            {
              //  GameNetworkServer.Instance.RequestRoomEnter(++ReqRoomIdx + "");
                WaitTime = Time.time;
            }
        }

        if (IsConnected && clientStatus == GameNetworkServer.CLIENT_STATUS.ROOM)
        {
           // Spawner.isGameStart = true;
            enabled = false;
        }
    }
}
