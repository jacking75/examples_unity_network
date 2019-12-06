using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameNetwork;
public class LoadUserInfo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string userID = GameNetworkServer.Instance.UserID;
        var idText = (GameObject.Find("id_txt")).GetComponent<Text>();
        idText.text = userID;
    }

}
