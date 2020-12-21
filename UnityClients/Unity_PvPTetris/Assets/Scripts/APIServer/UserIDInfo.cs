using System.Collections;
using System;
using UnityEngine;

public class UserIDInfo : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(this);
    }
    public string UserID { get; set; }
    public string AuthToken { get; set; }
    public string LobbyServerIP { get; set; }
    public UInt16 LobbyServerPort { get; set; }
}
