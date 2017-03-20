using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour 
{
	private float MaxHitPoints 	= 0;
	private float HitPoints 	= 0;

	public void Setup(float maxHitpoints)
	{
		MaxHitPoints = maxHitpoints;
		HitPoints = MaxHitPoints;
		UpdateUI();
	}

	public void DamageTaken(float dmg)
	{
		HitPoints -= dmg;
		HitPoints = Mathf.Clamp (HitPoints, 0, MaxHitPoints);
		UpdateUI();
	}

	private void UpdateUI()
	{
		ProgBarHandler.UpdateUIComponent (StringConstants.UITEXT_HitPoints, StringConstants.UIIMAGE_HitPoints, 'H', HitPoints, MaxHitPoints);
	}
}
