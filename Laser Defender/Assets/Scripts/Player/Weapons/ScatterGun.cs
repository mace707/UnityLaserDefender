using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterGun : Weapon 
{
	public ScatterGun()
	{
		GOProjectile = GameObject.Find("AmmunitionCase").GetComponent<Ammunition>().GetLaser();
	}

	public override void Fire(GameObject target)
	{

	}

	public override void Fire(Vector3 insPos, Vector3 dirrection)
	{
		GameObject projectileGO = GameObject.Instantiate(GOProjectile, insPos, Quaternion.identity);
		projectileGO.GetComponent<Projectile>().SetDamage(100);
		projectileGO.GetComponent<Rigidbody2D>().velocity = dirrection;
		Debug.Log("Scatter Gun");
	}
}
