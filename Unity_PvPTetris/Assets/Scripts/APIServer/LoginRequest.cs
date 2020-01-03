using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;
using System.IO;
using LitJson;
using UnityEngine.SceneManagement;
using LobbyServer;
using GameNetwork;
using ServerCommon;
using MessagePack;
using System;

public class LoginRequest : MonoBehaviour
{
    JsonData result_json;
    bool isLobbyRequestSended = false;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1920,1080,false);
    }

    // Update is called once per frame
    void Update()
    {
        if (LobbyNetworkServer.Instance.m_ClientState != CLIENT_LOBBY_STATE.LOBBY)
        {
            var packetList = LobbyNetworkServer.Instance.ReadPacket();
            foreach(var packet in packetList)
            {
                LobbyServerPacketHandler.Process(packet);
            }            
        }

        if (LobbyNetworkServer.Instance.m_ClientState == CLIENT_LOBBY_STATE.LOGIN && isLobbyRequestSended==false) 
        {
            LobbyNetworkServer.Instance.LobbyEnterRequest(0);
            isLobbyRequestSended = true;
        }


        if (LobbyNetworkServer.Instance.m_ClientState == CLIENT_LOBBY_STATE.LOBBY && isLobbyRequestSended == true)
        {
            SceneManager.LoadScene("Lobby");
        }

    }

    class LoginResultData
    {
        public short Result;
        public string AuthToken;
    }


    // 아래는 Redis를 사용하는 API서버를 통해 로그인을 하는 함수
    // TODO 현재 클라이언트에서는 테스트를 위해 이 함수가 아니라 LoginBtnClicket

    public void SendLoginRequest()
    {
        var input_address = (GameObject.Find("input_apiServer_field")).GetComponent<InputField>().text;
        var input_id = (GameObject.Find("input_id_field")).GetComponent<InputField>().text;
        var input_pw = (GameObject.Find("input_pw_field")).GetComponent<InputField>().text;

        string data = "{\"UserID\":\""+input_id+"\", \"UserPW\":\""+input_pw+"\"}";

        StartCoroutine(Post($"http://{input_address}/api/Login", data)); 
    }


    IEnumerator Post(string url, string bodyJsonString)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        Debug.Log("text: "+request.downloadHandler.text);
        result_json = JsonMapper.ToObject(request.downloadHandler.text);

        UserIDInfo user_id_info = GameObject.Find("UserIdentification").GetComponent<UserIDInfo>();
        string result_code = result_json["result"].ToString();

        if (result_code=="0" && result_json["authToken"] != null ) //0을 Success Code로 설정하였음
        {
            string auth_token = result_json["authToken"].ToString();
            user_id_info.UserID = (GameObject.Find("input_id_field")).GetComponent<InputField>().text;
            user_id_info.AuthToken= auth_token;
            user_id_info.LobbyServerIP = result_json["lobbyServerIP"].ToString();
            user_id_info.LobbyServerPort = Convert.ToUInt16(result_json["lobbyServerPort"].ToString());

            LobbyNetworkServer.Instance.LoginRequest(user_id_info);
        }
    }


}
