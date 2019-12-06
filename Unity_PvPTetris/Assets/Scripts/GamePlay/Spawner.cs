using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameNetwork;

/* 
 * v0.0.2-r12
 * Written by Veritas83
 * www.NigelTodman.com
 * /Scripts/Spawner.cs
 */

public class Spawner : MonoBehaviour {

    public static bool isGameStart { get; set; } = false;
    public static bool isGameRunning { get; set; } = false;
    public static bool isGameEndPacketArrived { get; set; } = false;

    public GameObject[] GameOverPanel;
    // Use this for initialization
    void Start ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (isGameStart == true)
        {
            isGameRunning = true;
            isGameStart = false;
            spawnNext();
        }

        if(isGameEndPacketArrived == true)
        {
            GameOverFn();
            isGameRunning = false;
            isGameEndPacketArrived = false;
        }
	}

    // Groups (of Blocks that fall)
    public  Group[] groups;
    public ShadowGroup[] shadowgroups;


    public void spawnNext()
    {
        if (isGameRunning != true)
        {
            return;
        }
        // Random Index
         int i = UnityEngine.Random.Range(0, groups.Length);
        // Spawn Group at current Position
        Group spawned = Instantiate(groups[i], transform.position, Quaternion.identity);
    //    ShadowGrid.SpawnShadow(shadowgroups[i],spawned);
    }

    //GameOverPanel
    public void GameOverFn()
    {
        Debug.Log("GameOverFn() fired!");
        GameManager.Instance.isGameOver = true;

        GameObject gsui = GameObject.FindGameObjectWithTag("gsui");
        Instantiate(GameOverPanel[0], new Vector2(0,0),Quaternion.identity);
        GameObject gopactual = GameObject.Find("GameOverPanel(Clone)");
        gopactual.transform.parent = gsui.transform;
        gopactual.transform.SetPositionAndRotation(new Vector2(920, 500), Quaternion.identity);
    }
}
