using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Focus : MonoBehaviour
{
	[SerializeField]	private float MaxFocusPoints 	= 0;
	[SerializeField]	private int Increment 			= 0;
	[SerializeField]	private float RepeatRate 		= 0;
	[SerializeField]	private float Cost 				= 0;

	[SerializeField]	private Image UIBar 			= null;
	[SerializeField]	private Text UIText 			= null;

	private float FocusPoints = 0;

	public void StartGathering()
	{
		InvokeRepeating("Gather", 0, RepeatRate);
	}

	public void StopGathering()
	{
		CancelInvoke("Gather");
	}

	private void Gather()
	{
		if (GlobalConstants.FreezeAllNoTimeScale) 
		{
			UpdateUI(FocusPoints);
			return;
		}
		
		if(FocusPoints < MaxFocusPoints)
		{
			FocusPoints += Increment;
			FocusPoints = Mathf.Clamp(FocusPoints, 0, MaxFocusPoints);
			UpdateUI(FocusPoints);
		}
	}

	public bool Consume()
	{
		if (FocusPoints - Cost >= 0)
		{
			FocusPoints -= Cost;
			UpdateUI(FocusPoints);
			return true;
		}
		return false;
	}

	private void UpdateUI(float value)
	{
		float ratio = FocusPoints / MaxFocusPoints;
		UIBar.rectTransform.localScale = new Vector3(ratio, 1, 1);
		UIText.text = "F " + FocusPoints.ToString() + "/" + MaxFocusPoints;
	}
}
