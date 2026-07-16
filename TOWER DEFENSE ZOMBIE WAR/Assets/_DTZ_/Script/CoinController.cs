using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    Animator anim;
    int add = 0;
    public void Init(int amount)
    {
        add = amount;
    }

    private void OnEnable()
    {
        StartCoroutine(WorkingCo());
    }

    IEnumerator WorkingCo()
    {
        anim = GetComponent<Animator>();
        yield return new WaitForSeconds(1);
        anim.SetBool("collected", true);

        GlobalValue.SavedCoins += add;
        FloatingTextManager.Instance.ShowText((int)add + "", Vector2.up * 1, Color.yellow, transform.position);

        SoundManager.PlaySfx(SoundManager.Instance.coinCollect);
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }
}
