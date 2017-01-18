using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour 
{
	[SerializeField]
	float Speed = 15.0f;

	float XMin = -5;
	float XMax = 5;
	float Padding = 0.5f;

	public GameObject Projectile;
	public GameObject Shield;
	public float ProjectileSpeed = 0.0f;
	public float FiringRate = 0.2f;
	public float HitPoints = 100;
	private float MaxHitPoints = 100;
	public float ShieldPoints = 100;
	private float MaxShieldPoints = 100;


	private GameObject ActiveShield;

	public AudioClip FireSound;
	Animator mAnimator;
	public Image HealthBarForeGround;
	public Text HealthBarRatioText;

	public Image ShieldBarForGround;
	public Text ShieldBarRatioText;

	[SerializeField]
	public bool DoubleShotEnabled = true;

	// Use this for initialization
	void Start () 
	{
		MaxHitPoints = HitPoints;
		//Distance between the camera and the object.
		float distance = transform.position.z - Camera.main.transform.position.z;

		// The vector in brackets is reperesents the screen, where 0 is left most and 
		// 1 is right most with 0.5 being center.
		XMin = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distance)).x + Padding;
		XMax = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, distance)).x - Padding;

		mAnimator = GetComponent<Animator>();

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
			GameObject leftBeam = (GameObject)Instantiate(Projectile, leftBeamPos, Quaternion.identity);
			GameObject rightBeam = (GameObject)Instantiate(Projectile, rightBeamPos, Quaternion.identity);
			leftBeam.GetComponent<Rigidbody2D>().velocity = new Vector3(0, ProjectileSpeed, 0);
			rightBeam.GetComponent<Rigidbody2D>().velocity = new Vector3(0, ProjectileSpeed, 0);
		} 
		else
		{
			GameObject beam = (GameObject)Instantiate(Projectile, transform.position, Quaternion.identity);
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
		if (Input.GetKey (KeyCode.LeftArrow))
		{
			transform.position += Vector3.left * Speed * Time.deltaTime;
			mAnimator.SetInteger ("PlayerDirrection", -1);
		}
		else if (Input.GetKey (KeyCode.RightArrow))
		{
			transform.position += Vector3.right * Speed * Time.deltaTime;
			mAnimator.SetInteger ("PlayerDirrection", 1);
		}
		else
		{
			mAnimator.SetInteger ("PlayerDirrection", 0);
		}

		if (Input.GetKeyDown(KeyCode.Space)) 
			InvokeRepeating ("FireProjectile", 0.00001f, FiringRate);
		if (Input.GetKeyUp (KeyCode.Space))
			CancelInvoke ("FireProjectile");

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
			laser.Hit();
			HitPoints -= laser.GetDamage();
			UpdateHealthBar();
			if(HitPoints <= 0)
				Die();
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
		LevelManager mgr = GameObject.Find("LevelManager").GetComponent<LevelManager>();
		mgr.LoadLevel("Win Screen");
		Destroy(gameObject);
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

		if(ratio > 0.75)
			HealthBarForeGround.color = new Color32(0, 255, 0, 200);
		else if(ratio > 0.50)
			HealthBarForeGround.color = new Color32(255, 255, 0, 200);
		else if(ratio > 0.25)
			HealthBarForeGround.color = new Color32(255, 165, 0, 200);
		else if(ratio >= 0)
			HealthBarForeGround.color = new Color32(255, 0, 0, 200);

		HealthBarRatioText.text = "HP " + (Mathf.Floor(ratio * 100)).ToString() + '%';
	}
}
