using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGun : Weapon 
{
	public LaserGun()
	{
		GOProjectile = GameObject.Find("AmmunitionCase").GetComponent<Ammunition>().GetLaser();
	}

	public override void Fire(GameObject target)
	{
		
	}

	public override void Fire(Vector3 insPos, Vector3 dirrection)
	{
		// Maybe use a function here to get the projectile from a Projectile factory...
		// Each projectile will behave differently so it makes sense to do this.
		GameObject projectileGO = GameObject.Instantiate(GOProjectile, insPos, Quaternion.identity);
		projectileGO.GetComponent<Projectile>().SetDamage(100);
		projectileGO.GetComponent<Rigidbody2D>().velocity = dirrection;
		Debug.Log("Laser Gun");
	}
}