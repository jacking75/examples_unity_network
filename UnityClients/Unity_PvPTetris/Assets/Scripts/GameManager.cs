using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.SceneManagement;

using System.Threading;
using System.Linq;
using GameNetwork;
using System.Text;

/* 
 * v0.0.2-r12
 * Written by Veritas83
 * www.NigelTodman.com
 * /Scripts/GameManager.cs
 */
public class GameManager : MonoBehaviour {
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }
    private static GameManager instance = null;
    public Int32 ScoreValue = 0;
    public Int32 LineValue = 0;
    public bool InputAllowed = true;
    public Single FallSpeed = 1f;
    public Int32 GameLevel = 0;
    public string SetPlayerName = "Player";
    public InputField myInputField;
    public bool isGameOver = false;


    public Int32 RivalScoreValue = 0;
    public Int32 RivalLineValue = 0;
    public Int32 RivalGameLevel = 0;

    void Awake()
    {
        if(instance)
        {
            DestroyImmediate(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Use this for initialization
    void Start () {
        GameObject sm = GameObject.FindGameObjectWithTag("SettingsMenu");
        if(sm!=null) DontDestroyOnLoad(sm);
        LoadSettings();
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    public void CallLobbyScene()
    {
       // this.Queue
       
    }


    public void NewGame()
    {    
        GameManager.Instance.LineValue = 0;
        GameManager.Instance.ScoreValue = 0;
        GameManager.Instance.ScoreValue = 0;
        GameObject gsui = GameObject.FindGameObjectWithTag("gsui");
        GameObject PlayerLabel = GameObject.FindGameObjectWithTag("PlayerLabel");
        GameObject pl = GameObject.FindGameObjectWithTag("PauseLabel");
        pl.GetComponent<Text>().enabled = false;
        
        PlayerLabel.GetComponent<Text>().text = GameManager.Instance.SetPlayerName;
        LoadSettings();
        //Debug Stuff
    }


    public void LoadPlayer()
    {
        string PlayerName = GameNetworkServer.Instance.UserID;
        GameObject[] LocalNameLabels = GameObject.FindGameObjectsWithTag("LocalPlayerName");

        foreach (GameObject TextLabelObject in  LocalNameLabels)
        {
            Text PlayerText = TextLabelObject.GetComponent<Text>();
            PlayerText.text = PlayerName;
        }
        
        Debug.Log("Player Name set to: " + PlayerName);

        string RivalName = GameNetworkServer.Instance.RivalID;
        GameObject[] RemoteNameLabels = GameObject.FindGameObjectsWithTag("RemotePlayerName");
        foreach (GameObject TextLabelObject in RemoteNameLabels)
        {
            Text PlayerText = TextLabelObject.GetComponent<Text>();
            PlayerText.text = RivalName;
        }

    }
   
    public void LoadSettings()
    {
        LoadPlayer();
    }
    public void ScreenFlash()
    {
        Debug.Log("ScreenFlash() fired!");
        StartCoroutine("LerpColor");
        GameObject go = GameObject.FindGameObjectWithTag("MainCamera");
        go.GetComponent<Camera>().backgroundColor = Color.white;
        go.GetComponent<Camera>().backgroundColor = Color.Lerp(Color.black,Color.white,2.5f);
        go.GetComponent<Camera>().backgroundColor = Color.black;
    }
    IEnumerator LerpColor()
    {
        for (int c = 0; c <= 4; c++) { 
        float t = 0f;
        float duration = 0.06f;
        float smoothness = 0.01f;
        float increment = smoothness / duration;
        Debug.Log("LerpColor() fired!");
        GameObject go = GameObject.FindGameObjectWithTag("MainCamera");
        go.GetComponent<Camera>().backgroundColor = Color.white;
        while (t <= 1)
        {
            go.GetComponent<Camera>().backgroundColor = Color.Lerp(Color.gray, Color.white, t);
            t += increment;
            Debug.Log("T Value: " + t.ToString());
            yield return new WaitForSeconds(smoothness);
        }
        go.GetComponent<Camera>().backgroundColor = Color.black;
        yield return true;
        }
    }
}
