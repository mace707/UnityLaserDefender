using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour 
{
	// Public
	public enum TravelPath
	{
		LeftToRight,
		RightToLeft,
		SidesToCenter,
		LeftToCenter,
		RightToCenter,
	}
	public TravelPath Path;

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
	public bool AimedShot = true;

	[SerializeField]
	public AudioClip FireSound;
	[SerializeField]
	public AudioClip DeathSound;

	[SerializeField]
	public GameObject Projectile;

	[SerializeField]
	public GameObject Explosion;

	[SerializeField]
	public GameObject[] ItemDrops; 

	[SerializeField]
	public float[] ItemDropProbabilities; 

	[SerializeField]
	public float Damage;

	// private
	public bool MovingRight = true;
	private bool MovingDown = false;
	private bool MovingUp = false;

	private float Width = 0;
	private float Height = 0;

	private ScoreKeeper mScoreKeeper;

	private float ScreenMaxX = 0;
	private float ScreenMinX = 0;
	private float ScreenMidX = 0;

	private Vector3 StartVerticalVec = new Vector3(0,0,0);

	SpawnCounter mSpawnCounter;

	void Start()
	{
		mScoreKeeper = GameObject.Find(StringConstants.TEXTScore).GetComponent<ScoreKeeper>();
		mSpawnCounter = GameObject.Find(StringConstants.TEXTSpawnCount).GetComponent<SpawnCounter>();

		float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
		ScreenMinX = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distanceToCamera)).x;
		ScreenMaxX = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, distanceToCamera)).x;
		ScreenMidX = Camera.main.ViewportToWorldPoint (new Vector3 (0.5f, 0, distanceToCamera)).x;

		Width = gameObject.GetComponent<Collider2D>().bounds.size.x;
		Height = gameObject.GetComponent<Collider2D>().bounds.size.y;

		if(transform.position.x < ScreenMidX)	MovingRight = true;
		else MovingRight = false;
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		HandleCollision(col);
	}

	void FireProjectile()
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

	// Update is called once per frame
	void Update()
	{
		float probability = ShotsPerSecond * Time.deltaTime;
		if(Random.value < probability)
			FireProjectile();
	}

	// We need this in a fixed update otherwise some game objects jump randomly.
	void FixedUpdate()
	{
		HandleMovement();
	}

	void HandleMovement()
	{
		switch(Path)
		{
		case TravelPath.LeftToRight:
		case TravelPath.RightToLeft:
			Move(ScreenMinX, ScreenMaxX);
			break;
		case TravelPath.LeftToCenter:
			Move(ScreenMinX, ScreenMidX);
			break;
		case TravelPath.RightToCenter:
			Move(ScreenMidX, ScreenMaxX);
			break;
		case TravelPath.SidesToCenter:
			break;
		default:
			break;
		}
	}

	void Move(float minX, float maxX)
	{
		if(!MovingUp && !MovingDown) MoveHorizontal(minX, maxX);
		else if (MovingDown) MoveDown();
	}

	void MoveHorizontal(float xMin, float xMax)
	{
		if(transform.position.x < xMin + Width / 2)
		{
			MovingRight = true;
			MovingDown = true;
		}
		else if(transform.position.x > xMax - Width / 2)
		{
			MovingRight = false;
			MovingDown = true;
		}

		if(MovingRight)	transform.position += Vector3.right * 2 * Time.deltaTime;
		else			transform.position += Vector3.left * 2 * Time.deltaTime;
	}

	void MoveDown()
	{
		Vector3 vertVal = Vector3.down * Time.deltaTime * 2;
		transform.position += vertVal;
		StartVerticalVec += vertVal;

		if(Mathf.Abs(StartVerticalVec.y) >= Height)
		{
			StartVerticalVec = new Vector3(0, 0, 0);
			MovingDown = false;
		}
	}

	void Die()
	{
		mSpawnCounter.ChangeCount();
		Instantiate(Explosion, transform.position, Quaternion.identity);
		int itemDropIndex = Random.Range(0, ItemDrops.Length);

		if(Random.value < ItemDropProbabilities[itemDropIndex])
		{
			GameObject itemDrop = Instantiate(ItemDrops[itemDropIndex], transform.position, Quaternion.identity);
			itemDrop.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.position.x, -5);
		}

		AudioSource.PlayClipAtPoint(DeathSound, transform.position, 50f);
		mScoreKeeper.SetScore(Score);
		Destroy(gameObject);
	}

	void HandleCollision(Collider2D col)
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
}
	