using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarManager : MonoBehaviour 
{
	private float MaxHitPoints 	= 0;
	private float HitPoints 	= 0;

	[SerializeField]	private Image UIBar = null;
	[SerializeField]	private Text UIText = null;

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
		float ratio = HitPoints / MaxHitPoints;
		UIBar.rectTransform.localScale = new Vector3(ratio, 1, 1);

		if(ratio > 0.75)			UIBar.color = new Color32(0, 255, 0, 200);
		else if(ratio > 0.50)		UIBar.color = new Color32(255, 255, 0, 200);
		else if(ratio > 0.25)		UIBar.color = new Color32(255, 165, 0, 200);
		else if(ratio >= 0)			UIBar.color = new Color32(255, 0, 0, 200);

		UIText.text = "H" + HitPoints.ToString() + "/" + MaxHitPoints;
	}
}
