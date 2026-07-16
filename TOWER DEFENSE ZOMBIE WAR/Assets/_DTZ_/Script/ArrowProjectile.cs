using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : Projectile, ICanTakeDamage
{
    int Damage = 30;
    public GameObject DestroyEffect;
    public int pointToGivePlayer;
    public float timeToLive = 3;
    public AudioClip soundHitEnemy;
    [Range(0, 1)]
    public float soundHitEnemyVolume = 0.5f;
    public AudioClip soundHitNothing;
    [Range(0, 1)]
    public float soundHitNothingVolume = 0.5f;

    public GameObject ExplosionObj;
    float timeToLiveCounter = 0;

    bool isHit = false;
    bool criticalDamage = false;
    WeaponEffect arrowEffect;
    WEAPON_EFFECT forceEffect;
    float speed = 1;

    void OnEnable()
    {
        timeToLiveCounter = timeToLive;
        isHit = false;
    }

    public void Init(int damage, float _speed, bool isCritical, WeaponEffect _arrowEffect, WEAPON_EFFECT _forceEffect = WEAPON_EFFECT.NORMAL)
    {
        arrowEffect = _arrowEffect;
        forceEffect = _forceEffect;
        Damage = (int)( damage * Random.Range(0.9f, 1.1f));
        criticalDamage = isCritical;
        speed = _speed;
    }

    public Vector2 checkTargetDistanceOffset = new Vector2(-0.25f,0);
    public float checkTargetDistance = 1;

    // Update is called once per frame

    public override void Update()
    {
        base.Update();

        if (isHit)
            return;

        transform.Translate(0, speed * Time.deltaTime, 0, Space.Self);

        if ((timeToLiveCounter -= Time.deltaTime) <= 0)
        {
            //DestroyProjectile();
            gameObject.SetActive(false);
        }
    }

    IEnumerator DestroyProjectile(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        if (DestroyEffect != null)
            SpawnSystemHelper.GetNextObject(DestroyEffect, true).transform.position = transform.position;

        if (Explosion)
        {
            var bullet = Instantiate(ExplosionObj, transform.position, Quaternion.identity) as GameObject;
        }

        gameObject.SetActive(false);
    }

    public void TakeDamage(float damage, Vector2 force, Vector2 hitPoint, GameObject instigator, BODYPART bodyPart = BODYPART.NONE)
    {
        SoundManager.PlaySfx(soundHitNothing, soundHitNothingVolume);
        StartCoroutine(DestroyProjectile(1));
    }

    protected override void OnCollideOther(RaycastHit2D other)
    {
        SoundManager.PlaySfx(soundHitNothing, soundHitNothingVolume);
        StartCoroutine(DestroyProjectile(3));

    }

    protected override void OnCollideTakeDamage(RaycastHit2D other, ICanTakeDamage takedamage)
    {
        base.OnCollideTakeDamage(other, takedamage);
        if (criticalDamage)
            FloatingTextManager.Instance.ShowText("CRIT!", Vector2.up, Color.yellow, other.collider.gameObject.transform.position, 30);
        takedamage.TakeDamage(Damage, Vector2.zero, transform.position, Owner,BODYPART.NONE, arrowEffect, forceEffect);
        SoundManager.PlaySfx(soundHitEnemy, soundHitEnemyVolume);
        StartCoroutine(DestroyProjectile(0));
    }

    protected override void OnCollideTakeDamageBodyPart(RaycastHit2D other, ICanTakeDamageBodyPart takedamage)
    {
        
        base.OnCollideTakeDamageBodyPart(other, takedamage);
        WeaponEffect weaponEffect = new WeaponEffect();
        takedamage.TakeDamage(Damage, force, transform.position, Owner);
        StartCoroutine(DestroyProjectile(0));
    }

    public void TakeDamage(float damage, Vector2 force, Vector2 hitPoint, GameObject instigator, BODYPART bodyPart = BODYPART.NONE, WeaponEffect weaponEffect = null, WEAPON_EFFECT forceEffect = WEAPON_EFFECT.NORMAL)
    {
        StartCoroutine(DestroyProjectile(0));
    }
}
