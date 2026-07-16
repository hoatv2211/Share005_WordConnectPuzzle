using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddAndUpgradePlayer : MonoBehaviour, IGetTouchEvent
{
    public int unlockAtLevel = 0;
    //int beginPlayer = 0;
    public GameObject addIcon, upgradeIcon, locked;
    public GameObject upgradeFX;
    public List<int> prices = new List<int>();
    public SoldierController[] Players;
    public TextMesh priceTxt;
    public TextMesh unlockAtLevelTxt;

    int currentPlayer = -1;

    private void Awake()
    {
        foreach (var player in Players)
        {
            player.gameObject.SetActive(false);
        }

        if (GameMode.Instance && GlobalValue.levelPlaying < unlockAtLevel)
        {
            addIcon.SetActive(false);
            upgradeIcon.SetActive(false);
            locked.SetActive(true);
           
            unlockAtLevelTxt.text = "Unlock at \n Level " + unlockAtLevel;
            Destroy(this);
        }else
            locked.SetActive(false);
    }

    void Start()
    {
        if (Players.Length <= 0)
        {
            Debug.LogError("No player in " + gameObject.name);
            enabled = false;
            return;
        }

        if (GameMode.Instance && GlobalValue.levelPlaying == 1)     //only show tutorial when begin game from the Logo scene
            StartCoroutine(WaitAndShowTutorialCo());

        InvokeRepeating("CheckStatus", 0, 0.2f);
    }

    IEnumerator WaitAndShowTutorialCo()     //wait and check if player hire the gunner or not to show the tutorial
    {
        yield return new WaitForSeconds(5);
        if (currentPlayer == -1)
        {
            while (prices[currentPlayer + 1] > GlobalValue.SavedCoins) { yield return null; }       //wait until enough coin to upgrade

            MenuManager.Instance.ShowHireMeTutorial(true);
            Time.timeScale = 0;
            while (currentPlayer == -1) { yield return null; }
            MenuManager.Instance.ShowHireMeTutorial(false);
            Time.timeScale = 1;

            //wait for upgrade
            yield return new WaitForSeconds(10);

            while (prices[currentPlayer + 1] > GlobalValue.SavedCoins) { yield return null; }       //wait until enough coin to upgrade

            MenuManager.Instance.ShowUpgradeMeTutorial(true);
            Time.timeScale = 0;
            while (currentPlayer == 0) { yield return null; }
            MenuManager.Instance.ShowUpgradeMeTutorial(false);
            Time.timeScale = 1;
        }
    }

    private void CheckStatus()
    {
        upgradeIcon.SetActive(currentPlayer + 1 < Players.Length);
        if (currentPlayer + 1 < Players.Length)
            priceTxt.text = prices[currentPlayer + 1] + "";

    }

    void SetPlayer()
    {
        foreach (var player in Players)
        {
            player.gameObject.SetActive(false);
        }

        Players[currentPlayer].gameObject.SetActive(true);
        if (upgradeFX)
            SpawnSystemHelper.GetNextObject(upgradeFX, true).transform.position = transform.position;
    }

    public void TouchEvent()
    {
        if ((currentPlayer + 1 >= Players.Length))
            return;

        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;

        if(prices[currentPlayer + 1] > GlobalValue.SavedCoins)
        {
            FloatingTextManager.Instance.ShowText("Not Enough Coins!", Vector2.zero, Color.yellow, Vector2.zero);
            SoundManager.PlaySfx(SoundManager.Instance.soundNotEnoughCoin);
            return;
        }

        if ((addIcon.activeInHierarchy || upgradeIcon.activeInHierarchy))
        {
            addIcon.SetActive(false);
            currentPlayer++;
            GlobalValue.SavedCoins -= prices[currentPlayer];
            SetPlayer();
            SoundManager.PlaySfx(currentPlayer == 0 ? SoundManager.Instance.soundAddArcher : SoundManager.Instance.soundUpgradeArcher);
        }
    }
}
