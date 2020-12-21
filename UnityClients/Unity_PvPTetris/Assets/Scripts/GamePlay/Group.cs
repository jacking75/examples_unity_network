using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameNetwork;

/* 
 * v0.0.1-r11
 * Written by Veritas83
 * www.NigelTodman.com
 * /Scripts/Group.cs
 */

public class Group : MonoBehaviour
{
    // Time since last gravity tick
    public Single lastFall = 0;
    public Single lastSecond = 0;
    public static Single lastSendPacket = 0;
    private Single SyncPacketInterval;
    private static Single TimeCapture = 0;
    public static int RecordIdx { get; set; } = 0;
    public Int16 blockType { get; set; } = -1;
    private static GameSyncReqPacket synchronizePacket { get; set; }

    public ShadowGroup MyShadow; 
    void Start()
    {
        SyncPacketInterval = GameNetworkServer.Instance.SyncPacketInterval;
        if (synchronizePacket == null)
        {
            synchronizePacket = new GameSyncReqPacket();
        }
           
        // Default position not valid? Then it's game over
       if (!isValidGridPos())
        {
            GameObject gop = GameObject.FindGameObjectWithTag("spawner");
            GameNetworkServer.Instance.SendGameEndPacket();
            gop.GetComponent<Spawner>().GameOverFn();
            Destroy(gameObject);
        }


       if(gameObject.name.Length >= 5)
        {
            switch (gameObject.name[5])
            {
                case 'I':
                    {
                        EnqueueEventToSyncPacket(TimeCapture, (Int16)EVENT_TYPE.SPAWN_GROUP_I);
                        break;
                    }

                case 'J':
                    {
                        EnqueueEventToSyncPacket(TimeCapture, (Int16)EVENT_TYPE.SPAWN_GROUP_J);
                        break;
                    }

                case 'L':
                    {
                        EnqueueEventToSyncPacket(TimeCapture, (Int16)EVENT_TYPE.SPAWN_GROUP_L);
                        break;
                    } 

                case 'O':
                    {
                        EnqueueEventToSyncPacket(TimeCapture, (Int16)EVENT_TYPE.SPAWN_GROUP_O);
                        break;
                    }

                case 'S':
                    {
                        EnqueueEventToSyncPacket(TimeCapture, (Int16)EVENT_TYPE.SPAWN_GROUP_S);
                        break;
                    }

                case 'T':
                    {
                        EnqueueEventToSyncPacket(TimeCapture, (Int16)EVENT_TYPE.SPAWN_GROUP_T);
                        break;
                    }

                case 'Z':
                    {
                        EnqueueEventToSyncPacket(TimeCapture, (Int16)EVENT_TYPE.SPAWN_GROUP_Z);
                        break;
                    }
            }

        }

    }


    public void EnqueueEventToSyncPacket(Single TimeCapture, Int16 EventType)
    {
        synchronizePacket.EventRecordArr6[RecordIdx++] = EventType;
    }



    // Update is called once per frame
    void Update()
    {

        if (Time.time - lastSendPacket >= SyncPacketInterval) //설정된 동기화패킷 설정 간격마다 데이터를 보낸다.
        {
            lock (synchronizePacket)
            {
                synchronizePacket.Score = GameManager.Instance.ScoreValue;
                synchronizePacket.Line = GameManager.Instance.LineValue;
                synchronizePacket.Level = GameManager.Instance.GameLevel;
                GameNetworkServer.Instance.SendSynchronizePacket(synchronizePacket);
                lastSendPacket = Time.time;
                RecordIdx = 0;
                for (int i=0; i<6; i++)
                {
                    synchronizePacket.EventRecordArr6[i] = -1;
                }
            }

        }

            //FallSpeed called
        if (Time.time - lastFall >= GameManager.Instance.FallSpeed) {
            Vector3 vector = new Vector3(0, -1, 0);          
            // Modify position
            transform.position += vector;
            // See if valid
            if (isValidGridPos()) {
                // It's valid. Update grid.
                updateGrid();
                EnqueueEventToSyncPacket(Time.time, (Int16)EVENT_TYPE.MOVE_DOWN);
            }
            else {
                // It's not valid. revert.
                transform.position += new Vector3(0, 1, 0);
                // Clear filled horizontal lines
                Grid.deleteFullRows();
                EnqueueEventToSyncPacket(Time.time, (Int16)EVENT_TYPE.DELETE_ROW);
                // Spawn next Group
                Debug.Log("Spawned (FallTime)");
                FindObjectOfType<Spawner>().spawnNext();

                // Disable script
                enabled = false;
            //    MyShadow.SetEnable(false);
            }
            lastFall = Time.time;
        }


        // Move Left
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Vector3 vector = new Vector3(-1, 0, 0);
            transform.position += vector;

            if (isValidGridPos())  {
                updateGrid();
                EnqueueEventToSyncPacket(Time.time, (Int16)EVENT_TYPE.MOVE_LEFT);
            }
            else {
                transform.position += new Vector3(1, 0, 0);
            }
                
        }

        // Move Right
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Vector3 vector = new Vector3(1, 0, 0);
            transform.position += vector;

            if (isValidGridPos())  {
                updateGrid();
                EnqueueEventToSyncPacket(Time.time, (Int16)EVENT_TYPE.MOVE_RIGHT);
            }
            else {
                transform.position += new Vector3(-1, 0, 0);
            }
               
        }

        // Rotate
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.Rotate(0, 0, -90);

            if (isValidGridPos()) {
                updateGrid();
                //MyShadow.UpdateShadowRotation();
                EnqueueEventToSyncPacket(Time.time, (Int16)EVENT_TYPE.ROTATE);
            }
            else
                transform.Rotate(0, 0, 90);
        }

        // Move Downwards and Fall
        else if (Input.GetKeyDown(KeyCode.DownArrow) ||
                 Time.time - lastFall >= 1)  {
            Vector3 vector = new Vector3(0, -1, 0);
            transform.position += vector;

            if (isValidGridPos()) {
                updateGrid();
                EnqueueEventToSyncPacket(Time.time, (Int16)EVENT_TYPE.MOVE_DOWN);
            }
            else  {
                transform.position += new Vector3(0, 1, 0);

                // Clear filled horizontal lines
                Grid.deleteFullRows();
                EnqueueEventToSyncPacket(Time.time, (Int16)EVENT_TYPE.DELETE_ROW);
                Debug.Log("Spawned (KeyDown)");
                FindObjectOfType<Spawner>().spawnNext();

                enabled = false;
              //  MyShadow.SetEnable(false);
            }
            lastFall = Time.time;
        }

    }

    bool isValidGridPos()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = Grid.roundVec2(child.position);

            // Not inside Border?
            if (!Grid.insideBorder(v))
                return false;

            // Block in grid cell (and not part of same group)?
            if (Grid.grid[(int)v.x, (int)v.y] != null &&
                Grid.grid[(int)v.x, (int)v.y].parent != transform)
                return false;
        }
        GameManager.Instance.isGameOver = false;
        return true;
    }

    void updateGrid()
    {
        // Remove old children from grid
        for (int y = 0; y < Grid.h; ++y) {
            for (int x = 0; x < Grid.w; ++x) {
                if (Grid.grid[x, y] != null)  {
                    if (Grid.grid[x, y].parent == transform) {
                        Grid.grid[x, y] = null;
                    }
                }
            }
        }
        // Add new children to grid
        foreach (Transform child in transform)
        {
            Vector2 v = Grid.roundVec2(child.position);
            Grid.grid[(int)v.x, (int)v.y] = child;
        }
    }
    //End
}
