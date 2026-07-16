using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossbowShootPointBtn : MonoBehaviour
{
    public static CrossbowShootPointBtn Instance;

    public bool isWorking { get; set; }
    public Image percentImg;
    public GameObject tutorialTxt;

    [ReadOnly] public Vector2 forcePoint;

    public float forceTime = 5;

    public Transform crosshairObj;
    float counter;


    public GameObject autoShootingBtn;

    void Start()
    {
        Instance = this;
        tutorialTxt.SetActive(GlobalValue.levelPlaying == 1);
    }

    void Update()
    {
        if (counter > 0)
            counter -= Time.deltaTime;

        percentImg.fillAmount = Mathf.Clamp01(counter / forceTime);

        isWorking = counter > 0;

        crosshairObj.gameObject.SetActive(isWorking);

        autoShootingBtn.SetActive(isWorking);

    }

    public void SetPoint()
    {
        forcePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        counter = forceTime;
        crosshairObj.position = Input.mousePosition;
        tutorialTxt.SetActive(false);
    }

    public void StopWorking()
    {
        counter = 0;
    }
}