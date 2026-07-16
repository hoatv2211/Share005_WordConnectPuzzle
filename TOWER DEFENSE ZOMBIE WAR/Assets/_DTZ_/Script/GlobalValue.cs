using UnityEngine;
using System.Collections;

public class GlobalValue : MonoBehaviour {
    public static bool isFirstOpenMainMenu = true;
	public static int worldPlaying = 1;
	public static int levelPlaying = 1;

    public static string WorldReached = "WorldReached";
	public static bool isSound = true;
	public static bool isMusic = true;

    public static bool isNewGame
    {
        get { return PlayerPrefs.GetInt("isNewGame", 0) == 0; }
        set { PlayerPrefs.SetInt("isNewGame", value ? 0 : 1); }
    }

	public static int SavedCoins{ 
		get { return PlayerPrefs.GetInt ("Coins", 200); } 
		set { PlayerPrefs.SetInt ("Coins", value); } 
	}
    
    public static int LevelPass { 
		get { return PlayerPrefs.GetInt ("LevelReached", 0); } 
		set { PlayerPrefs.SetInt ("LevelReached", value); } 
	}

	public static void LevelStar(int level, int stars){
		PlayerPrefs.SetInt ("LevelStars" + level, stars);
	}

	public static int LevelStar(int level){
		return PlayerPrefs.GetInt ("LevelStars" + level, 0); 
	}

	public static bool RemoveAds { 
		get { return PlayerPrefs.GetInt ("RemoveAds", 0) == 1 ? true : false; } 
		set { PlayerPrefs.SetInt ("RemoveAds", value ? 1 : 0); } 
	}

    public static int ItemPoison
    {
        get { return PlayerPrefs.GetInt("ItemPoison", 3); }
        set { PlayerPrefs.SetInt("ItemPoison", value); }
    }

    public static int ItemFreeze
    {
        get { return PlayerPrefs.GetInt("ItemFreeze", 3); }
        set { PlayerPrefs.SetInt("ItemFreeze", value); }
    }

    public static int ItemLightning
    {
        get { return PlayerPrefs.GetInt("ItemLightning", 3); }
        set { PlayerPrefs.SetInt("ItemLightning", value); }
    }

    public static int UpgradeStrongWall
    {
        get { return PlayerPrefs.GetInt("UpgradeStrongWall", 0); }
        set { PlayerPrefs.SetInt("UpgradeStrongWall", value); }
    }
    public static float StrongWallExtra
    {
        get { return PlayerPrefs.GetFloat("StrongWallExtra", 0); }
        set { PlayerPrefs.SetFloat("StrongWallExtra", value); }
    }
}
