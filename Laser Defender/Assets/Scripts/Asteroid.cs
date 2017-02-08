using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour 
{
	// Use this for initialization
	[SerializeField]
	public float HitPoints = 0;

	[SerializeField]
	public GameObject Explosion;

	void OnTriggerEnter2D(Collider2D col)
	{
		Projectile laser = col.gameObject.GetComponent<Projectile> ();
		if(laser)
		{
			laser.Hit();
			HitPoints -= laser.GetDamage();
			if(HitPoints <= 0)
				Die();
		}
	}

	void Die()
	{
		Instantiate(Explosion, this.transform.position, Quaternion.identity);
		Destroy(gameObject);
	}

}
