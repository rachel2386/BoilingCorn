using Steamworks;
using UnityEngine;

public class SteamAchievementsHandler : MonoBehaviour
{

    [SerializeField] SteamAchievementsScriptableObject steamAchievementData;
    public void UnlockAchievement(string achievementName)
    {
        if (!SteamManager.Initialized) return;
               
        var achievement = steamAchievementData.GetAchievementFromName(achievementName);
        if (achievement == null) return;

        print("achievement unlocked!" + achievement.apiName);

        
        //set achievement flag
        SteamUserStats.SetAchievement(achievement.apiName);

        // push to steam
        SteamUserStats.StoreStats();

       

    }

    public bool IsAchievementUnlocked(string apiName)
    { 
        if(!SteamManager.Initialized) return false;

        bool achieved;
        SteamUserStats.GetAchievement(apiName, out achieved);
        return achieved;
        
    
    }
}
