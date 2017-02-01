﻿using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour 
{
	// Public
	public enum TravelPath
	{
		LeftToRight,
		RightToLeft,
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

	// private
	private bool MovingRight = true;
	private bool MovingDown = false;
	private bool MovingUp = false;

	private float Width = 0;
	private float Height = 0;

	private ScoreKeeper mScoreKeeper;

	private float ScreenMaxX = 0;
	private float ScreenMinX = 0;

	private Vector3 StartVerticalVec = new Vector3(0,0,0);

	void Start()
	{
		mScoreKeeper = GameObject.Find("Score").GetComponent<ScoreKeeper>();

		float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
		ScreenMinX = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distanceToCamera)).x;
		ScreenMaxX = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, distanceToCamera)).x;

		Width = gameObject.GetComponent<Collider2D>().bounds.size.x;
		Height = gameObject.GetComponent<Collider2D>().bounds.size.y;

		switch(Path)
		{
		case TravelPath.LeftToRight:
			MovingRight = true;
			break;
		case TravelPath.RightToLeft:
			MovingRight = false;
			break;
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		HandleCollision(col);
	}

	void FireProjectile()
	{
		Vector3 targetPosition = GameObject.Find("Player").transform.position;
		float targetDirrection = AimedShot ? targetPosition.x - transform.position.x : 0;

		GameObject projectile = (GameObject)Instantiate (Projectile, transform.position, Quaternion.identity);
		projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(targetDirrection, -ProjectileSpeed);

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
			MoveBasic();
			break;
		}
	}

	void MoveBasic()
	{
		if(!MovingUp && !MovingDown) MoveHorizontal();
		else if (MovingDown) MoveDown();
	}

	void MoveHorizontal()
	{
		if(transform.position.x < ScreenMinX + Width / 2)
		{
			MovingRight = true;
			MovingDown = true;
		}
		else if(transform.position.x > ScreenMaxX - Width / 2)
		{
			MovingRight = false;
			MovingDown = true;
		}

		if(MovingRight)
			transform.position += Vector3.right * 2 * Time.deltaTime;
		else
			transform.position += Vector3.left * 2 * Time.deltaTime;
	}

	void MoveDown()
	{
		Debug.Log("Entered");
		Vector3 vertVal = Vector3.down * Time.deltaTime * 2;
		transform.position += vertVal;
		StartVerticalVec += vertVal;

		if(Mathf.Abs(StartVerticalVec.y) >= Height)
		{
			StartVerticalVec = new Vector3(0, 0, 0);
			MovingDown = false;
			Debug.Log("Exit");
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
	