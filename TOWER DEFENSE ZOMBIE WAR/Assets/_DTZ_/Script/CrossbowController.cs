using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowController : MonoBehaviour
{
   
    public enum SHOOTING_MODE { Manual, Auto}

    [Header("+++ SHOOTING SETUP +++")]
    public float speed = 20;
    public int damage = 20;
    public float shootingRate = 1f;
    public Transform firePoint;
    public LayerMask layerAsTarget;
    public float checkRadius = 6;
    public WeaponEffect weaponEffect;
    public AudioClip soundShoot;

    [Header("+++ ARROWS +++")]
    public ArrowProjectile normalArrow;
    public ArrowProjectile freezeArrow, lightningArrow, poisonedArrow;

    float lastShootTime = -999;
    [ReadOnly] public GameObject target;
    public Animator crossBowAnimator;

    void Start()
    {
        if(crossBowAnimator == null)
            crossBowAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;

        if (target != null || CrossbowShootPointBtn.Instance.isWorking)
        {
            var pos = CrossbowShootPointBtn.Instance.isWorking ? CrossbowShootPointBtn.Instance.forcePoint : (Vector2)target.transform.position;
            transform.up = pos - (Vector2) transform.position;
        }

        Shoot();    //call shooting overtime
    }

    void Shoot()
    {
        if ((Time.time - lastShootTime) > shootingRate)
        {
            if (CheckTarget() || CrossbowShootPointBtn.Instance.isWorking)
            {
                lastShootTime = Time.time;
                var arrow = SpawnSystemHelper.GetNextObject(GetArrow().gameObject, firePoint.position, true).GetComponent<ArrowProjectile>();

                var pos = CrossbowShootPointBtn.Instance.isWorking ? CrossbowShootPointBtn.Instance.forcePoint : (Vector2)target.transform.position;
                Vector2 shootingDirection = pos - (Vector2) transform.position;       //get the direction to look

                arrow.transform.up = shootingDirection;        //rotate the arrow to look at the target
                arrow.Init(damage, speed, Random.Range(0f, 1f) < 0.2f, weaponEffect, BoostItemUI.Instance.currentEffect);
                crossBowAnimator.SetTrigger("shoot");
                SoundManager.PlaySfx(soundShoot);
            }
        }
    }

    bool CheckTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, checkRadius, Vector2.zero, 0, layerAsTarget);
        float nearestDistance = 999;
        GameObject nearestTarget = null;
        foreach (var hit in hits)
        {
            var distance = Vector2.Distance(hit.transform.position, transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = hit.collider.gameObject;
            }
        }

        target = nearestTarget;
        return target != null;
    }

    ArrowProjectile GetArrow()
    {
        switch (weaponEffect.effectType)
        {
            case WEAPON_EFFECT.FREEZE:
                return freezeArrow;
            case WEAPON_EFFECT.LIGHTING:
                return lightningArrow;
            case WEAPON_EFFECT.POISON:
                return poisonedArrow;
            default:
                return normalArrow;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
