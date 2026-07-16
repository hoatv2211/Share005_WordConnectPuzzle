using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUpgrade : MonoBehaviour
{
    public string itemName = "ITEM NAME";
    public string infor = "information for item";
    public int maxUpgrade = 5;
    public Image[] upgradeDots;
    public Sprite dotImageOn, dotImageOff;
    public Text nameTxt, currentExtraTxt, nextUpgradeTxt;
    [HideInInspector] public int coinPrice = 1;
    public Text coinTxt;

    public Button upgradeButton;

    [Header("Strong Wall")]
    [Range(1,100)]
    public int StrongPerUpgrade = 20;       //inscrease health percent

    void Start()
    {
        if (GameMode.Instance)
        {
            coinPrice = GameMode.Instance.upgradeFortressPrice;
        }
        nameTxt.text = itemName;
        coinTxt.text = coinPrice + "";

        UpdateStatus();
    }

    void UpdateStatus()
    {
        currentExtraTxt.text = "Current: " + GlobalValue.StrongWallExtra + "% Health" ;
        int currentUpgrade = GlobalValue.UpgradeStrongWall;
        if (currentUpgrade >= maxUpgrade)
        {
            coinTxt.text = "MAX";
            upgradeButton.interactable = false;
            upgradeButton.GetComponent<Image>().enabled = false;
            SetDots(maxUpgrade);
        }
        else
        {
            nextUpgradeTxt.text = "+" + StrongPerUpgrade + "%";
            SetDots(currentUpgrade);
        }
    }

    void SetDots(int number)
    {
        for (int i = 0; i < upgradeDots.Length; i++)
        {
            if (i < number)
                upgradeDots[i].sprite = dotImageOn;
            else
                upgradeDots[i].sprite = dotImageOff;

            if (i >= maxUpgrade)
                upgradeDots[i].gameObject.SetActive(false);
        }
    }

    public void Upgrade()
    {
        if (GlobalValue.SavedCoins >= coinPrice)
        {
            SoundManager.PlaySfx(SoundManager.Instance.soundUpgrade);
            GlobalValue.SavedCoins -= coinPrice;

            GlobalValue.UpgradeStrongWall++;
            GlobalValue.StrongWallExtra += (float) StrongPerUpgrade /100f;
            
            UpdateStatus();
        }
        else
        {
            SoundManager.PlaySfx(SoundManager.Instance.soundNotEnoughCoin);
            if (AdsManager.Instance && AdsManager.Instance.isRewardedAdReady())
                NotEnoughCoins.Instance.ShowUp();
        }
    }
}
