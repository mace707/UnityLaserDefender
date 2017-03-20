using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusManager : MonoBehaviour
{
	[SerializeField]	private float MaxFocusPoints 	= 0;
	[SerializeField]	private int Increment 			= 0;
	[SerializeField]	private float RepeatRate 		= 0;
	[SerializeField]	private float Cost 				= 0;

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
			UpdateUI();
			return;
		}
		
		if(FocusPoints < MaxFocusPoints)
		{
			FocusPoints += Increment;
			FocusPoints = Mathf.Clamp(FocusPoints, 0, MaxFocusPoints);
			UpdateUI();
		}
	}

	public bool Consume()
	{
		if (FocusPoints - Cost >= 0)
		{
			FocusPoints -= Cost;
			UpdateUI();
			return true;
		}
		return false;
	}

	private void UpdateUI()
	{
		ProgBarHandler.UpdateUIComponent (StringConstants.UITEXT_FocusPoints, StringConstants.UIIMAGE_FocusPoints, 'F', FocusPoints, MaxFocusPoints);
	}
}
