using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

/* 
 * v0.0.2-r12
 * Written by Veritas83
 * www.NigelTodman.com
 * /Scripts/ScoreScript.cs
 */

public class ScoreScript : MonoBehaviour
{
    private string ScoreDisplay = "";
    private string HighScoreDisplay = "";
    //private string PlayerDisplay = "";
    public Text ScoreLabel;
    public Text HighScoreLabel;
    public Text PlayerLabel;
    bool HighScoreRefreshed = false;
    const string privCode = "";
    const string pubCode = "";
    const string baseURL = "http://dreamlo.com/lb/";

    // Use this for initialization
    void Start()
    {
         /*   GameManager.Instance.NewGame();
            ScoreLabel.text = "Score: " + GameManager.Instance.ScoreValue.ToString() + "\n Lines: " + GameManager.Instance.LineValue.ToString() + "\n Level: " + GameManager.Instance.GameLevel.ToString();
            PlayerLabel.text = GameManager.Instance.SetPlayerName; */
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdateScore()
    {
        GameManager.Instance.LineValue = GameManager.Instance.LineValue + 1;
        GameManager.Instance.GameLevel = (GameManager.Instance.LineValue / 2);
      //  GameManager.Instance.GameLevel = 7;
        GameObject gmgr = GameObject.FindGameObjectWithTag("GameManager");
        if (GameManager.Instance.GameLevel == 0)
        {
            if (Grid.cons == 4)
            {
                gmgr.GetComponent<GameManager>().ScreenFlash();
                GameManager.Instance.ScoreValue = GameManager.Instance.ScoreValue + 96;
            }
            else if (Grid.cons == 3)
            {
                //gmgr.GetComponent<GameManager>().ScreenFlash();
                GameManager.Instance.ScoreValue = GameManager.Instance.ScoreValue + 36;
            }
            else if (Grid.cons == 2)
            {
                //gmgr.GetComponent<GameManager>().ScreenFlash();
                GameManager.Instance.ScoreValue = GameManager.Instance.ScoreValue + 24;
            }
            else if (Grid.cons <= 1)
            {
                gmgr.GetComponent<GameManager>().ScreenFlash();
                GameManager.Instance.ScoreValue = GameManager.Instance.ScoreValue + 12;
            }
        } else if (GameManager.Instance.GameLevel >= 1 && GameManager.Instance.GameLevel <= 6)
        {
            if (Grid.cons == 4)
            {
                gmgr.GetComponent<GameManager>().ScreenFlash();
                GameManager.Instance.ScoreValue = GameManager.Instance.ScoreValue + (96 * GameManager.Instance.GameLevel);
            }
            else if (Grid.cons == 3)
            {
                GameManager.Instance.ScoreValue = GameManager.Instance.ScoreValue + (36 * GameManager.Instance.GameLevel);
            }
            else if (Grid.cons == 2)
            {
                GameManager.Instance.ScoreValue = GameManager.Instance.ScoreValue + (24 * GameManager.Instance.GameLevel);
            }
            else if (Grid.cons <= 1)
            {
                GameManager.Instance.ScoreValue = GameManager.Instance.ScoreValue + (12 * GameManager.Instance.GameLevel);
            }
        } else if (GameManager.Instance.GameLevel >= 7)
        {
            if (Grid.cons == 4)
            {
                gmgr.GetComponent<GameManager>().ScreenFlash();
                GameManager.Instance.ScoreValue = GameManager.Instance.ScoreValue + 576;
            }
            else if (Grid.cons == 3)
            {
                GameManager.Instance.ScoreValue = GameManager.Instance.ScoreValue + 216;
            }
            else if (Grid.cons == 2)
            {
                GameManager.Instance.ScoreValue = GameManager.Instance.ScoreValue + 144;
            }
            else if (Grid.cons <= 1)
            {
                GameManager.Instance.ScoreValue = GameManager.Instance.ScoreValue + 72;
            }
            
        }
        //FallSpeed set here
        if (GameManager.Instance.GameLevel > 1 && GameManager.Instance.GameLevel <= 6)
        {
            GameManager.Instance.FallSpeed = (10f - GameManager.Instance.GameLevel) * 0.1f;
        }
        else if (GameManager.Instance.GameLevel >= 7 && GameManager.Instance.GameLevel <= 9)
        {
            GameManager.Instance.FallSpeed = (10f - GameManager.Instance.GameLevel) * 0.1f + 0.05f;
        }
        else if (GameManager.Instance.GameLevel == 10)
        {
            GameManager.Instance.FallSpeed = 0.15f;
        }
        else if (GameManager.Instance.GameLevel == 11)
        {
            GameManager.Instance.FallSpeed = 0.10f;
        }
        ScoreDisplay = "Score: " + GameManager.Instance.ScoreValue.ToString() + "\n Lines: " + GameManager.Instance.LineValue.ToString() + "\n Level: " + GameManager.Instance.GameLevel.ToString();
        ScoreLabel.text = ScoreDisplay;

    }

}