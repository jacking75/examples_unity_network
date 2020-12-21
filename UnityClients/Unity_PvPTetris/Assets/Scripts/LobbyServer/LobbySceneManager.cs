using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LobbyServer;
using GameNetwork;

public class LobbySceneManager : MonoBehaviour
{
    private InputField chatMsgInputField;
    private Text chattingLog;
    public static bool isMatchingResArrived { get; set; } = false;
    public static bool isMatchingNtfArrived { get; set; } = false;
    public static bool isWatingEnterRoomRes { get; set; } = false;
    
    private static LobbyMatchNtfPacket matchInfo;
    public static RoomEnterResPacket roomEnterRes { get; set; }

    // Start is called before the first frame update
    void Start()
    {
      //  Screen.SetResolution(1920, 1060, false);
        chatMsgInputField = GameObject.Find("ChatMsgInputField").GetComponent<InputField>();
        chattingLog = GameObject.Find("ChattingLog").GetComponent<Text>();
        matchInfo = new LobbyMatchNtfPacket();
        roomEnterRes = new RoomEnterResPacket();
        chattingLog.text = "";
    }


    // Update is called once per frame
    void Update()
    {
        ProcessLobbyPacket();
                
        //채팅메세지 확인.
        if (LobbyNetworkServer.Instance.ChatMsgQueue.Count > 0)
        {
            var chatMsg = LobbyNetworkServer.Instance.ChatMsgQueue.Dequeue();
            chattingLog.text += chatMsg + "\n";
        }

        if(isMatchingResArrived == true)
        {
            ProcessMatchingResponse();
            isMatchingResArrived = false;
        }

        if (isMatchingNtfArrived == true)
        {
            ProcessMatchingNotify();
            isMatchingNtfArrived = false;
        }

        if(GameNetworkServer.Instance.GetIsConnected())
        {
            GameNetworkServer.Instance.ProcessGameServerPacket();
        }

        if(GameNetworkServer.Instance.ClientStatus == GameNetworkServer.CLIENT_STATUS.LOGIN && isWatingEnterRoomRes==false)
        {
            GameNetworkServer.Instance.RequestRoomEnter(matchInfo.RoomNumber);
            isWatingEnterRoomRes = true;
        }
        else if(GameNetworkServer.Instance.ClientStatus == GameNetworkServer.CLIENT_STATUS.ROOM)
        {
            SceneManager.LoadScene("Game");
        }
        
    }


    public void OnClickMsgSendButton()
    {
        string message = "";
        if (chatMsgInputField != null)
        {
            message = chatMsgInputField.text;
        }
        if (message.Length <= 0)
        {
            return;
        }

        /*if (message.Length > PacketDataValue.MAX_CHAT_SIZE)
        {
            message = message.Substring(PacketDataValue.MAX_CHAT_SIZE - 1);
        }*/
        LobbyNetworkServer.Instance.LobbyChatRequest(message);
    }


    public void OnClickMatchingReqButton()
    {
        LobbyNetworkServer.Instance.MatchingRequest();
    }


    void ProcessLobbyPacket()
    {
        var packetList = LobbyNetworkServer.Instance.ReadPacket();

        if (packetList != null)
        {
            foreach (var packet in packetList)
            {
                if (packet.PacketID == ClientNetLib.PacketDef.SysPacketIDDisConnectdFromServer)
                {
                    //SetDisconnectd();
                    Debug.Log("서버와 접속 종료 !!!");
                }
                else
                {
                    LobbyServerPacketHandler.Process(packet);
                }
            }
        }
    }

    void ProcessMatchingResponse()
    {
        GameObject.Find("MatchingReqButtonText").GetComponent<Text>().text = "매칭중";
        GameObject.Find("MatchingReqButton").GetComponent<Button>().interactable = false;
    }

    void ProcessMatchingNotify()
    {
        GameNetworkServer.Instance.ConnectToServer(matchInfo.GameServerIP, matchInfo.GameServerPort);
        if(GameNetworkServer.Instance.GetIsConnected() == true)
        {
            GameNetworkServer.Instance.RequestLogin(LobbyNetworkServer.Instance.UserID, LobbyNetworkServer.Instance.UserID); //PW를 dummy데이터로 설정하였음
        }
    }


    public static bool FillMatchInfo(string ip_addr, ushort port, int room_idx)
    {
        matchInfo.GameServerIP = ip_addr;
        matchInfo.GameServerPort = port;
        matchInfo.RoomNumber = room_idx;

        Debug.Log("[FillMatchInfo] ip:"+ip_addr+"  port:"+port+"  roomNum:"+room_idx);
        return true;
    }

    public static int GetMatchedRooom()
    {
        return matchInfo.RoomNumber;
    }
}
