using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public bool HasCompletedStoryMode;

    //the values defined in this constructor will be the default values
    //the game starts with when there's no data to load (new game)
    public GameData()
    {
        this.HasCompletedStoryMode = false;
    
    }
}
