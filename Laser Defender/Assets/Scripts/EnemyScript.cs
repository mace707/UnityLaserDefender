using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour 
{
	[SerializeField]
	public float Health 			= 150;
	public float ProjectileSpeed 	= 50.0f;
	public float ShotsPerSecond 	= 0.5f;

	public GameObject Projectile;

	private ScoreKeeper mScoreKeeper;

	public AudioClip FireSound;
	public AudioClip DeathSound;

	public GameObject Explosion;

	public GameObject[] ItemDrops; 

	public bool AimedShot = true;

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

		float playerXPos = GameObject.Find("Player").transform.position.x;

		float xShootDirrection = AimedShot ? playerXPos - transform.position.x : 0;

		GameObject beam = (GameObject)Instantiate (Projectile, transform.position, Quaternion.identity);
		beam.GetComponent<Rigidbody2D>().velocity = new Vector2(xShootDirrection, -ProjectileSpeed);
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
		Instantiate(Explosion, transform.position, Quaternion.identity);
		int itemDropIndex = Random.Range(0, 11);
		if(itemDropIndex < ItemDrops.Length)
		{
			GameObject itemDrop = Instantiate(ItemDrops[itemDropIndex], transform.position, Quaternion.identity);
			itemDrop.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.position.x, -5);
		}

		AudioSource.PlayClipAtPoint(DeathSound, transform.position, 50f);
		mScoreKeeper.SetScore(175);
		Destroy(gameObject);
	}

}
