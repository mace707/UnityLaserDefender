using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour 
{
	[SerializeField]
	public float Health = 150;

	void OnTriggerEnter2D(Collider2D col)
	{
		Projectile laser = col.gameObject.GetComponent<Projectile> ();

		if(laser)
		{
			laser.Hit();
			Health -= laser.GetDamage();
			if(Health <= 0)
				Destroy(gameObject);
		}
	}
}
