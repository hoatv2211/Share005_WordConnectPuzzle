using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENEMYSTATE {
	IDLE,
	ATTACK,
    WALK,
	HIT,
	DEATH
}

public enum ENEMYEFFECT {
	NONE,
	FREEZE,
    POISON,
	EXPLOSION
}

public enum DIEBEHAVIOR {
	NORMAL,
	DESTROY,
	BLOWUP
}

public class Enemy : MonoBehaviour,ICanTakeDamage {
    [Header("Setup")]
	public float walkSpeed = 3;
    [HideInInspector] public DIEBEHAVIOR dieBehavior = DIEBEHAVIOR.NORMAL;
    [ReadOnly] public float multipleSpeed = 1;
    
    [Header("HEALTH")]
	[Range(0,5000)]
	public int health = 100;
    public GameObject hitFX;
    public GameObject disableFX;
    public Transform spawnDisableFX;
    public GameObject bloodPuddleFX;
    public GameObject[] explosionFX;
    public Vector2 randomHitPoint = new Vector2 (0.2f, 0.2f);
    public Vector2 randomBloodPuddlePoint = new Vector2 (0.5f, 0.25f);
    public Vector2 healthBarOffset = new Vector2(0, 1.5f);
    
    [ReadOnly] public ENEMYSTATE enemyState = ENEMYSTATE.IDLE;
	protected ENEMYEFFECT enemyEffect;
	[Space]
    [Header("*** LOCAL FX ***")]
    public GameObject freezeFX;
    public GameObject poisonFX;
    public GameObject lightningFX;

    //[Header("Freeze Option")]
    [HideInInspector] public bool canBeFreeze = true;

    //[Header("Poison Option")]
    [HideInInspector] public bool canBePoison = true;

   
    [Tooltip("% reduce poison damage")]
    [Range(0,90)]
    [HideInInspector] public float resistPoisonPercent = 10;
    [Range(0, 1)]
    [HideInInspector] public float poisonSlowSpeed = 0.3f;
    [ReadOnly] public float damagePoisonPerSecond;
    
    [Header("Sound")]
    [Range(0,1)]
    public float soundHitVol = 0.5f;
	public AudioClip[] soundHit;
    [Range(0, 1)]
    public float soundDieVol = 0.5f;
    public AudioClip[] soundDie;
    [Range(0, 1)]
    public float soundDieBlowVol = 0.5f;
    public AudioClip[] soundDieBlow;
    [Range(0, 1)]
    public float soundAggressiveVolume = 0.5f;
    public AudioClip[] soundAggressive;
    public float soundAggressiveDelayMin = 2;
    public float soundAggressiveDelayMax = 8;
    public float chanceToPlayAggressive = 0.5f;

    [ReadOnly] public int currentHealth;
	Vector2 hitPos;
	public bool isPlaying{ get; set; }
	public bool isStopping { get; set; }
    [ReadOnly] public bool isStunning = false;

    protected HealthBarEnemyNew healthBar;
	protected Animator anim;
	protected float moveSpeed;

    public bool isFacingRight(){
        return transform.rotation.eulerAngles.y == 180 ? true : false;
    }

    protected virtual void OnEnable()
    {
        isPlaying = true;
    }

    public virtual void Start()
    {
        currentHealth = health;
        moveSpeed = walkSpeed;

        var healthBarObj = (HealthBarEnemyNew)Resources.Load("HealthBar", typeof(HealthBarEnemyNew));
        healthBar = (HealthBarEnemyNew)Instantiate(healthBarObj, healthBarOffset, Quaternion.identity);

        healthBar.Init(transform, (Vector3)healthBarOffset);

        anim = GetComponent<Animator>();

        enemyState = ENEMYSTATE.WALK;

        StartCoroutine(PlaySoundAggressiveCo());

        freezeFX.SetActive(false);
        poisonFX.SetActive(false);
        lightningFX.SetActive(false);
    }

    IEnumerator PlaySoundAggressiveCo()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(soundAggressiveDelayMin, soundAggressiveDelayMax));
            if (Random.Range(0f, 1f) < chanceToPlayAggressive)
                SoundManager.PlaySfx(soundAggressive, soundAggressiveVolume);
        }
    }


    public void AnimSetTrigger(string name){
		anim.SetTrigger (name);
	}

	public void AnimSetBool(string name, bool value){
		anim.SetBool (name, value);
	}

	public void AnimSetFloat(string name, float value){
		anim.SetFloat (name, value);
	}

	public void SetEnemyState(ENEMYSTATE state){
		enemyState = state;
	}

	public void SetEnemyEffect(ENEMYEFFECT effect){
		enemyEffect = effect;
	}

	public virtual void Update(){
		healthBar.transform.localScale = new Vector2 (transform.localScale.x > 0 ? Mathf.Abs (healthBar.transform.localScale.x) : -Mathf.Abs (healthBar.transform.localScale.x), healthBar.transform.localScale.y);
	}

	public virtual void FixedUpdate(){

	}

	public virtual void Hit(Vector2 force = default(Vector2), bool pushBack = false, bool knockDownRagdoll = false, bool shock = false)
    {
		SoundManager.PlaySfx (soundHit, soundHitVol);
	}

	public virtual void KnockBack(Vector2 force, float stunningTime = 0){
		
	}

    public virtual void Die(){
		isPlaying = false;
		SetEnemyState (ENEMYSTATE.DEATH);
        StopAllCoroutines();

        poisonFX.SetActive(false);
        freezeFX.SetActive(false);
        lightningFX.SetActive(false);

        if (GetComponent<GiveCoinWhenDie>())
        {
            GetComponent<GiveCoinWhenDie>().GiveCoin();
        }

		if (enemyEffect == ENEMYEFFECT.EXPLOSION) {
			if (bloodPuddleFX) {
				for (int i = 0; i < Random.Range (2, 5); i++) {
					SpawnSystemHelper.GetNextObject (bloodPuddleFX, true).transform.position = 
						(Vector2)transform.position + new Vector2 (Random.Range (-(randomBloodPuddlePoint.x * 2), randomBloodPuddlePoint.x * 2), Random.Range (-(2 * randomBloodPuddlePoint.y), 2 * randomBloodPuddlePoint.y));
				}
			}
			if (explosionFX.Length>0) {
				for (int i = 0; i < Random.Range (1, 3); i++) {
					var obj = SpawnSystemHelper.GetNextObject (explosionFX[Random.Range(0,explosionFX.Length)], false);
					obj.transform.position = transform.position;
					obj.SetActive (true);
				}
			}

			SoundManager.PlaySfx (soundDieBlow, soundDieBlowVol);
		} else
			SoundManager.PlaySfx (soundDie, soundDieVol);
        
    }

	private void CheckDamagePerFrame(float _damage){
		if (enemyState == ENEMYSTATE.DEATH)
			return;

		currentHealth -= (int) _damage;
		if (healthBar)
			healthBar.UpdateValue (currentHealth / (float) health);
		
		if (currentHealth <= 0)
			Die ();
	}

	#region ICanFreeze implementation
	public virtual void Freeze (float time, GameObject instigator)
	{
		if (enemyEffect == ENEMYEFFECT.FREEZE)
			return;

		if (canBeFreeze) {
			enemyEffect = ENEMYEFFECT.FREEZE;
            freezeFX.SetActive(true);
            StartCoroutine (UnFreezeCo (time));
		}
	}

	IEnumerator UnFreezeCo(float time)
    {
        AnimSetBool("isFreezing", true);

        if (enemyEffect != ENEMYEFFECT.FREEZE)
			yield break;
        
		yield return new WaitForSeconds(time);
		UnFreeze ();
	}

	void UnFreeze(){
		if (enemyEffect != ENEMYEFFECT.FREEZE)
			return;
        freezeFX.SetActive(false);
        enemyEffect = ENEMYEFFECT.NONE;
        AnimSetBool("isFreezing", false);
    }

    #endregion

    #region LIGHTING
    public virtual void Lighting(float time, GameObject instigator)
    {
        if (enemyEffect == ENEMYEFFECT.FREEZE)
            return;
        
            enemyEffect = ENEMYEFFECT.FREEZE;
            StartCoroutine(UnLightingCo(time));
    }

    IEnumerator UnLightingCo(float time)
    {
        AnimSetBool("isLighting", true);
        lightningFX.SetActive(true);

        if (enemyEffect != ENEMYEFFECT.FREEZE)
            yield break;

        yield return new WaitForSeconds(time);
        UnLighting();
    }

    void UnLighting()
    {
        if (enemyEffect != ENEMYEFFECT.FREEZE)
            return;

        lightningFX.SetActive(false);
        enemyEffect = ENEMYEFFECT.NONE;
        AnimSetBool("isLighting", false);
    }

    #endregion

    #region ICanPoison implementation
    public virtual void Poison(float damage, float time, GameObject instigator)
    {
        if (enemyEffect == ENEMYEFFECT.POISON)
            return;

        if (enemyEffect == ENEMYEFFECT.FREEZE)
        {
            UnFreeze();
        }

        if (canBePoison)
        {
            damagePoisonPerSecond = damage;
            enemyEffect = ENEMYEFFECT.POISON;

            StartCoroutine(PoisonCo(time));
        }
    }

    IEnumerator PoisonCo(float time)
    {
        AnimSetBool("isPoisoning", true);
        poisonFX.SetActive(true);
        multipleSpeed = poisonSlowSpeed;
        if (enemyEffect != ENEMYEFFECT.POISON)
            yield break;

        int wait = (int) time;

        while (wait > 0)
        {
            yield return new WaitForSeconds(1);
            int _damage = (int)(damagePoisonPerSecond * Random.Range(90 - resistPoisonPercent, 100f - resistPoisonPercent) * 0.01f);
            currentHealth -= _damage;
            if (healthBar)
                healthBar.UpdateValue(currentHealth / (float)health);

            FloatingTextManager.Instance.ShowText("" + (int) _damage, healthBarOffset, Color.red, transform.position);

            if (currentHealth <= 0)
            {
                PoisonEnd();
                Die();
                yield break;
            }

            wait -= 1;
        }


        if (enemyState == ENEMYSTATE.DEATH)
        {
            gameObject.SetActive(false);
        }

        PoisonEnd();
    }

    void PoisonEnd()
    {
        if (enemyEffect != ENEMYEFFECT.POISON)
            return;

        AnimSetBool("isPoisoning", false);
        multipleSpeed = 1;
        poisonFX.SetActive(false);
        enemyEffect = ENEMYEFFECT.NONE;
    }
    #endregion

    #region Stun
    public virtual void Stun(float time = 2)
    {

    }

    public virtual void StunManuallyOn()
    {

    }

    public virtual void StunManuallyOff()
    {

    }
    #endregion

    #region TakeDamage implementation
    protected BODYPART _bodyPart;
    protected Vector2 _bodyPartForce;
    protected float _damage;
    public void TakeDamage(float damage, Vector2 force, Vector2 hitPoint, GameObject instigator, BODYPART bodyPart = BODYPART.NONE, WeaponEffect weaponEffect = null, WEAPON_EFFECT forceEffect = WEAPON_EFFECT.NORMAL)
    {
        if (enemyState == ENEMYSTATE.DEATH)
            return;

        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;

        if (isStopping)
            return;
        _bodyPart = bodyPart;
        _bodyPartForce = force;
        _damage = damage;
        hitPos = hitPoint;
        bool isExplosion = false;

        currentHealth -= (int)damage;
        FloatingTextManager.Instance.ShowText("" + (int) damage, healthBarOffset, Color.red, transform.position);

        if (hitFX)
            SpawnSystemHelper.GetNextObject(hitFX, true).transform.position =
                hitPos + new Vector2(Random.Range(-randomHitPoint.x, randomHitPoint.x), Random.Range(-randomHitPoint.y, randomHitPoint.y));
        if (bloodPuddleFX)
            SpawnSystemHelper.GetNextObject(bloodPuddleFX, true).transform.position =
                (Vector2)transform.position + new Vector2(Random.Range(-randomBloodPuddlePoint.x, randomBloodPuddlePoint.x), Random.Range(-randomBloodPuddlePoint.y, randomBloodPuddlePoint.y));


        if (healthBar)
            healthBar.UpdateValue(currentHealth / (float)health);
        if (currentHealth <= 0)
        {
            if (isExplosion || dieBehavior == DIEBEHAVIOR.BLOWUP)
            {
                SetEnemyEffect(ENEMYEFFECT.EXPLOSION);
            }

            Die();
        }
        else
        {
            if (weaponEffect != null)
            {
                switch (forceEffect != WEAPON_EFFECT.NORMAL ? forceEffect : weaponEffect.effectType)
                {
                    case WEAPON_EFFECT.POISON:
                        Poison(weaponEffect.poisonDamagePerSec, weaponEffect.poisonTime, instigator);
                        return;
                    case WEAPON_EFFECT.FREEZE:
                        Freeze(weaponEffect.freezeTime, instigator);
                        return;
                    case WEAPON_EFFECT.LIGHTING:
                        Lighting(0.5f, instigator);
                        break;
                    default:
                        break;
                }
            }

            Hit(force);
        }
    }
    #endregion
}
