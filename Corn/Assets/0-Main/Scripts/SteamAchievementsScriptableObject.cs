using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(order = 1, fileName = "Steam Achievements")]
public class SteamAchievementsScriptableObject : ScriptableObject
{
    public List<SteamAchievement> AllSteamAchievements = new List<SteamAchievement>();

    public SteamAchievement GetAchievementFromName(string name)
    { 
        return AllSteamAchievements.Find(x => x.Name == name);
    }
}

[System.Serializable]
public class SteamAchievement
{
    public string Name;
    public string apiName;

}
