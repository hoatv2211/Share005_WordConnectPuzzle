using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheFortrest : MonoBehaviour, ICanTakeDamage
{
    public float maxHealth = 300;

    [ReadOnly] public float extraHealth = 0;
    [ReadOnly] public float currentHealth;

    [Header("SHAKNG")]
    public float speed = 30f; //how fast it shakes
    public float amount = 0.5f; //how much it shakes
    public float shakeTime = 0.3f;
    public bool shakeX, shakeY;

    Vector2 startingPos;
    IEnumerator ShakeCoDo;

    void Awake()
    {
        startingPos = transform.position;
    }

    IEnumerator ShakeCo(float time)
    {
        float counter = 0;
        while (counter < time)
        {
            transform.position = startingPos + new Vector2(Mathf.Sin(Time.time * speed) * amount * (shakeX ? 1 : 0), Mathf.Sin(Time.time * speed) * amount * (shakeY ? 1 : 0));

            yield return null;
            counter += Time.deltaTime;
        }

        transform.position = startingPos;
    }

    void Start()
    {
        extraHealth = maxHealth * GlobalValue.StrongWallExtra;
        maxHealth += extraHealth;
        currentHealth = maxHealth;
        MenuManager.Instance.UpdateHealthbar(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage, Vector2 force, Vector2 hitPoint, GameObject instigator, BODYPART bodyPart = BODYPART.NONE, WeaponEffect weaponEffect = null, WEAPON_EFFECT forceEffect = WEAPON_EFFECT.NORMAL)
    {
        currentHealth -= damage;
        FloatingTextManager.Instance.ShowText("" + (int)damage, Vector2.up * 2, Color.yellow, transform.position);

        MenuManager.Instance.UpdateHealthbar(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
           GameManager.Instance.GameOver();
            gameObject.SetActive(false);
        }
        else
        {
            if (ShakeCoDo != null)
                StopCoroutine(ShakeCoDo);

            ShakeCoDo = ShakeCo(shakeTime);
            StartCoroutine(ShakeCoDo);
        }
    }
}
