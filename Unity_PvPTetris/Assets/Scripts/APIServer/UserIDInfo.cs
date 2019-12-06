using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserIDInfo : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(this);
    }
    public string UserID { get; set; }
    public string AuthToken { get; set; }
}
