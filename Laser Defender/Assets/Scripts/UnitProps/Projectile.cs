using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour 
{
	public float Damage = 100f;

	public enum DamageType
	{
		DamageTypeStandard,
		DamageTypeFrost,
		DamageTypeExplosion,
	}

	public DamageType ProjectileDamageType;

	public float SlowDownFactor = 1;

	public GameObject ExplodingProjectile;

	public void SetDamage(float damage)
	{
		Damage = damage;
	}

	public float GetDamage()
	{
		return Damage;
	}

	public void Hit()
	{
		if(ProjectileDamageType == DamageType.DamageTypeExplosion)
			Instantiate(ExplodingProjectile, transform.position, Quaternion.identity);

		Destroy(gameObject);
	}
}
