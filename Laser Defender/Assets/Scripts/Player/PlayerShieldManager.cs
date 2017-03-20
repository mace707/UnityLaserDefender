using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShieldManager : MonoBehaviour
{
	[SerializeField]	private float MaxShieldPoints 	= 0;
	[SerializeField] 	private float ShieldPoints 		= 0;
	[SerializeField]	private int ConsumptionRate 	= 0;
	[SerializeField]	private int RegenerationRate 	= 0;
	[SerializeField]  	private bool HasRegeneration	= false;

	private bool Active = false;

	public void StartRegenerating()
	{
		if(HasRegeneration)
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

	public void Activate(Transform parent)
	{
		Active = true;
		StopRegenerating();
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
			Deactivate();
	}

	public void Deactivate()
	{
		Active = false;
		CancelInvoke("Consume");
		StartRegenerating();
	}

	public bool IsActive()
	{
		return Active;
	}

	private void UpdateUI()
	{
		ProgBarHandler.UpdateUIComponent (StringConstants.UITEXT_ShieldPoints, StringConstants.UIIMAGE_ShieldPoints, 'S', ShieldPoints, MaxShieldPoints);
	}

	public void OnTriggerEnter2D(Collider2D col)
	{
		Projectile laser = col.gameObject.GetComponent<Projectile>();
		if(laser)
			laser.Hit();
	}
}
