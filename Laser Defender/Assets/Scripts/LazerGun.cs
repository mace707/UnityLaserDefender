using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
	void Fire(GameObject target);
	void Fire(Vector3 startPos, Vector3 dirrection);
}

public class LazerGun : IWeapon
{
	private GameObject ProjectileGO;

	public LazerGun(GameObject projectileGO)
	{
		ProjectileGO = projectileGO;
	}

	public void Fire(GameObject target)
	{
		
	}

	public void Fire(Vector3 insPos, Vector3 dirrection)
	{
		GameObject projectileGO = GameObject.Instantiate(ProjectileGO, insPos, Quaternion.identity);
		projectileGO.GetComponent<Projectile>().SetDamage(100);
		projectileGO.GetComponent<Rigidbody2D>().velocity = dirrection;

	}
}
