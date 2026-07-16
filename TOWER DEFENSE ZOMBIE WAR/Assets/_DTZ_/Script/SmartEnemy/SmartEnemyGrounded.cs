using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartEnemyGrounded : Enemy, ICanTakeDamage {
    
	public bool isDead{ get; set; }
	[HideInInspector]
	public Vector3 velocity;
	bool allowCheckAttack = true;
	EnemyMeleeAttack meleeAttack;
	EnemyRangeAttack rangeAttack;
	SpawnItemHelper spawnItem;
    
	public override void Start ()
	{
		base.Start ();
		isPlaying = true;

		meleeAttack = GetComponent<EnemyMeleeAttack> ();
		rangeAttack = GetComponent<EnemyRangeAttack>();

		if (meleeAttack && meleeAttack.MeleeObj)
			meleeAttack.MeleeObj.SetActive (true);

		spawnItem = GetComponent<SpawnItemHelper> ();
    }
    
	public override void Update ()
	{
		base.Update ();
		HandleAnimation ();

        if (enemyState != ENEMYSTATE.WALK)
        {
			velocity = Vector2.zero;
            return;
        }
    }

	float velocityXSmoothing;

	public virtual void LateUpdate(){
		if (!isPlaying)
        {
            velocity = Vector2.zero;
            return;
        }

		float targetMoveSpeed = moveSpeed;

        if (enemyState != ENEMYSTATE.WALK || enemyEffect == ENEMYEFFECT.FREEZE)
			targetMoveSpeed = 0;

        if (isStopping || isStunning)
			targetMoveSpeed = 0;

		velocity.x = 0;
        velocity.y = Mathf.SmoothDamp (velocity.y, targetMoveSpeed, ref velocityXSmoothing, 0.1f);

		transform.Translate(new Vector3(0, -velocity.y * Time.deltaTime, 0));

        if (isPlaying && allowCheckAttack && enemyEffect != ENEMYEFFECT.FREEZE)
        {
            CheckAttack();
        }
	}
    
    public override void Stun(float time = 2)
    {
        base.Stun(time);
        StartCoroutine(StunCo(time));
    }

    IEnumerator StunCo(float time)
    {
        AnimSetTrigger("stun");
        isStunning = true;
        yield return new WaitForSeconds(time);
        isStunning = false;
    }

    public override void StunManuallyOn()
    {
        AnimSetTrigger("stun");
        isStunning = true;
    }

    public override void StunManuallyOff()
    {
        isStunning = false;
    }

	void CheckAttack()
	{
		if (meleeAttack && meleeAttack.AllowAction())
		{
			if (meleeAttack.CheckPlayer())
			{
				SetEnemyState(ENEMYSTATE.ATTACK);
				AnimSetTrigger("melee");
				meleeAttack.Check4Hit();
			}
			else if (!meleeAttack.isAttacking && enemyState == ENEMYSTATE.ATTACK)
			{
				SetEnemyState(ENEMYSTATE.WALK);
			}
		}

		if (rangeAttack)
		{
			if (rangeAttack.CheckAttack())
			{
				SetEnemyState(ENEMYSTATE.ATTACK);
				if (rangeAttack.Shoot())
					AnimSetTrigger("shoot");
			}
		}
	}

	void HandleAnimation(){
		AnimSetFloat ("speed", Mathf.Abs (velocity.y));
        AnimSetBool("isStunning", isStunning);
    }

	public void SetForce(float x, float y){
		velocity = new Vector3 (x, y, 0);
	}

	public override void Die ()
	{
		if (isDead)
			return;

		base.Die ();

		isDead = true;

		CancelInvoke ();

		var cols= GetComponents<Collider2D>();
		foreach (var col in cols)
			col.enabled = false;

		if (spawnItem && spawnItem.spawnWhenDie)
			spawnItem.Spawn ();
        
        AnimSetBool("isDead", true);

		if (enemyEffect == ENEMYEFFECT.EXPLOSION || dieBehavior == DIEBEHAVIOR.DESTROY) {
			gameObject.SetActive (false);
			return;
		}
        
		StopAllCoroutines ();
            StartCoroutine(DisableEnemy(AnimationHelper.getAnimationLength(anim, "Die") + 2f));
    }

	public override void Hit (Vector2 force, bool pushBack = false, bool knockDownRagdoll = false, bool shock = false)
	{
		if (!isPlaying || isStunning)
			return;

		base.Hit (force, pushBack, knockDownRagdoll, shock);
		if (isDead)
			return;

        AnimSetTrigger("hit");

        if (spawnItem && spawnItem.spawnWhenHit)
			spawnItem.Spawn ();

		if (pushBack)
            StartCoroutine(PushBack(force));
        else if (shock)
            StartCoroutine(Shock());
    }

	public override void KnockBack (Vector2 force, float stunningTime = 0)
	{
		base.KnockBack (force);

        SetForce(force.x, force.y);
    }

	public IEnumerator PushBack(Vector2 force){
		SetForce (force.x, force.y);

		if (isDead) {
			Die ();
			yield break;
		}
	}

    public IEnumerator Shock()
    {
        if (isDead)
        {
            Die();
            yield break;
        }
    }

    IEnumerator DisableEnemy(float delay){
		yield return new WaitForSeconds (delay);
        if (disableFX)
            SpawnSystemHelper.GetNextObject(disableFX, true).transform.position = spawnDisableFX != null ? spawnDisableFX.position : transform.position;
        gameObject.SetActive (false);
	}
}
