using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("ADDP/Enemy AI/Melee Attack")]
public class EnemyMeleeAttack : MonoBehaviour {
	public LayerMask targetLayer;
    [ReadOnly] public int maxTargetPerHit = 1;       //how many target damaged per hit
    public Vector2 checkPointOffset = new Vector2(0, -1);
    public float radiusCheck = 1;
    public float dealDamage = 20;
    public float attackDamageDelay = 0.5f;      //delay amount of time before check the target and deal the damage
    [Range(1, 100)]
    public int criticalPercent = 10;
	public float meleeRate = 1;
	float lastShoot = -999;
	public bool isAttacking { get; set; }
	[HideInInspector] public GameObject MeleeObj;
    [Range(0, 1)]
    public float soundAttacksVol = 0.5f;
    public AudioClip[] soundAttacks;
    WeaponEffect hasWeaponEffect;

	public bool AllowAction(){
		return Time.time - lastShoot > meleeRate;
	}

    public bool CheckPlayer () {
        RaycastHit2D hit = Physics2D.CircleCast((Vector2)transform.position + checkPointOffset, radiusCheck, Vector2.zero, 0, targetLayer);
        if (hit)
            return true;
        else
            return false;
	}

    public void Check4Hit()
    {
        StartCoroutine(Check4HitCo());
    }

    IEnumerator Check4HitCo() {
        lastShoot = Time.time;
        yield return new WaitForSeconds(attackDamageDelay);

        RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2) transform.position + checkPointOffset, radiusCheck * 1.2f, Vector2.zero, 0, targetLayer);
        int counterHit = 0;
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if (counterHit < maxTargetPerHit)
                {
                    var takeDamage = (ICanTakeDamage)hit.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
                    if (takeDamage != null)
                    {
                        float _damage = dealDamage + (int)(Random.Range(-0.1f, 0.1f) * dealDamage);
                        if (Random.Range(0, 100) < criticalPercent)
                        {
                            _damage *= 2;
                            FloatingTextManager.Instance.ShowText("CRIT!", Vector3.up, Color.red, hit.collider.gameObject.transform.position, 30);
                        }

                        if (hasWeaponEffect != null)
                        {
                            takeDamage.TakeDamage(_damage, Vector2.zero, hit.point, gameObject, BODYPART.NONE, hasWeaponEffect);
                        }
                        else
                            takeDamage.TakeDamage(_damage, Vector2.zero, hit.point, gameObject);

                        counterHit++;
                    }

                    if (soundAttacks.Length > 0)
                        SoundManager.PlaySfx(soundAttacks[Random.Range(0, soundAttacks.Length)], soundAttacksVol);
                }
            }
        }

        yield return new WaitForSeconds(1);
        isAttacking = false;
    }

	void OnDrawGizmos(){
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere ((Vector2)transform.position + checkPointOffset, radiusCheck);
	}
}
