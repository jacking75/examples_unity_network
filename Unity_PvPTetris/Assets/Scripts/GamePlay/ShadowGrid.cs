using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameNetwork;
using System;
public class ShadowGrid : MonoBehaviour
{
    public static GameObject ShadowGridStart;
    public static Queue<GameSynchronizeNotifyPacket> RecvSyncPacketQueue;

    public ShadowGroup[] shadowgroups;
    GameSynchronizeNotifyPacket RecvSyncPacket;
    public static Single GetPacketTime { get; set; } = 0;
    Single PacketSimulTimeFrame = 0;

    //원격 클라이언트에서 블록이 Spawn되면 할당됨
    public static ShadowGroup RemoteShadow;
    bool IsSimulating = false;
    int EventProcessIdx = -1;
    // Start is called before the first frame update
    void Start()
    {
        RecvSyncPacketQueue = new Queue<GameSynchronizeNotifyPacket>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(IsSimulating == true) {
            if (EventProcessIdx >= 6)
            {
                IsSimulating = false;
                RecvSyncPacket = null;
                return;
            }

            PacketSimulTimeFrame = Time.time - GetPacketTime;
            //  Debug.Log("PacketSimulTimeFrame"+PacketSimulTimeFrame);
            //  EVENT_TYPE recordEventType = (EVENT_TYPE)RecvSyncPacket.EventRecordArr[EventProcessIdx].EventType;
            EVENT_TYPE recordEventType = (EVENT_TYPE)RecvSyncPacket.EventRecordArr[EventProcessIdx];
            while (recordEventType == EVENT_TYPE.NONE && EventProcessIdx < 6)
            {
                EventProcessIdx++;
                if (EventProcessIdx >= 6)
                {
                    break;
                }
                recordEventType = (EVENT_TYPE)RecvSyncPacket.EventRecordArr[EventProcessIdx];
            }

            if (recordEventType != EVENT_TYPE.NONE && EventProcessIdx < 6)
            {
                ProcessEvent(recordEventType);
                EventProcessIdx++;
            }

          //  GameManager.Instance.ScoreValue = 

        }
        else
        {
            if (RecvSyncPacketQueue != null && RecvSyncPacketQueue.Count > 0)
            {
                RecvSyncPacket = RecvSyncPacketQueue.Dequeue();
                Text RivalScore = GameObject.FindGameObjectWithTag("RivalScore").GetComponent<Text>();
                RivalScore.text = "Score: "+ RecvSyncPacket.Score+"\nLine: "+ RecvSyncPacket.Line+"\nLevel:"+ RecvSyncPacket.Level;
                GetPacketTime = Time.time;
                EventProcessIdx = 0;
                IsSimulating = true;
            }
        }




    }


    void ProcessEvent(EVENT_TYPE eventType)
    {
      //  Debug.Log("ShadowGrid Process Event : " + eventType);

        switch (eventType)
        {
            case EVENT_TYPE.SPAWN_GROUP_I:
                {
                    SpawnShadow(shadowgroups[(int)EVENT_TYPE.SPAWN_GROUP_I]);
                    break;
                }

            case EVENT_TYPE.SPAWN_GROUP_J:
                {
                    SpawnShadow(shadowgroups[(int)EVENT_TYPE.SPAWN_GROUP_J]);
                    break;
                }

            case EVENT_TYPE.SPAWN_GROUP_L:
                {
                    SpawnShadow(shadowgroups[(int)EVENT_TYPE.SPAWN_GROUP_L]);
                    break;
                }

            case EVENT_TYPE.SPAWN_GROUP_O:
                {
                    SpawnShadow(shadowgroups[(int)EVENT_TYPE.SPAWN_GROUP_O]);
                    break;
                }

            case EVENT_TYPE.SPAWN_GROUP_S:
                {
                    SpawnShadow(shadowgroups[(int)EVENT_TYPE.SPAWN_GROUP_S]);
                    break;
                }

            case EVENT_TYPE.SPAWN_GROUP_T:
                {
                    SpawnShadow(shadowgroups[(int)EVENT_TYPE.SPAWN_GROUP_T]);
                    break;
                }

            case EVENT_TYPE.SPAWN_GROUP_Z:
                {
                    SpawnShadow(shadowgroups[(int)EVENT_TYPE.SPAWN_GROUP_Z]);
                    break;
                }

            case EVENT_TYPE.MOVE_DOWN:
                {
                    if (RemoteShadow != null)
                    {
                        RemoteShadow.UpdateShadowMovement(new Vector3(0, -1, 0));
                    }
                    break;
                }

            case EVENT_TYPE.MOVE_LEFT:
                {
                    if (RemoteShadow != null)
                    {
                        RemoteShadow.UpdateShadowMovement(new Vector3(-1, 0, 0));
                    }
                    break;
                }

            case EVENT_TYPE.MOVE_RIGHT:
                {
                    if (RemoteShadow != null)
                    {
                        RemoteShadow.UpdateShadowMovement(new Vector3(1, 0, 0));
                    }
                    break;
                }

            case EVENT_TYPE.ROTATE:
                {
                    if (RemoteShadow != null)
                    {
                        RemoteShadow.UpdateShadowRotation();
                    }
                    break;
                }

            case EVENT_TYPE.DELETE_ROW:
                {
                    deleteFullRows();
                    break;
                }
        }
    }




    // The Grid itself
    public static int w = 10;
    public static int h = 20;
    public static Transform[,] shadowGrid = new Transform[w, h];
    public static int cons = 0;
    public static bool prev = false;
    public static bool updated = true;


    public static Vector2 roundVec2(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x),
                           Mathf.Round(v.y));
    }

    public static void SpawnShadow(ShadowGroup obj/*, Group original*/)
    {
        Vector3 block_pos = new Vector3();

        if (ShadowGridStart == null)
        {
            ShadowGridStart = GameObject.Find("ShadowGridStart");
        }

  //      Debug.Log("ShadowGridStart->"+ ShadowGridStart.name);
        block_pos.x = ShadowGridStart.transform.localPosition.x +4;
        block_pos.y = ShadowGridStart.transform.localPosition.y+15;

        // original.MyShadow=(ShadowGroup)Instantiate(obj, block_pos, Quaternion.identity);
        RemoteShadow = Instantiate(obj, block_pos, Quaternion.identity);
    }
 
    public static void deleteRow(int y)
    {
        for (int x = 0; x < w; ++x)
        {
            Destroy(shadowGrid[x, y].gameObject);
            shadowGrid[x, y] = null;
        }
    }

    public static void decreaseRow(int y)
    {
        for (int x = 0; x < w; ++x)
        {
            if (shadowGrid[x, y] != null)
            {
                // Move one towards bottom
                shadowGrid[x, y - 1] = shadowGrid[x, y];
                shadowGrid[x, y] = null;

                // Update Block position
                shadowGrid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public static void decreaseRowsAbove(int y)
    {
        for (int i = y; i < h; ++i)
            decreaseRow(i);
    }

    public static bool isRowFull(int y)
    {
        for (int x = 0; x < w; ++x)
            if (shadowGrid[x, y] == null)
            {
                prev = false;
                return false;
            }
        prev = true;
        return true;
    }

    public static void deleteFullRows()
    {
        cons = 0;
        prev = false;
        for (int y = 0; y < h; ++y)
        {
            if (isRowFull(y))
            {
                deleteRow(y);
                decreaseRowsAbove(y + 1);
                --y;
            }
        }
    }



}
