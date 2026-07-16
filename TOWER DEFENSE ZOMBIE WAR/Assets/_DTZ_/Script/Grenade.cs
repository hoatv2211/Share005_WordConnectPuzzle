using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float damage = 100;
    public float radius = 3;
    public LayerMask targetLayer;
    public AudioClip sound;
    public GameObject blowFX;
    Vector2 targetPos = Vector2.zero;

    bool isWorked = false;
    float moveSpeed = 20;

    public void Init(Vector2 _targetPos, float _damage = -1)
    {
        targetPos = _targetPos;
        if (_damage != -1)
            damage = _damage;
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
            var hits = Physics2D.CircleCastAll(transform.position, radius, Vector2.zero, 0, targetLayer);
            if (hits.Length > 0)
            {
                foreach (var obj in hits)
                {
                    obj.collider.gameObject.GetComponent<ICanTakeDamage>().TakeDamage(damage, Vector2.zero, obj.point, gameObject);
                }
            }

            SoundManager.PlaySfx(sound);
            if (blowFX)
                SpawnSystemHelper.GetNextObject(blowFX, true).transform.position = transform.position;

            gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
