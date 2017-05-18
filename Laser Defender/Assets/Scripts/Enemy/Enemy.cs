using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour 
{
	// Serialized Public
	[SerializeField]	public float Health = 0;
	[SerializeField]	public float ProjectileSpeed = 0;
	[SerializeField]	public float ShotsPerSecond = 0;
	[SerializeField]	public int Score = 0;
	[SerializeField]	public float Damage;
	[SerializeField] 	public GameObject Projectile;

	// Item Drops
	[SerializeField]	public GameObject[] ItemDrops; 
	[SerializeField]	public float[] ItemDropProbabilities; 

	// Sounds and animations
	[SerializeField]	public AudioClip FireSound;
	[SerializeField]	public AudioClip DeathSound;
	[SerializeField]	public AudioClip CloakSound;
	[SerializeField]	public GameObject Explosion;

	// Shield
	[SerializeField] 	private GameObject 	GOShield = null;
	[SerializeField] 	private float 		ShieldMinDuration = 0;
	[SerializeField] 	private float 		ShieldMaxDuration = 0;
	[SerializeField]	private bool 		HasShield = false;
	[SerializeField]	private float 		ShieldActivationProbability = 0;

	//Cloak
	[SerializeField]	private bool		HasCloak = false;
	[SerializeField]	private float 		CloakMinDuration = 0;
	[SerializeField]	private float 		CloakMaxDuration = 0;
	[SerializeField]	private float		CloakActivationProbability = 0;
						private bool 		CloakActive = false;
						private Color 		SpriteColor;

	private ScoreText mScoreText;
	public bool TrackingAmmunition = false;
	public bool Invincible = false;
	private LevelHandler ActiveLevelHandler;
	private GameObject INSTShield;
	private Animator mAnimator;

	void Start()
	{
		mAnimator = GetComponent<Animator>();
		mScoreText = GameObject.Find(StringConstants.TEXTScore).GetComponent<ScoreText>();
		ActiveLevelHandler = GameObject.Find("LevelHandler").GetComponent<LevelHandler>();
		if(HasShield)
			InvokeRepeating("ActivateShield", ShieldMinDuration, ShieldMaxDuration);
		if (HasCloak)
			InvokeRepeating("ActivateCloak", CloakMinDuration, CloakMaxDuration);
	}

	private void ActivateCloak()
	{
		DeactivateCloak();
		float randomProbability = Random.Range(GlobalConstants.ProbabilityMin, GlobalConstants.ProbabilityMax);
		if(randomProbability < CloakActivationProbability)
		{
			mAnimator.SetTrigger("EngageCloak");
			float cloakDuration = Random.Range (CloakMinDuration, CloakMaxDuration);
			Invoke ("DeactivateCloak", cloakDuration);
			CloakActive = true;
			AudioSource.PlayClipAtPoint(CloakSound, transform.position, 40f);
		}
	}

	private void DeactivateCloak()
	{
		if (CloakActive)
		{
			mAnimator.SetTrigger("DisengageCloak");
			CloakActive = false;
			AudioSource.PlayClipAtPoint(CloakSound, transform.position, 40f);
		}
	}

	private void ActivateShield()
	{
		Debug.Assert(GOShield != null);
		if(GOShield == null)
			return;
		
		DeactivateShield();
		float randomProbability = Random.Range(GlobalConstants.ProbabilityMin, GlobalConstants.ProbabilityMax);
		if(randomProbability < ShieldActivationProbability)
		{
			INSTShield = Instantiate(GOShield, transform.position, Quaternion.identity);
			INSTShield.transform.SetParent(transform);
			float shieldDuration = Random.Range(ShieldMinDuration, ShieldMaxDuration);
			Invoke("DeactivateShield", shieldDuration);
		}
	}

	private void DeactivateShield()
	{
		Destroy(INSTShield);
	}

	void Update()
	{
		if(GlobalConstants.FreezeAllNoTimeScale)
			return;

		FireProjectile();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(INSTShield == null)
		{
			HandleCollision(col);
		}
	}

	void FireProjectile()
	{
		float probability = ShotsPerSecond * Time.deltaTime;
		if(Random.value < probability)
		{
			GameObject projectile = (GameObject)Instantiate(Projectile, transform.position, Quaternion.identity);
			projectile.GetComponent<Projectile>().SetDamage(Damage);

			if(TrackingAmmunition)
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
		mScoreText.SetScore(Score);

		Instantiate(Explosion, transform.position, Quaternion.identity);
		int itemDropIndex = Random.Range(0, ItemDrops.Length);

		if(Random.value < ItemDropProbabilities[itemDropIndex])
		{
			GameObject itemDrop = Instantiate(ItemDrops[itemDropIndex], transform.position, Quaternion.identity);
			itemDrop.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.position.x, -5);
		}

		ActiveLevelHandler.EnemyKilled();

		AudioSource.PlayClipAtPoint(DeathSound, transform.position, 50f);
		Destroy(gameObject);
	}

	void HandleCollision(Collider2D col)
	{
		if (Invincible || CloakActive)
			return;
		
		Projectile laser = col.gameObject.GetComponent<Projectile>();
		if(laser)
		{
			laser.Hit();
			Health -= laser.GetDamage();
			if(Health <= 0)
				Die();
		}
	}
}
	