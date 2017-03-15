using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour 
{
	float XMin = -5;
	float XMax = 5;
	float Padding = 0.5f;

	public float ProjectileSpeed = 0.0f;
	public float FiringRate = 0.2f;

	public GameObject ScatterGunGO;

	public AudioClip FireSound;

	[SerializeField]
	public bool DoubleShotEnabled = true;

	public Transform CustomizeCanvas;

	public GameObject mPropertyKeeperHealth;
	public GameObject mPropertyKeeperDamage;
	public GameObject mPropertyKeeperSpeed;
	public GameObject mPropertyKeeperProjectiles;

	public GameObject mPropertyKeeperHealthCost;
	public GameObject mPropertyKeeperDamageCost;
	public GameObject mPropertyKeeperSpeedCost;
	public GameObject mPropertyKeeperProjectilesCost;

	[SerializeField]
	private float MaxHitPoints = 0;
	private float HitPoints = 0;

	[SerializeField]
	public float Speed = 15.0f;
	private float DefaultSpeed = 15.0f;
	private float StartingSpeed = 15.0f;

	public float Damage = 100;
	private float DefaultDamage = 100;
	private float StartingDamage = 100;

	private DustText mDustKeeper;

	public GameObject MenuHandlerGO;
	private InGameMenuHandler MenuHandler;

	Weapon PrimaryWeapon;

	[SerializeField]
	private GameObject GOShield = null;
	private Shield mShield;

	[SerializeField]
	private GameObject GOHPBar = null;
	private HPBarManager mHPBar;

	public bool FreezePlayer = false;

	[SerializeField]
	private GameObject GOFocus = null;
	private Focus mFocus;

	// Use this for initialization
	void Start () 
	{
		MenuHandler = MenuHandlerGO.GetComponent<InGameMenuHandler>();
		//StartingDamage = PlayerPrefs.GetFloat(StringConstants.PPDamage, DefaultDamage);

		Damage = StartingDamage;

	//	StartingHitpoints = PlayerPrefs.GetFloat(StringConstants.PPHitPoints, DefaultHitpoints);
	//	HitPoints = StartingHitpoints;
	//	MaxHitPoints = StartingHitpoints;

		HitPoints = MaxHitPoints;

	//	StartingSpeed = PlayerPrefs.GetFloat(StringConstants.PPSpeed, DefaultSpeed);
		Speed = StartingSpeed;

		//Distance between the camera and the object.
		float distance = transform.position.z - Camera.main.transform.position.z;

		// The vector in brackets is reperesents the screen, where 0 is left most and 
		// 1 is right most with 0.5 being center.
		XMin = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distance)).x + Padding;
		XMax = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, distance)).x - Padding;

		mDustKeeper = GameObject.Find(StringConstants.PPDust).GetComponent<DustText>();

		mFocus = GOFocus.GetComponent<Focus>();
		mFocus.StartGathering();
		PrimaryWeapon = WeaponFactory.GetWeapon(WeaponFactory.WeaponType.WeaponTypeRocketLauncher);
		mShield = GOShield.GetComponent<Shield>();
		mHPBar = GOHPBar.GetComponent<HPBarManager>();
		mHPBar.Setup(MaxHitPoints);
	}

	void FireProjectile()
	{
		AudioSource.PlayClipAtPoint(FireSound, transform.position);
		// Quaternion.identity -> means no rotations

		if(!DoubleShotEnabled)
		{
			Vector3 leftBullet = new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z);
			Vector3 rightBullet = new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z);
			PrimaryWeapon.Fire(leftBullet, new Vector3(0, ProjectileSpeed, 0));
			PrimaryWeapon.Fire(rightBullet, new Vector3(0, ProjectileSpeed, 0));
		} 
		else
		{
			PrimaryWeapon.Fire(transform.position, new Vector3(0, ProjectileSpeed, 0));
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if(GlobalConstants.FreezeAllNoTimeScale)
			return;

		if(!FreezePlayer)
		{
			HandleKeyPress();
			// Restrict the player to the game space.
			float newX = Mathf.Clamp(transform.position.x, XMin, XMax);
			transform.position = new Vector3(newX, transform.position.y, transform.position.z);
		}	
	}

	void HandleKeyPress()
	{
		// Time.Deltatime (Time between frames) makes it frame rate independant. 
		// If a frame takes longer to render, it will move at a higher speed.
		if (Input.GetKey (KeyCode.LeftArrow))			transform.position += Vector3.left * Speed * Time.deltaTime;
		else if (Input.GetKey (KeyCode.RightArrow))		transform.position += Vector3.right * Speed * Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.Space)) 		InvokeRepeating ("FireProjectile", 0, FiringRate);
		if (Input.GetKeyUp (KeyCode.Space))			CancelInvoke ("FireProjectile");

		if(Input.GetKeyDown(KeyCode.LeftShift))
		{
			if(mShield.IsShieldActive())
				mShield.Deactivate();
			else
				mShield.Activate(transform);
		}

		if(Input.GetKeyDown(KeyCode.LeftControl))
			FireScatterGun();
			
	}

	//FireSpecial...
	public void FireScatterGun()
	{
		if (mFocus.Consume ()) 
		{
			float pl = 0;
			float pr = 0;
			for (int i = 0; i < 10; i += 2) 
			{
				pl -= 3;
				pr += 3;
			
				GameObject leftBeam = (GameObject)Instantiate (ScatterGunGO, transform.position, Quaternion.identity);
				GameObject rightBeam = (GameObject)Instantiate (ScatterGunGO, transform.position, Quaternion.identity);

				rightBeam.GetComponent<Projectile> ().SetDamage (Damage);
				leftBeam.GetComponent<Projectile> ().SetDamage (Damage);

				rightBeam.GetComponent<Rigidbody2D> ().velocity = new Vector3 (pl, ProjectileSpeed, 0);
				leftBeam.GetComponent<Rigidbody2D> ().velocity = new Vector3 (pr, ProjectileSpeed, 0);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(mShield.IsShieldActive())
			return;

		Projectile laser = col.gameObject.GetComponent<Projectile>();

		if(laser)
		{
			if(laser.ProjectileDamageType == Projectile.DamageType.DamageTypeFrost)
				SlowDownPlayer(laser.SlowDownFactor);
			else if(laser.ProjectileDamageType == Projectile.DamageType.DamageTypeExplosion)
				PostExplosionShift(laser.transform.position);

			laser.Hit();
			HitPoints -= laser.GetDamage();
			mHPBar.DamageTaken(laser.GetDamage ());
			if(HitPoints <= 0)
				Die();
			return;
		}
	}

	void Die()
	{
		SaveSettings();
		LevelManager mgr = GameObject.Find(StringConstants.SCRIPTLevelManager).GetComponent<LevelManager>();
		mgr.LoadLevel(StringConstants.SCENEWin);
		Destroy(gameObject);
	}

	void SaveSettings()
	{
		PlayerPrefs.SetInt(StringConstants.PPDust, DustText.Dust);
	}

	void PostExplosionShift(Vector3 impactLocation)
	{
		if (impactLocation.x < transform.position.x)			transform.position += Vector3.right;
		else													transform.position += Vector3.left;
	}

	void SlowDownPlayer(float factor)
	{
		Speed *= factor;
		ProjectileSpeed *= factor;
		FiringRate /= factor;
		StartCoroutine(ReturnToNormal(factor, 5));
	}

	IEnumerator ReturnToNormal(float factor, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		Speed /= factor;
		ProjectileSpeed /= factor;
		FiringRate *= factor;
	}

	public void CustomizeShip()
	{
		MenuHandler.ActivatePlayerCustomizationMenu();
		ResetValues();
		Invoke("UpdateValues", 0.000001f);
	}

	public void UpdateValues()
	{
		mPropertyKeeperHealth.GetComponent<Text>().text 			= HitPoints.ToString();
		mPropertyKeeperSpeed.GetComponent<Text>().text 				= Speed.ToString();
		mPropertyKeeperDamage.GetComponent<Text>().text 			= Damage.ToString();
		mPropertyKeeperProjectiles.GetComponent<Text>().text 		= "1";

		mPropertyKeeperHealthCost.GetComponent<Text>().text 		= UpgradeCostHealth().ToString() + "$";
		mPropertyKeeperSpeedCost.GetComponent<Text>().text 			= UpgradeCostSpeed().ToString() + "$";
		mPropertyKeeperDamageCost.GetComponent<Text>().text 		= UpgradeCostDamage().ToString() + "$";
		mPropertyKeeperProjectilesCost.GetComponent<Text>().text 	= "NA";
	}

	public void CustomizeShipCanceled()
	{
		ResetValues();
		MenuHandler.ActivatePauseMenu();
	}

	public void AcceptShipUpgrades()
	{
		PlayerPrefs.SetFloat(StringConstants.PPHitPoints, HitPoints);
		MaxHitPoints = HitPoints;

		PlayerPrefs.SetFloat(StringConstants.PPSpeed, Speed);
		StartingSpeed = Speed;

		PlayerPrefs.SetFloat(StringConstants.PPDamage, Damage);
		StartingDamage = Damage;

	//	UpdateHealthBar();
		MenuHandler.ActivatePauseMenu();
	}

	public void ResetValues()
	{
		HitPoints = MaxHitPoints;
		Speed = StartingSpeed;
		Damage = StartingDamage;
	}

	public void UpgradeHealth()
	{
		float cost = UpgradeCostHealth();
		if(cost <= DustText.Dust)
		{
			mDustKeeper.SetScore((int)-cost);
			HitPoints *= 2;
			UpdateValues();
		}
	}
		
	public void UpgradeSpeed()
	{
		float cost = UpgradeCostSpeed();
		if(cost <= DustText.Dust)
		{
			mDustKeeper.SetScore((int)-cost);
			Speed += 5;
			UpdateValues();
		}
	}

	public void UpgradeDamage()
	{
		float cost = UpgradeCostDamage();
		if(cost <= DustText.Dust)
		{
			mDustKeeper.SetScore((int)-cost);
			Damage *= 2;
			UpdateValues();
		}
	}


	private float UpgradeCostHealth()
	{
		return HitPoints;/// DefaultHitpoints * 100;
	}

	private float UpgradeCostDamage()
	{
		return Damage / DefaultDamage * 100;
	}

	private float UpgradeCostSpeed()
	{
		return ((Speed + 5 - DefaultSpeed)/5) * 200;
	}


	public void PrepareDefaults()
	{
		Damage = StartingDamage;
		HitPoints = MaxHitPoints;//StartingHitpoints;
		Speed = StartingSpeed;
	}
}