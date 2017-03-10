using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Focus
{
	private int FocusPoints = 0;
	private int MaxFocusPoints = 0;
	private int Increment = 0;
	private float RepeatRate = 0;

	public Image UIBar;
	public Text UIText;

	public Focus(int maxFocusPoints, int increment, float repeatRate)
	{
		MaxFocusPoints = maxFocusPoints;
		Increment = increment;
		RepeatRate = repeatRate;
		UIBar = GameObject.Find("FPBarForeground").GetComponent<Image>();
		UIText = GameObject.Find("FPDisplayText").GetComponent<Text>();
	}

	public void StartGathering(MonoBehaviour monoBehaviour)
	{
		monoBehaviour.InvokeRepeating("Gather", 0, RepeatRate);
	}

	public void StopGathering()
	{
		MonoBehaviour.CancelInvoke("Gather");
	}

	private void Gather()
	{
		if(FocusPoints < MaxFocusPoints)
		{
			FocusPoints += Increment;
			FocusPoints = Mathf.Clamp(FocusPoints, 0, MaxFocusPoints);
			UpdateUI();
		}
	}

	private void UpdateUI()
	{
		float ratio = FocusPoints / MaxFocusPoints;
		UIBar.rectTransform.localScale = new Vector3(ratio, 1, 1);
		UIText.text = "F " + FocusPoints.ToString() + "/" + MaxFocusPoints.ToString();
	}
}
