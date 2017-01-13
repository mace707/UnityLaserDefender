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

	private ScoreKeeper mScoreKeeper;

	public AudioClip FireSound;
	public AudioClip DeathSound;


	void Start()
	{
		mScoreKeeper = GameObject.Find("Score").GetComponent<ScoreKeeper>();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		Projectile laser = col.gameObject.GetComponent<Projectile> ();

		if(laser)
		{
			laser.Hit();
			Health -= laser.GetDamage();
			if(Health <= 0)
				Die();
		}
	}

	void FireProjectile()
	{
		AudioSource.PlayClipAtPoint(FireSound, transform.position);
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

	void Die()
	{
		AudioSource.PlayClipAtPoint(DeathSound, transform.position);
		mScoreKeeper.SetScore(175);
		Destroy(gameObject);
	}

}
