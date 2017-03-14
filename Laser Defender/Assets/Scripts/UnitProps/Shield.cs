using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour 
{
	[SerializeField]	private float MaxShieldPoints 	= 0;
	[SerializeField] 	private float ShieldPoints 		= 0;
	[SerializeField]	private int ConsumptionRate 	= 0;
	[SerializeField]	private int RegenerationRate 	= 0;

	private Image UIBar 				= null;
	private Text UIText 				= null;
	private GameObject GOActiveShield 	= null;

	public void StartRegenerating()
	{
		InvokeRepeating("Regenerate", 5, 1);
	}

	private void Regenerate()
	{
		if (GlobalConstants.FreezeAllNoTimeScale)
		{
			UpdateUI();
			return;
		}

		if(ShieldPoints < MaxShieldPoints)
		{
			ShieldPoints += RegenerationRate;
			ShieldPoints = Mathf.Clamp(ShieldPoints, 0, MaxShieldPoints);
			UpdateUI();
		}
		else
			StopRegenerating();
	}

	public void StopRegenerating()
	{
		CancelInvoke("Regenerate");
	}

	public void ActivateShield(Transform parent)
	{
		StopRegenerating();
		UIBar = GameObject.Find("SPBarForeground").GetComponent<Image>();
		UIText = GameObject.Find("SPDisplayText").GetComponent<Text>();
		GOActiveShield = Instantiate(gameObject, parent.position, Quaternion.identity);
		GOActiveShield.transform.SetParent(parent);
		InvokeRepeating("Consume", 0, 1);
	}

	private void Consume()
	{
		if (GlobalConstants.FreezeAllNoTimeScale)
		{
			UpdateUI();
			return;
		}

		if(ShieldPoints > 0)
		{
			ShieldPoints -= ConsumptionRate;
			ShieldPoints = Mathf.Clamp(ShieldPoints, 0, MaxShieldPoints);
			UpdateUI();
		}
		else
			DeactivateShield();
	}

	public void DeactivateShield()
	{
		CancelInvoke("Consume");
		Destroy(GOActiveShield);
		StartRegenerating();
	}

	public bool IsShieldActive()
	{
		return GOActiveShield != null;
	}

	private void UpdateUI()
	{
		float ratio = ShieldPoints / MaxShieldPoints;
		UIBar.rectTransform.localScale = new Vector3(ratio, 1, 1);
		UIText.text = "S " + ShieldPoints.ToString() + "/" + MaxShieldPoints;
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		Projectile laser = col.gameObject.GetComponent<Projectile>();
		if(laser)
			laser.Hit();
	}
}
