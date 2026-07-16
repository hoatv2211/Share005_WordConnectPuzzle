using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopCharacterUpgrade : MonoBehaviour
{
    public UpgradedCharacterParameter characterID;
    [Space]
    public Text upgradeRangeDamageStep;
    public Text totolExtraDamage;
    public Text price;

    public GameObject dot;
    public GameObject dotHoder;
    List<Image> upgradeDots;

    public Sprite dotImageOn, dotImageOff;

    void Start()
    {
        upgradeDots = new List<Image>();
        upgradeDots.Add(dot.GetComponent<Image>());
        for (int i = 1; i < characterID.UpgradeSteps.Length; i++)
        {
            upgradeDots.Add(Instantiate(dot, dotHoder.transform).GetComponent<Image>());
        }

        UpdateParameter();
    }

    void UpdateParameter()
    {
        if (characterID.CurrentUpgrade != -1)
        {
            price.text = characterID.UpgradeSteps[characterID.CurrentUpgrade].price + "";
            upgradeRangeDamageStep.text = "+" + characterID.UpgradeSteps[characterID.CurrentUpgrade].rangeDamageStep;

            SetDots(characterID.CurrentUpgrade);
        }
        else
        {
            price.text = "MAX";
            upgradeRangeDamageStep.gameObject.SetActive(false);
            SetDots(upgradeDots.Count);
        }
        totolExtraDamage.text = "Extra Damage = " + characterID.UpgradeRangeDamage;
    }

    void SetDots(int number)
    {
        for (int i = 0; i < upgradeDots.Count; i++)
        {
            if (i < number)
                upgradeDots[i].sprite = dotImageOn;
            else
                upgradeDots[i].sprite = dotImageOff;
        }
    }

    public void Upgrade()
    {
        if (characterID.CurrentUpgrade == -1)
            return;

        if(GlobalValue.SavedCoins >= characterID.UpgradeSteps[characterID.CurrentUpgrade].price)
        {
            GlobalValue.SavedCoins -= characterID.UpgradeSteps[characterID.CurrentUpgrade].price;
            SoundManager.PlaySfx(SoundManager.Instance.soundUpgrade);

            characterID.UpgradeCharacter();
            UpdateParameter();
        }
    }
}
