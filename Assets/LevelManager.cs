using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public void LoadLevel(string sceneName)
    {
        SteamVR_LoadLevel levelLoader = GetComponent<SteamVR_LoadLevel>();
        levelLoader.levelName = sceneName;
        levelLoader.Trigger();
        
    }
}
