using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameBottle : MonoBehaviour
{
    public float damage = 100;
    public float timeToLive = 6;

    public AudioClip sound;
    public FlameFireZone flameZone;
    Vector2 targetPos = Vector2.zero;
    float moveSpeed = 20;
    bool isWorked = false;

    public void Init(Vector2 _targetPos)
    {
        targetPos = _targetPos;
    }

    private void OnEnable()
    {
        isWorked = false;
    }

    void Update()
    {
        if (isWorked)
            return;

        if (targetPos == Vector2.zero)
            targetPos = transform.position;

        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            isWorked = true;
            SoundManager.PlaySfx(sound);
            Instantiate(flameZone.gameObject, transform.position, Quaternion.identity).GetComponent<FlameFireZone>().Init(timeToLive, damage);
            gameObject.SetActive(false);
        }
    }
}
