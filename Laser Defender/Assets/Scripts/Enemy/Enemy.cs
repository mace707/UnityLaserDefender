using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour 
{
	public int SpawnCount = 0;

	// Serialized Public
	[SerializeField]
	public float Health = 0;
	[SerializeField]
	public float ProjectileSpeed = 0;
	[SerializeField]
	public float ShotsPerSecond = 0;

	[SerializeField]
	public int Score = 100;

	[SerializeField]
	public AudioClip FireSound;
	[SerializeField]
	public AudioClip DeathSound;

	[SerializeField]
	public GameObject Explosion;

	[SerializeField]
	public GameObject[] ItemDrops; 

	[SerializeField]
	public float[] ItemDropProbabilities; 

	[SerializeField]
	public float Damage;

	private ScoreText mScoreText;

	EnemyCountText mEnemyCountText;

	public GameObject Projectile;

	public bool AimedShot = false;

	public bool Invincible = false;

	void Start()
	{
		mScoreText = GameObject.Find(StringConstants.TEXTScore).GetComponent<ScoreText>();
		mEnemyCountText = GameObject.Find(StringConstants.TEXTSpawnCount).GetComponent<EnemyCountText>();
	}

	void Update()
	{
		if(GlobalConstants.FreezeAllNoTimeScale)
			return;

		FireProjectile();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		HandleCollision(col);
	}

	void FireProjectile()
	{
		float probability = ShotsPerSecond * Time.deltaTime;
		if(Random.value < probability)
		{
			GameObject projectile = (GameObject)Instantiate(Projectile, transform.position, Quaternion.identity);
			projectile.GetComponent<Projectile>().SetDamage(Damage);

			if(AimedShot)
			{
				Vector3 targetPosition = GameObject.Find(StringConstants.GOPlayer).transform.position;
				Vector3 relativePos = targetPosition - transform.position;
				projectile.GetComponent<Rigidbody2D>().velocity = relativePos.normalized * ProjectileSpeed;
			}
			else
			{
				projectile.GetComponent<Rigidbody2D>().velocity = Vector3.down * ProjectileSpeed;
			}
			
			AudioSource.PlayClipAtPoint(FireSound, transform.position);
		}
	}

	void Die()
	{
		mEnemyCountText.ChangeCount();
		mScoreText.SetScore(Score);

		Instantiate(Explosion, transform.position, Quaternion.identity);
		int itemDropIndex = Random.Range(0, ItemDrops.Length);

		if(Random.value < ItemDropProbabilities[itemDropIndex])
		{
			GameObject itemDrop = Instantiate(ItemDrops[itemDropIndex], transform.position, Quaternion.identity);
			itemDrop.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.position.x, -5);
		}

		AudioSource.PlayClipAtPoint(DeathSound, transform.position, 50f);
		Destroy(gameObject);
	}

	void HandleCollision(Collider2D col)
	{
		Projectile laser = col.gameObject.GetComponent<Projectile> ();
		if(laser && !Invincible)
		{
			laser.Hit();
			Health -= laser.GetDamage();
			if(Health <= 0)
				Die();
		}
	}
}
	