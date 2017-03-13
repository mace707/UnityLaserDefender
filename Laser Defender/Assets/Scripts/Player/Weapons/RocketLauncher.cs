using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : Weapon 
{
	public RocketLauncher()
	{
		GOProjectile = GameObject.Find("AmmunitionCase").GetComponent<Ammunition>().GetRocket();
	}

	public override void Fire(GameObject target)
	{

	}

	public override void Fire(Vector3 insPos, Vector3 dirrection)
	{
		GameObject projectileGO = GameObject.Instantiate(GOProjectile, insPos, Quaternion.identity);
		projectileGO.GetComponent<Projectile>().SetDamage(100);
		projectileGO.GetComponent<Rigidbody2D>().velocity = dirrection;
		Debug.Log("Rocket Launcher");
	}
}
