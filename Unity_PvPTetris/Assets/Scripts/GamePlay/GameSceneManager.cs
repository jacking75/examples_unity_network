using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameNetwork;

public class GameSceneManager : MonoBehaviour
{
    private InputField chatMsgInputField;
    private Text chattingLog;
    GameObject ErrorMsgPopUp;


    bool isGameStart = false;
    // Start is called before the first frame update
    void Start()
    {
     //   Screen.SetResolution(1366, 720, false);
        chatMsgInputField = GameObject.Find("ChatMsgInputField").GetComponent<InputField>();
        chattingLog = GameObject.Find("ChattingLog").GetComponent<Text>();
        chattingLog.text = "";

        ErrorMsgPopUp = GameObject.Find("ErrorMsgPanel");
        ErrorMsgPopUp.SetActive(false);
    }



    // Update is called once per frame
    void Update()
    {
        var packet = GameNetworkServer.Instance.ReadPacket();
        if (packet.PacketID != 0)
        {
            GameServerPacketHandler.Process(packet);
        }
        else if (packet.PacketID == NetLib.PacketDef.SysPacketIDDisConnectdFromServer)
        {
            //SetDisconnectd();
            Debug.Log("서버와 접속 종료 !!!");
        }


        //채팅메세지 확인.
        if (GameNetworkServer.Instance.ChatMsgQueue.Count > 0)
        {
            RoomChatNotPacket recvMsg = GameNetworkServer.Instance.ChatMsgQueue.Dequeue();
            chattingLog.text += "[" + recvMsg.UserID + "] " + recvMsg.Message + "\n";
        }

        if(isGameStart == false && GameNetworkServer.Instance.ClientStatus == GameNetworkServer.CLIENT_STATUS.GAME )
        {
            GameObject.Find("GameStartButton").GetComponent<Button>().interactable = false;
            isGameStart = true;
        }        
    }


    public void OnClickMsgSendButton()
    {
        string message="";
        if (chatMsgInputField != null) {
            message = chatMsgInputField.text;
        }
        if(message.Length <=0) {
            return;
        }

        if(message.Length > PacketDataValue.MAX_CHAT_SIZE)
        {
            message = message.Substring(PacketDataValue.MAX_CHAT_SIZE - 1);
        }
        GameNetworkServer.Instance.RequestChatMsg(message);
    }


    public void OnClickGameStartBtn()
    {

        if(GameNetworkServer.Instance.ClientStatus == GameNetworkServer.CLIENT_STATUS.ROOM)
        {
            GameNetworkServer.Instance.SendGameStartPacket(new GameStartRequestPacket());
        }
        else
        {
            PopUpErrorMessage("게임시작 요청을 보낼 수 있는 상태가 아닙니다.");
        }
        
    }


    public void PopUpErrorMessage(string message)
    {
        ErrorMsgPopUp.SetActive(true);
        Text ErrorMessage = GameObject.Find("ErrorMsgText").GetComponent<Text>();
        ErrorMessage.text = message;

    }


}
