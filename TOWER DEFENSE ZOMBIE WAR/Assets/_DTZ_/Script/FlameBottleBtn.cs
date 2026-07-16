using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlameBottleBtn : MonoBehaviour
{
    public GameObject locator;
    public FlameBottle flameBottle;
    public AudioClip throwSound;
    public Text costTxt;
    GameObject currentLocator;
    Button btn;
    float limitY = -5.5f;       //don't allow throw the grenade if the point is lower than this Y value

    private void Start()
    {
        btn = GetComponent<Button>();
        costTxt.text = FuelBarController.Instance.flameBottleCost + "";
    }

    private void OnDisable()
    {
        if (currentLocator != null)
        {
            Destroy(currentLocator);
        }
    }

    public void BeginDrag()
    {
        if (!btn.interactable)
            return;

        if (currentLocator == null)
            currentLocator = Instantiate(locator);

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.x = 0;
        currentLocator.transform.position = pos;
    }

    public void OnDrag()
    {
        if (currentLocator != null)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.x = 0;
            currentLocator.transform.position = pos;

            if (Tutorial.Instance.isWorking)
                Tutorial.Instance.Close();
        }
    }

    public void EndDrag()
    {
        if (currentLocator != null)
        {
            if (currentLocator.transform.position.y > limitY)
            {
                Instantiate(flameBottle.gameObject, new Vector3(0, -6, 0), Quaternion.identity).GetComponent<FlameBottle>().Init(currentLocator.transform.position);
                SoundManager.PlaySfx(throwSound);
                FuelBarController.Instance.UseFlameBottle();
            }
            else
            {
                FloatingTextManager.Instance.ShowText("Don't throw it here!", Vector2.zero, Color.yellow, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
            Destroy(currentLocator);
        }
    }
}
