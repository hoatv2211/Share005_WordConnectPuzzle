using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangeAttack : MonoBehaviour
{
	public LayerMask targetAsLayer;
	public AudioClip soundShoot;
	[Range(0, 1)]
	public float soundShootVolume = 0.5f;

	public float bulletSpeed = 20;
	public Transform shootingPoint;
	public float damage = 30;
	public float checkTargetDistance = 5;
	public Projectile bullet;
	public float shootingRate = 1;
	float lastShoot = -999;


	Vector3 _target;
	// Update is called once per frame
	public bool CheckAttack()
	{
		if (Physics2D.Raycast(shootingPoint.position, Vector2.down, checkTargetDistance, targetAsLayer))
		{
			return true;
		}
		else
			return false;
	}

	public bool Shoot()
	{
		if ((Time.time - lastShoot) > shootingRate)
		{
			lastShoot = Time.time;
			SoundManager.PlaySfx(soundShoot, soundShootVolume);

			var projectile = SpawnSystemHelper.GetNextObject(bullet.gameObject, false).GetComponent<Projectile>();
			projectile.transform.position = shootingPoint.position;
			projectile.transform.right = Vector2.down;

			projectile.Initialize(gameObject, Vector2.down, Vector2.zero, bulletSpeed, false, damage * Random.Range(0.9f, 1.1f), 0);
			projectile.gameObject.SetActive(true);
			return true;
		}
		else
			return false;
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawRay(shootingPoint.position, Vector2.down * checkTargetDistance);
	}
}
