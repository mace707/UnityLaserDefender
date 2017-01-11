using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour 
{
	[SerializeField]
	public float Health 			= 150;
	public float ProjectileSpeed 	= 50.0f;
	public float FiringRate 		= 1.2f;
	public float ShotsPerSecond 	= 0.5f;

	public GameObject Projectile;

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

	void FireProjectile()
	{
		GameObject beam = (GameObject)Instantiate (Projectile, transform.position, Quaternion.identity);
		beam.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -ProjectileSpeed);
	}

	// Update is called once per frame
	void Update () 
	{
		float probability = ShotsPerSecond * Time.deltaTime;
		if (Random.value < probability)
		{
			FireProjectile();
		}
	}

}
