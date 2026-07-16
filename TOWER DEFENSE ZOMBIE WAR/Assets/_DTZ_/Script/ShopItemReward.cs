using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class ShopItemReward : MonoBehaviour {
    public string itemName = "ITEM NAME";
	public enum ItemType{Lightning, Posion, Freeze}
	public ItemType itemType;

	public int rewardedUnit = 1;

    public Text nameTxt;
	public Text rewardedAmountTxt;
	public Text currentAmountTxt;

	[ReadOnly] public int coinPrice = 1;
	public Text coinTxt;

	void OnEnable(){
		UpdateAmount ();
	}

	void Start(){
        if (GameMode.Instance)
        {
            switch (itemType)
            {
                case ItemType.Lightning:
                    coinPrice = GameMode.Instance.lightningArrowPrice;
                    break;
                case ItemType.Posion:
                    coinPrice = GameMode.Instance.poisonArrowPrice;
                    break;
                case ItemType.Freeze:
                    coinPrice = GameMode.Instance.freezeArrowPrice;
                    break;
                default:
                    break;
            }
        }

        UpdateAmount ();

		rewardedAmountTxt.text = "+" + rewardedUnit;
		coinTxt.text = coinPrice.ToString ();
        nameTxt.text = itemName;
    }

	public void UseCoin(){
		var coins = GlobalValue.SavedCoins;
        if (coins >= coinPrice)
        {
            coins -= coinPrice;
            GlobalValue.SavedCoins = coins;

            DoReward();
        }
        else
        {
            SoundManager.PlaySfx(SoundManager.Instance.soundNotEnoughCoin);
            if (AdsManager.Instance && AdsManager.Instance.isRewardedAdReady())
                NotEnoughCoins.Instance.ShowUp();
        }
	}

	private void DoReward(){
        switch (itemType)
        {
            case ItemType.Lightning:
                GlobalValue.ItemLightning += rewardedUnit;
                break;
            case ItemType.Posion:
                GlobalValue.ItemPoison += rewardedUnit;
                break;
            case ItemType.Freeze:
                GlobalValue.ItemFreeze += rewardedUnit;
                break;
            default:
                break;
        }

		UpdateAmount ();
        SoundManager.PlaySfx(SoundManager.Instance.soundPurchased);
	}

    private void UpdateAmount()
    {
        switch (itemType)
        {
            case ItemType.Lightning:
                currentAmountTxt.text = "Remain: " + GlobalValue.ItemLightning;
                break;
            case ItemType.Posion:
                currentAmountTxt.text = "Remain: " + GlobalValue.ItemPoison;
                break;
            case ItemType.Freeze:
                currentAmountTxt.text = "Remain: " + GlobalValue.ItemFreeze;
                break;
            default:
                break;
        }
    }
}
