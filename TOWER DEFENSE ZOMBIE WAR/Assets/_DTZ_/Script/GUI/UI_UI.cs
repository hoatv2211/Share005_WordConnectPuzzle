using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_UI : MonoBehaviour
{
    public float lerpSpeed = 2;

    [Header("PLAYER HEALTHBAR")]
    public Slider healthSlider;
    public Text health;
    float healthValue;


    [Header("ENEMY WAVE")]
    public Slider enemyWavePercentSlider;
    float enemyWaveValue;

    [Space]
    public Text coinTxt;
    public Text expTxt;
    public Text levelName;

    private void Start()
    {
        healthValue = 1;
        enemyWaveValue = 0;

        healthSlider.value = 1;
        enemyWavePercentSlider.value = 0;
        if(levelName)
        levelName.text = "Level " + GlobalValue.levelPlaying;
    }

    private void Update()
    {
        healthSlider.value = Mathf.Lerp(healthSlider.value, healthValue, lerpSpeed * Time.deltaTime);
        enemyWavePercentSlider.value = Mathf.Lerp(enemyWavePercentSlider.value, enemyWaveValue, lerpSpeed * Time.deltaTime);
        coinTxt.text = GlobalValue.SavedCoins + "";
    }

    public void UpdateHealthbar(float currentHealth, float maxHealth)
    {
            healthValue = Mathf.Clamp01(currentHealth / maxHealth);
            health.text = (int)currentHealth + "/" + (int)maxHealth;
    }

    public void UpdateEnemyWavePercent(float currentSpawn, float maxValue)
    {
        enemyWaveValue = Mathf.Clamp01(currentSpawn / maxValue);
    }
    
   
}
