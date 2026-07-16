using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardedVideoBtn : MonoBehaviour
{
    public AudioClip soundRewarded;
    public Text rewardedTxt;

    private void Start()
    {
        rewardedTxt.text = "+" + AdsManager.Instance.getRewarded;
    }

    public void Buy()
    {
        if (AdsManager.Instance && AdsManager.Instance.isRewardedAdReady())
        {
            AdsManager.AdResult += AdsManager_AdResult;
            AdsManager.Instance.ShowRewardedAds();
        }
    }

    private void AdsManager_AdResult(bool isSuccess, int rewarded)
    {
        AdsManager.AdResult -= AdsManager_AdResult;
        if (isSuccess)
        {
            GlobalValue.SavedCoins += rewarded;
            SoundManager.PlaySfx(soundRewarded);
        }
    }
}
