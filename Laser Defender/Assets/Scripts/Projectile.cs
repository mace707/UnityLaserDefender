using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour 
{
	public float Damage = 100f;

	public bool IsIceProjectile = false;
	public bool IsExplodingProjectile = false;

	public float SlowDownFactor = 1;

	public GameObject ExplodingProjectile;

	public float GetDamage()
	{
		return Damage;
	}

	public void Hit()
	{
		if(IsExplodingProjectile)
			Instantiate(ExplodingProjectile, transform.position, Quaternion.identity);

		Destroy(gameObject);
	}
}
