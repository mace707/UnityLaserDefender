using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour 
{
	[SerializeField]
	private float MaxShieldPoints = 0;

	[SerializeField]
	private int Increment = 0;

	[SerializeField]
	private float RepeatRate = 0;

	[SerializeField]
	private float Cost = 0;

	private float ShieldPoints = 0;

	GameObject GOActiveShield = null;
	void Start()
	{
		ShieldPoints = MaxShieldPoints;
	}

	public void Activate(Transform parent)
	{
		if(!GOActiveShield)
		{
			GOActiveShield = Instantiate(gameObject, parent.position, Quaternion.identity);
			GOActiveShield.transform.SetParent(parent);
			InvokeRepeating("Consume", 0, 5);
		}
		else
			Deactivate();
	}

	private void Consume()
	{
		if(ShieldPoints - Cost >= 0)
			ShieldPoints -= Cost;
		else
			Deactivate();
		Debug.Log(ShieldPoints);
	}

	public void Deactivate()
	{
		CancelInvoke("Consume");
		Destroy(GOActiveShield);
	}

	public bool Active()
	{
		return GOActiveShield != null;
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		Projectile laser = col.gameObject.GetComponent<Projectile>();

		if(laser)
			laser.Hit();
	}
}
