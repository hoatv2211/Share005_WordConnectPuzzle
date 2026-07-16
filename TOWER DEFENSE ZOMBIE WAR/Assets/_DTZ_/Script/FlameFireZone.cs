using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameFireZone : MonoBehaviour
{
    public Vector2 boxZone = new Vector2(10, 1.5f);
    public LayerMask targetAsLayer;
    public int damage = 30;
    public float damageRate = 1;
    public float timeToWork = 6;
    float beginTime;

    public void Init(float timeToLive, float _damage = -1)
    {
        timeToWork = timeToLive;
        if (_damage != -1)
            damage = (int) _damage;
    }

    private void Start()
    {
        StartCoroutine(WorkingCo());
    }

    IEnumerator WorkingCo()
    {
        beginTime = Time.time;
        RaycastHit2D[] hits;
        while (true)
        {
            hits = Physics2D.BoxCastAll(transform.position, boxZone, 0, Vector2.zero, 0, targetAsLayer);
            foreach (var hit in hits)
            {
                var enemyDamage = (ICanTakeDamage)hit.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
                if (enemyDamage != null)
                {
                    enemyDamage.TakeDamage(damage, Vector2.zero, hit.point, gameObject);
                }
            }


            yield return new WaitForSeconds(damageRate);

            if ((Time.time - beginTime) > timeToWork)
            {
                gameObject.SetActive(false);
                yield break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, boxZone);
    }
}
