using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ShadowGroup : MonoBehaviour
{
    public float lastFall = 0;
    public static int InputCnt = 0; //사람이 초당 몇번을 
    void Start()
    {
       
    }

    void Update()
    {
    }


    public void UpdateShadowMovement(Vector3 vector)  {
        transform.position += vector;
        InputCnt++;
        updateGrid();
        
    }

    public void UpdateShadowRotation()
    {
        transform.Rotate(0, 0, -90);
        InputCnt++;
        updateGrid();
    }

    public void SetEnable(bool val)
    {
        enabled = val;

    }

    void updateGrid()
    {
        // Remove old children from grid
        for (int y = 0; y < ShadowGrid.h; ++y)
        {
            for (int x = 0; x < ShadowGrid.w; ++x)
            {
                if (ShadowGrid.shadowGrid[x, y] != null)
                {
                    if (ShadowGrid.shadowGrid[x, y].parent == transform)
                    {
                        ShadowGrid.shadowGrid[x, y] = null;
                    }
                }
            }
        }


        Vector2 shadowGridPos = ShadowGrid.ShadowGridStart.transform.position - GameObject.Find("RemoteGame").transform.position;

        // Add new children to grid
        foreach (Transform child in transform)
        {
            Vector2 v = ShadowGrid.roundVec2(child.position) - shadowGridPos;
            ShadowGrid.shadowGrid[(int)v.x, (int)v.y] = child;
        }

    }

    public static explicit operator ShadowGroup(GameObject v)
    {
        throw new NotImplementedException();
    }
}
