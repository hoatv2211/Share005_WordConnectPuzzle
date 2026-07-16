using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoostItemUI : MonoBehaviour
{
    public static BoostItemUI Instance;
    [ReadOnly] public WEAPON_EFFECT currentEffect = WEAPON_EFFECT.NORMAL;

    [Header("Lightning Arrow")]
    public Text LA_remainTxt;
    public Button LA_Button;
    public int LA_Time = 25;
    [ReadOnly] public float LA_TimeCounter = 0;

    [Header("Poison Arrow")]
    public Text PA_remainTxt;
    public Button PA_Button;
    public int PA_Time = 30;
    [ReadOnly] public float PA_TimeCounter = 0;

    [Header("Freeze Arrow")]
    public Text FA_remainTxt;
    public Button FA_Button;
    public int FA_Time = 30;
    [ReadOnly] public float FA_TimeCounter = 0;

    [Header("Boost Item")]
    public Animator boostItemAnim;
    public Animator boostButtonAnim;
    public float boostItemautoHide = 3;

    [Space]

    public Text timeRemainTxt;
    public GameObject activeIcons;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LA_remainTxt.text = "x" + GlobalValue.ItemLightning;
        PA_remainTxt.text = "x" + GlobalValue.ItemPoison;
        FA_remainTxt.text = "x" + GlobalValue.ItemFreeze;

        LA_Button.interactable = GlobalValue.ItemLightning > 0;
        PA_Button.interactable = GlobalValue.ItemPoison > 0;
        FA_Button.interactable = GlobalValue.ItemFreeze > 0;
    }

    private void Update()
    {
        LA_Button.interactable = currentEffect != WEAPON_EFFECT.LIGHTING && GlobalValue.ItemLightning > 0;
        PA_Button.interactable = currentEffect != WEAPON_EFFECT.POISON && GlobalValue.ItemPoison > 0; ;
        FA_Button.interactable = currentEffect != WEAPON_EFFECT.FREEZE && GlobalValue.ItemFreeze > 0; ;

        activeIcons.SetActive(currentEffect!= WEAPON_EFFECT.NORMAL);
    }


    #region Lightning Arrow
    public void ActiveLightningArrow()
    {
        if (currentEffect == WEAPON_EFFECT.LIGHTING)
            return;
        StopAllCoroutines();

        SoundManager.PlaySfx(SoundManager.Instance.BTsoundUseBoost);
        GlobalValue.ItemLightning--;
        LA_remainTxt.text = "x" + GlobalValue.ItemLightning;

        currentEffect = WEAPON_EFFECT.LIGHTING;


        RunTimerAutoHideBoostPanel();

        StartCoroutine(LightningArrowTimerCo());
    }

    IEnumerator LightningArrowTimerCo()
    {
        LA_TimeCounter = (float)LA_Time;
        while (LA_TimeCounter > 0)
        {
            LA_TimeCounter -= Time.deltaTime;
            timeRemainTxt.text = (int)LA_TimeCounter + "";
            yield return null;
        }

        currentEffect = WEAPON_EFFECT.NORMAL;
    }
    #endregion

    #region Poison Arrow
    public void ActivePoisonArrow()
    {
        if (currentEffect == WEAPON_EFFECT.POISON)
            return;
        StopAllCoroutines();

        SoundManager.PlaySfx(SoundManager.Instance.BTsoundUseBoost);
        GlobalValue.ItemPoison--;
        PA_remainTxt.text = "x" + GlobalValue.ItemPoison;
        currentEffect = WEAPON_EFFECT.POISON;

        RunTimerAutoHideBoostPanel();
        StartCoroutine(PoisonArrowTimerCo());
    }

    IEnumerator PoisonArrowTimerCo()
    {
        PA_TimeCounter = (float)PA_Time;
        while (PA_TimeCounter > 0)
        {
            PA_TimeCounter -= Time.deltaTime;
            timeRemainTxt.text = (int)PA_TimeCounter + "";
           yield return null;
        }

        currentEffect = WEAPON_EFFECT.NORMAL;
    }
    #endregion

    #region Freeze Arrow
    public void ActiveFrezzeArrow()
    {
        if (currentEffect == WEAPON_EFFECT.FREEZE)
            return;
        StopAllCoroutines();

        SoundManager.PlaySfx(SoundManager.Instance.BTsoundUseBoost);
        GlobalValue.ItemFreeze--;
        FA_remainTxt.text = "x" + GlobalValue.ItemFreeze;
        currentEffect = WEAPON_EFFECT.FREEZE;

        RunTimerAutoHideBoostPanel();

        StartCoroutine(FrezzeArrowTimerCo());
    }

    IEnumerator FrezzeArrowTimerCo()
    {
        FA_TimeCounter = (float)FA_Time;
        while(FA_TimeCounter > 0)
        {
            FA_TimeCounter -= Time.deltaTime;
            timeRemainTxt.text = (int)FA_TimeCounter + "";
            yield return null;
        }

        currentEffect = WEAPON_EFFECT.NORMAL;
    }
    #endregion

    #region BOOST PANEL
    IEnumerator BoostItemHideCoDo;
    public void BoostItem()
    {
        if (boostItemAnim.GetBool("show"))
        {
            HideBoostPanel();
        }
        else
        {
            SoundManager.PlaySfx(SoundManager.Instance.BTsoundOpen);
            boostItemAnim.SetBool("show", true);
            boostButtonAnim.SetBool("on", true);
            RunTimerAutoHideBoostPanel();
        }
    }

    void RunTimerAutoHideBoostPanel()
    {
        if (BoostItemHideCoDo != null)
            StopCoroutine(BoostItemHideCoDo);

        BoostItemHideCoDo = BoostItemHideCo(boostItemautoHide);
        StartCoroutine(BoostItemHideCoDo);
    }

    IEnumerator BoostItemHideCo(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideBoostPanel();
    }

    void HideBoostPanel()
    {
        SoundManager.PlaySfx(SoundManager.Instance.BTsoundHide);
        boostItemAnim.SetBool("show", false);
        boostButtonAnim.SetBool("on", false);
    }

    #endregion
}
