using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelBarController : MonoBehaviour
{
    public static FuelBarController Instance;
    [Header("FUEL BAR")]
    public Slider fuelBarSlider;
    public Text fuelBarTxt;
    public float fuelRechargeSpeed = 5;
    public float fuelMax = 100;
    float currentFuel = 0;
    float lerpSpeed = 2;
    //public int getCurrentFuel { get { return currentFuel; } }

    [Header("*** GRENADE ***")]
    public int grenadeCost = 25;
    public float grenadeRechargeTime = 10;
    public Button grenadeBtn;
    public Image grenadeFillImage;
    float grenadeRechargeTimeCounter;

    public bool canUseGrenade { get { return grenadeBtn.interactable; } }

    [Header("*** FLAME BOTTLE ***")]
    public int flameBottleCost = 25;
    public float flameBottleRechargeTime = 10;
    public Button flameBottleBtn;
    public Image flameBottleFillImage;
    float flameBottleRechargeTimeCounter;

    public bool canUseFlameBottle { get { return flameBottleBtn.interactable; } }

    void Awake()
    {
        Instance = this;
        currentFuel = fuelMax;

        grenadeRechargeTimeCounter = grenadeRechargeTime;
        flameBottleRechargeTimeCounter = flameBottleRechargeTime;
    }

    void Update()
    {
        var percent = Mathf.Clamp01(currentFuel / fuelMax);
        fuelBarSlider.value = Mathf.Lerp(fuelBarSlider.value, percent, lerpSpeed * Time.deltaTime);
        fuelBarTxt.text = (int) currentFuel + "/" + (int)fuelMax;

        if (currentFuel < fuelMax)
            currentFuel += fuelRechargeSpeed * Time.deltaTime;

        grenadeBtn.interactable = (currentFuel >= grenadeCost) && (grenadeRechargeTimeCounter == 0);
        flameBottleBtn.interactable = (currentFuel >= flameBottleCost )&& (flameBottleRechargeTimeCounter == 0);

        //Grenade
        if (grenadeRechargeTimeCounter > 0)
            grenadeRechargeTimeCounter -= Time.deltaTime;

        grenadeRechargeTimeCounter = Mathf.Max(0, grenadeRechargeTimeCounter);
        grenadeFillImage.fillAmount = grenadeRechargeTimeCounter / grenadeRechargeTime;

        //Flame
        if (flameBottleRechargeTimeCounter > 0)
            flameBottleRechargeTimeCounter -= Time.deltaTime;

        flameBottleRechargeTimeCounter = Mathf.Max(0, flameBottleRechargeTimeCounter);
        flameBottleFillImage.fillAmount = flameBottleRechargeTimeCounter / flameBottleRechargeTime;
    }

    public void UseGrenade()
    {
        grenadeRechargeTimeCounter = grenadeRechargeTime;
        currentFuel -= grenadeCost;
    }

    public void UseFlameBottle()
    {
        flameBottleRechargeTimeCounter = flameBottleRechargeTime;
        currentFuel -= flameBottleCost;
    }
}
