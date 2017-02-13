using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour 
{
	float XMin = -5;
	float XMax = 5;
	float Padding = 0.5f;

	public GameObject ProjectileGO;
	public GameObject Shield;
	public float ProjectileSpeed = 0.0f;
	public float FiringRate = 0.2f;

	private GameObject ActiveShield;

	public AudioClip FireSound;
	public Image HealthBarForeGround;
	public Text HealthBarRatioText;

	public Image ShieldBarForGround;
	public Text ShieldBarRatioText;

	[SerializeField]
	public bool DoubleShotEnabled = true;

	public Transform CustomizeCanvas;

	public GameObject mPropertyKeeperHealth;
	public GameObject mPropertyKeeperDamage;
	public GameObject mPropertyKeeperSpeed;
	public GameObject mPropertyKeeperShield;
	public GameObject mPropertyKeeperProjectiles;

	public GameObject mPropertyKeeperHealthCost;
	public GameObject mPropertyKeeperDamageCost;
	public GameObject mPropertyKeeperSpeedCost;
	public GameObject mPropertyKeeperShieldCost;
	public GameObject mPropertyKeeperProjectilesCost;

	public float MaxHitPoints = 100;
	private float HitPoints = 100;
	private float DefaultHitpoints = 100;
	private float StartingHitpoints = 100;

	[SerializeField]
	public float Speed = 15.0f;
	private float DefaultSpeed = 15.0f;
	private float StartingSpeed = 15.0f;

	public float Damage = 100;
	private float DefaultDamage = 100;
	private float StartingDamage = 100;

	private DustKeeper mDustKeeper;

	public float MaxShieldPoints = 100;
	private float ShieldPoints = 100;
	private float DefaultShieldPoints = 100;
	private float StartingShieldPoints = 100;

	// Use this for initialization
	void Start () 
	{
		StartingDamage = PlayerPrefs.GetFloat(StringConstants.PPDamage, DefaultDamage);
		Damage = StartingDamage;

		StartingHitpoints = PlayerPrefs.GetFloat(StringConstants.PPHitPoints, DefaultHitpoints);
		HitPoints = StartingHitpoints;
		MaxHitPoints = StartingHitpoints;

		StartingSpeed = PlayerPrefs.GetFloat(StringConstants.PPSpeed, DefaultSpeed);
		Speed = StartingSpeed;

		StartingShieldPoints = PlayerPrefs.GetFloat(StringConstants.PPShieldPoints, DefaultShieldPoints);
		ShieldPoints = StartingShieldPoints;
		MaxShieldPoints = StartingShieldPoints;

		//Distance between the camera and the object.
		float distance = transform.position.z - Camera.main.transform.position.z;

		// The vector in brackets is reperesents the screen, where 0 is left most and 
		// 1 is right most with 0.5 being center.
		XMin = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distance)).x + Padding;
		XMax = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, distance)).x - Padding;

		mDustKeeper = GameObject.Find(StringConstants.PPDust).GetComponent<DustKeeper>();

		UpdateHealthBar();
		UpdateShieldPointBar();
	}

	void FireProjectile()
	{
		AudioSource.PlayClipAtPoint(FireSound, transform.position);
		// Quaternion.identity -> means no rotations

		if(DoubleShotEnabled)
		{
			Vector3 leftBeamPos = new Vector3(transform.position.x - 0.5f, transform.position.y + 0.25f, transform.position.z);
			Vector3 rightBeamPos = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.25f, transform.position.z);
			GameObject leftBeam = (GameObject)Instantiate(ProjectileGO, leftBeamPos, Quaternion.identity);
			GameObject rightBeam = (GameObject)Instantiate(ProjectileGO, rightBeamPos, Quaternion.identity);
			leftBeam.GetComponent<Rigidbody2D>().velocity = new Vector3(0, ProjectileSpeed, 0);
			rightBeam.GetComponent<Rigidbody2D>().velocity = new Vector3(0, ProjectileSpeed, 0);
		} 
		else
		{
			GameObject beam = (GameObject)Instantiate(ProjectileGO, transform.position, Quaternion.identity);
			beam.GetComponent<Projectile>().SetDamage(Damage);
			beam.GetComponent<Rigidbody2D>().velocity = new Vector3(0, ProjectileSpeed, 0);
		}
	}

	// Update is called once per frame
	void Update () 
	{
		HandleKeyPress();
		// Restrict the player to the game space.
		float newX = Mathf.Clamp(transform.position.x, XMin, XMax);
		transform.position = new Vector3 (newX, transform.position.y, transform.position.z);
		if(ActiveShield)
		{
			ActiveShield.transform.position = transform.position;
		} 
		else
		{
			CancelInvoke ("DepleteShield");
		}
			
	}

	void HandleKeyPress()
	{
		// Time.Deltatime (Time between frames) makes it frame rate independant. 
		// If a frame takes longer to render, it will move at a higher speed.
		if (Input.GetKey (KeyCode.LeftArrow))			transform.position += Vector3.left * Speed * Time.deltaTime;
		else if (Input.GetKey (KeyCode.RightArrow))		transform.position += Vector3.right * Speed * Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.Space)) 		InvokeRepeating ("FireProjectile", FiringRate, FiringRate);
		if (Input.GetKeyUp (KeyCode.Space))			CancelInvoke ("FireProjectile");

		if(Input.GetKeyDown(KeyCode.LeftShift))
		{
			if(!ActiveShield && ShieldPoints > 0)
			{
				ActiveShield = (GameObject)Instantiate(Shield, transform.position, Quaternion.identity);
				InvokeRepeating("DepleteShield", 0.0001f, 0.1f);
				CancelInvoke("RegenerateShield");
			} 
			else if (ActiveShield)
			{	
				Destroy(ActiveShield);
				InvokeRepeating("RegenerateShield", 5f, 0.5f);
				CancelInvoke("DepleteShield");
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		Projectile laser = col.gameObject.GetComponent<Projectile>();

		if(laser)
		{
			if(laser.IsIceProjectile)
				SlowDownPlayer(laser.SlowDownFactor);
			else if(laser.IsExplodingProjectile)
				PostExplosionShift(laser.transform.position);

			laser.Hit();
			HitPoints -= laser.GetDamage();
			UpdateHealthBar();
			if(HitPoints <= 0)
				Die();
			return;
		}

		ItemDropShield shield = col.gameObject.GetComponent<ItemDropShield>();
		if(shield && ShieldPoints < MaxShieldPoints)
		{
			shield.Hit();
			ShieldPoints += MaxShieldPoints/10;

			if(ShieldPoints > MaxShieldPoints)
				ShieldPoints = MaxShieldPoints;

			UpdateShieldPointBar();
			return;
		}

		ItemDropHealth health = col.gameObject.GetComponent<ItemDropHealth>();
		if(health && HitPoints < MaxHitPoints)
		{
			health.Hit();
			HitPoints += MaxHitPoints/10;

			if(HitPoints > MaxHitPoints)
				HitPoints = MaxHitPoints;

			UpdateHealthBar();
			return;
		}
	}

	void DepleteShield()
	{
		ShieldPoints--;
		UpdateShieldPointBar();
		if(ShieldPoints <= 0)
		{
			Destroy(ActiveShield);
			CancelInvoke("DepleteShield");
			ShieldPoints = 0;
			InvokeRepeating("RegenerateShield", 5f, 0.5f);
		}
	}

	void RegenerateShield()
	{
		ShieldPoints++;
		UpdateShieldPointBar();
		if(ShieldPoints >= MaxShieldPoints)
		{
			CancelInvoke("RegenerateShield");
			ShieldPoints = MaxShieldPoints;
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
		PlayerPrefs.SetInt(StringConstants.PPDust, DustKeeper.Dust);
	}

	private void UpdateShieldPointBar()
	{
		float ratio = ShieldPoints / MaxShieldPoints;
		ShieldBarForGround.rectTransform.localScale = new Vector3(ratio, 1, 1);
		ShieldBarRatioText.text = "SP " + (Mathf.Floor(ratio * 100)).ToString() + '%';
	}

	private void UpdateHealthBar()
	{
		float ratio = HitPoints / MaxHitPoints;
		HealthBarForeGround.rectTransform.localScale = new Vector3(ratio, 1, 1);

		if(ratio > 0.75)			HealthBarForeGround.color = new Color32(0, 255, 0, 200);
		else if(ratio > 0.50)		HealthBarForeGround.color = new Color32(255, 255, 0, 200);
		else if(ratio > 0.25)		HealthBarForeGround.color = new Color32(255, 165, 0, 200);
		else if(ratio >= 0)			HealthBarForeGround.color = new Color32(255, 0, 0, 200);

		HealthBarRatioText.text = "HP " + HitPoints.ToString() + "/" + MaxHitPoints.ToString();
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
		Time.timeScale = 1;
		CustomizeCanvas.gameObject.SetActive(true);

		ResetValues();

		Invoke("UpdateValues", 0.000001f);
	}

	public void UpdateValues()
	{
		mPropertyKeeperHealth.GetComponent<PropertyKeeper>().SetScore(HitPoints);
		mPropertyKeeperSpeed.GetComponent<PropertyKeeper>().SetScore(Speed);
		mPropertyKeeperDamage.GetComponent<PropertyKeeper>().SetScore(Damage);
		mPropertyKeeperShield.GetComponent<PropertyKeeper>().SetScore(ShieldPoints);
		mPropertyKeeperProjectiles.GetComponent<PropertyKeeper>().SetScore(1);

		mPropertyKeeperHealthCost.GetComponent<Text>().text 		= UpgradeCostHealth().ToString() + "$";
		mPropertyKeeperSpeedCost.GetComponent<Text>().text 			= UpgradeCostSpeed().ToString() + "$";
		mPropertyKeeperDamageCost.GetComponent<Text>().text 		= UpgradeCostDamage().ToString() + "$";
		mPropertyKeeperShieldCost.GetComponent<Text>().text 		= UpgradeCostShield().ToString() + "$";
		mPropertyKeeperProjectilesCost.GetComponent<Text>().text 	= "NA";

		Time.timeScale = 0;
	}

	public void CustomizeShipCanceled()
	{
		ResetValues();
		CustomizeCanvas.gameObject.SetActive(false);
	}

	public void AcceptShipUpgrades()
	{
		PlayerPrefs.SetFloat(StringConstants.PPHitPoints, HitPoints);
		StartingHitpoints = HitPoints;
		MaxHitPoints = HitPoints;

		PlayerPrefs.SetFloat(StringConstants.PPSpeed, Speed);
		StartingSpeed = Speed;

		PlayerPrefs.SetFloat(StringConstants.PPDamage, Damage);
		StartingDamage = Damage;

		PlayerPrefs.SetFloat(StringConstants.PPShieldPoints, ShieldPoints);
		StartingShieldPoints = ShieldPoints;
		MaxShieldPoints = ShieldPoints;

		UpdateHealthBar();
		UpdateShieldPointBar();

		CustomizeCanvas.gameObject.SetActive(false);
	}

	public void ResetValues()
	{
		HitPoints = StartingHitpoints;
		Speed = StartingSpeed;
		Damage = StartingDamage;
		ShieldPoints = StartingShieldPoints;
	}

	public void UpgradeHealth()
	{
		float cost = UpgradeCostHealth();
		if(cost <= DustKeeper.Dust)
		{
			mDustKeeper.SetScore((int)-cost);
			HitPoints *= 2;
			UpdateValues();
		}
	}
		
	public void UpgradeSpeed()
	{
		float cost = UpgradeCostSpeed();
		if(cost <= DustKeeper.Dust)
		{
			mDustKeeper.SetScore((int)-cost);
			Speed += 5;
			UpdateValues();
		}
	}

	public void UpgradeDamage()
	{
		float cost = UpgradeCostDamage();
		if(cost <= DustKeeper.Dust)
		{
			mDustKeeper.SetScore((int)-cost);
			Damage *= 2;
			UpdateValues();
		}
	}

	public void UpgradeShield()
	{
		float cost = UpgradeCostShield();
		if(cost <= DustKeeper.Dust)
		{
			mDustKeeper.SetScore((int)-cost);
			ShieldPoints *= 2;
			UpdateValues();
		}
	}

	private float UpgradeCostHealth()
	{
		return HitPoints / DefaultHitpoints * 100;
	}

	private float UpgradeCostDamage()
	{
		return Damage / DefaultDamage * 100;
	}

	private float UpgradeCostSpeed()
	{
		return ((Speed + 5 - DefaultSpeed)/5) * 200;
	}

	private float UpgradeCostShield()
	{
		return ShieldPoints / DefaultShieldPoints * 100;
	}
}