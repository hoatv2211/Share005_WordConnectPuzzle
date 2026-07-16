using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial Instance;

    public float delayShow = 3;
    public GameObject tutContainer;
    public GameObject closeBtn;
    [HideInInspector] public bool isWorking = false;

    IEnumerator Start()
    {
        Instance = this;

        if (GlobalValue.levelPlaying == 1)
        {
            isWorking = true;
            tutContainer.SetActive(false);
            closeBtn.SetActive(false);
            yield return null;

            while (!FuelBarController.Instance.canUseGrenade && !FuelBarController.Instance.canUseFlameBottle) { yield return null; }

            Time.timeScale = 0;
            tutContainer.SetActive(true);
            
        }
    }

    public void ShowCloseButton()
    {
        closeBtn.SetActive(true);
    }

    public void Close()
    {
        Time.timeScale = 1;
        tutContainer.SetActive(false);
        closeBtn.SetActive(false);
        isWorking = false;
    }
}
