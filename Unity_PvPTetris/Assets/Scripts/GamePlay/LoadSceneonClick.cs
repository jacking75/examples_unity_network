using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/* 
 * v0.0.1-r11
 * Written by Veritas83
 * www.NigelTodman.com
 * /Scripts/LoadSceneonClick.cs
 */
public class LoadSceneonClick : MonoBehaviour
{
    public void LoadByIndex(int sceneIndex)
    {
        DestroyImmediate(GameObject.Find("GameOverPanel(Clone)"));
        SceneManager.LoadScene(sceneIndex);
    }
}
