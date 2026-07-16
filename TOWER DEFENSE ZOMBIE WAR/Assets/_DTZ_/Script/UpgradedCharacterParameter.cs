using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class UpgradeStep
{
    public int price;
    public int rangeDamageStep;
}

public class UpgradedCharacterParameter : MonoBehaviour
{
    public string ID = "unique ID";
    [Header("Default")]
    public UpgradeStep[] UpgradeSteps;
    
    public int CurrentUpgrade
    {
        get
        {
            int current = PlayerPrefs.GetInt(ID + "upgradeHealth" + "Current", 0);
            if (current >= UpgradeSteps.Length)
                current = -1;   //-1 mean overload
            return current;
        }
        set
        {
            PlayerPrefs.SetInt(ID + "upgradeHealth" + "Current", value);
        }
    }

    public void UpgradeCharacter()
    {
        if (CurrentUpgrade == -1)
            return;

        UpgradeRangeDamage += UpgradeSteps[CurrentUpgrade].rangeDamageStep;
        CurrentUpgrade++;
    }
    
    public int UpgradeRangeDamage
    {
        get { return PlayerPrefs.GetInt(ID + "UpgradeRangeDamage", 0); }
        set { PlayerPrefs.SetInt(ID + "UpgradeRangeDamage", value); }
    }
}
