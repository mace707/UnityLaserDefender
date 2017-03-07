using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour 
{
	// Use this for initialization
	[SerializeField]
	public float HitPoints = 0;

	[SerializeField]
	public GameObject Explosion;

	[SerializeField]
	public int DustValue; // The currency name is "Dust"

	private DustText mDustText;

	void Start()
	{
		mDustText = GameObject.Find(StringConstants.PPDust).GetComponent<DustText>();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		Projectile laser = col.gameObject.GetComponent<Projectile> ();
		if(laser)
		{
			laser.Hit();
			HitPoints -= laser.GetDamage();
			if(HitPoints <= 0)
				Die();
		}
	}

	// Update is called once per frame
	void Update () 
	{
		transform.RotateAround(transform.position, new Vector3(0, 0, 1), Time.deltaTime * 50);
	}

	void Die()
	{
		Instantiate(Explosion, this.transform.position, Quaternion.identity);
		mDustText.SetScore(DustValue);
		Destroy(gameObject);
	}

}
