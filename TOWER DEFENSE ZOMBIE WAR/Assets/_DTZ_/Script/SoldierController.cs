using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierController : MonoBehaviour
{
    public UpgradedCharacterParameter upgradedCharacterID;
    [Header("+++ SHOOTING SETUP +++")]
    public int damage = 20;
    public float shootingRate = 1f;
    public Transform firePoint;
    public LayerMask layerAsTarget;
    public float checkRadius = 6;

    [Header("+++ BULLET +++")]
    public Projectile bullet;
    public float bulletSpeed = 35;

    [Header("*** SOUND ***")]
    public AudioClip soundShowup;
    public AudioClip soundShoot;
    [Range(0f, 1f)]
    public float soundShootVolume = 0.5f;

    float lastShootTime = -999;
    [HideInInspector] public GameObject target;
    [HideInInspector] public Animator anim;
    [HideInInspector] public float angle = 0;
    [HideInInspector] public int extraDamage;

    void Start()
    {
        SoundManager.PlaySfx(soundShowup);

        if (anim == null)
            anim = GetComponent<Animator>();

        //Do Get Upgrade
        if (upgradedCharacterID != null)
        {
            extraDamage = upgradedCharacterID.UpgradeRangeDamage;
        }
    }

    void Update()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;
        Shoot();    //call shooting overtime
    }

    void Shoot()
    {
        if ((Time.time - lastShootTime) > shootingRate)
        {
            if (CheckTarget())
            {
                StartCoroutine(ShootCo());
            }
        }
    }

    IEnumerator ShootCo()
    {
        lastShootTime = Time.time;

        anim.SetTrigger("shoot");

        angle = Vector2.Angle(Vector2.right, target.transform.position - transform.position);

        int fixedAngle = 0;     //check the angle to set the animation for the player
        if (angle < 45)
            fixedAngle = 30;
        else if (angle < 80)
            fixedAngle = 60;
        else if (angle < 110)
            fixedAngle = 90;
        else if (angle < 145)
            fixedAngle = 120;
        else if (angle < 175)
            fixedAngle = 160;

        anim.SetInteger("angle", fixedAngle);
        SoundManager.PlaySfx(soundShoot, soundShootVolume);

        yield return new WaitForSeconds(0.1f);
        var projectile = SpawnSystemHelper.GetNextObject(bullet.gameObject, false).GetComponent<Projectile>();
        projectile.transform.position = firePoint.position;
        projectile.transform.right = target.transform.position - firePoint.position;

        Vector3 _dir;
        _dir = target.transform.position - firePoint.position;
        _dir.Normalize();

        projectile.Initialize(gameObject, _dir, Vector2.zero, bulletSpeed, false, (damage + extraDamage) * Random.Range(0.9f, 1.1f), 0);
        projectile.gameObject.SetActive(true);
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
