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

	public enum TravelPath
	{
		BasicLeftToRight,
		BasicRightToLeft,
		LeftToRight2Down1Up,
		RightToLeft2Down1Up
	}

	public TravelPath TravelRoute;

	public bool AimedShot = true;

	float XMax = 0;
	float XMin = 0;

	bool MovingRight = true;

	bool MoveDownStep = false;

	bool MoveUpStep = false;

	float Width, Height;

	public int SpawnCount = 0;

	void Start()
	{
		mScoreKeeper = GameObject.Find("Score").GetComponent<ScoreKeeper>();

		float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
		XMin = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distanceToCamera)).x;
		XMax = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, distanceToCamera)).x;

		Width = gameObject.GetComponent<Collider2D>().bounds.size.x;
		Height = gameObject.GetComponent<Collider2D>().bounds.size.y;
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

		HandleTravelRoute();

		if(MovingRight)	transform.position += Vector3.right * 2 * Time.deltaTime;
		else transform.position += Vector3.left * 2 * Time.deltaTime;
	}

	void HandleTravelRoute()
	{
		switch(TravelRoute)
		{
		case TravelPath.BasicLeftToRight:
			MovingRight = true;
			MoveBasic();
			break;
		case TravelPath.BasicRightToLeft:
			MovingRight = false;
			MoveBasic();
			break;
		case TravelPath.RightToLeft2Down1Up:
			MoveRightToLeft2down1Up();
			break;
		case TravelPath.LeftToRight2Down1Up:
			MoveLeftToRight2Down1Up();
			break;
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

	void MoveBasic()
	{
		if(transform.position.x < XMin + Width / 2)
		{
			MovingRight = true;
			MoveDownStep = true;
		} 
		else if(transform.position.x > XMax - Width / 2)
		{
			MovingRight = false;
			MoveDownStep = true;
		}

		if(MoveDownStep)
		{
			transform.position += Vector3.down * Height;
			MoveDownStep = false;
		}
	}

	void MoveLeftToRight2Down1Up()
	{
		if(transform.position.x < XMin + Width / 2)
		{
			MovingRight = true;
			MoveUpStep = true;
		} 
		else if(transform.position.x > XMax - Width / 2)
		{
			MovingRight = false;
			MoveDownStep = true;
		}

		if(MoveDownStep)
		{
			transform.position += Vector3.down * (Height * 2);
			MoveDownStep = false;
		}

		if(MoveUpStep)
		{
			transform.position += Vector3.up * Height;
			MoveUpStep = false;
		}
	}

	void MoveRightToLeft2down1Up()
	{
		if(transform.position.x < XMin + Width / 2)
		{
			MovingRight = true;
			MoveUpStep = true;
		} 
		else if(transform.position.x > XMax - Width / 2)
		{
			MovingRight = false;
			MoveDownStep = true;
		}

		if(MoveDownStep)
		{
			transform.position += Vector3.down * (Height * 2);
			MoveDownStep = false;
		}

		if(MoveUpStep)
		{
			transform.position += Vector3.up * Height;
			MoveUpStep = false;
		}
	}

}
	