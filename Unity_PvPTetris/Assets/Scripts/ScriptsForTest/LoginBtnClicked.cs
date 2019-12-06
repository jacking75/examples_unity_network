using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameNetwork;
public class LoginBtnClicked : MonoBehaviour
{
    private InputField id_input_field;
    private InputField pw_input_field;

    public bool login_success { get; set; } = false;
    private bool IsConnected = false;
    // Start is called before the first frame update
    void Start()
    {
        id_input_field = (GameObject.Find("input_id_field")).GetComponent<InputField>();
        pw_input_field = (GameObject.Find("input_pw_field")).GetComponent<InputField>();
    }

    public void OnClicked()
    {
        Debug.Log("id=" + id_input_field.text + " pw" + pw_input_field.text);
    //    GameNetworkServer.Instance.ConnectToServer();
        if (GameNetworkServer.Instance.GetIsConnected() == true)
        {
            IsConnected = true;
            GameNetworkServer.Instance.RequestLogin(id_input_field.text, pw_input_field.text);

        }

        //
    }

    void Update() {
        if (IsConnected && GameNetworkServer.Instance.ClientStatus == GameNetworkServer.CLIENT_STATUS.LOGIN)
        {
             SceneManager.LoadScene("Lobby");
            //SceneManager.LoadScene("Game");
        }
        


    }
}
