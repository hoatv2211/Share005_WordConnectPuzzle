using UnityEngine;
using System.Collections;

public class SimpleProjectile : Projectile, ICanTakeDamage
{
    public int Damage = 30;
	public GameObject DestroyEffect;
    public float timeToLive = 3;
	public AudioClip soundHitEnemy;
	[Range(0,1)]
	public float soundHitEnemyVolume = 0.5f;

	[Header("*** GRENADE OPTIONAL ***")]
	public Grenade spawnGrenade;
    float timeToLiveCounter = 0;

    void OnEnable()
    {
        timeToLiveCounter = timeToLive ;
    }

	public override void Update()
	{
		base.Update();

		if (isStop)
			return;
		
		if ((timeToLiveCounter -= Time.deltaTime) <= 0)
			DestroyProjectile ();

		transform.Translate((Direction + new Vector2(InitialVelocity.x, 0)) * Speed * Time.deltaTime, Space.World);
	}

	void DestroyProjectile(){
        if (DestroyEffect != null)
            SpawnSystemHelper.GetNextObject(DestroyEffect, true).transform.position = transform.position;

		if (spawnGrenade) {
			Instantiate(spawnGrenade.gameObject, transform.position, Quaternion.identity).GetComponent<Grenade>().Init(transform.position, (NewDamage == 0 ? Damage : NewDamage));
		}

        gameObject.SetActive(false) ;
	}


	public void TakeDamage(float damage, Vector2 force, Vector2 hitPoint, GameObject instigator, BODYPART bodyPart = BODYPART.NONE, WeaponEffect weaponEffect = null, WEAPON_EFFECT forceEffect = WEAPON_EFFECT.NORMAL)
    {
		DestroyProjectile ();
	}

	protected override void OnCollideOther (RaycastHit2D other)
	{
		DestroyProjectile ();
	}

	protected override void OnCollideTakeDamage (RaycastHit2D other, ICanTakeDamage takedamage)
	{
		takedamage.TakeDamage ((NewDamage == 0 ? Damage : NewDamage), Vector2.zero, transform.position, Owner, BODYPART.NONE,weaponEffect);
		SoundManager.PlaySfx (soundHitEnemy, soundHitEnemyVolume);
		DestroyProjectile ();
	}

	bool isStop = false;
}

