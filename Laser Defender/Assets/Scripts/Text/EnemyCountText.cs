using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCountText : MonoBehaviour
{
	public int Count = 0;
	public float Max = 0;
	private Text mText;

	public Image WCBar;

	public void SetMax(int max)
	{
		Max = max;
		Count = max;
		RefreshCount();
	}

	void Start()
	{
		mText = GetComponent<Text>();
		Reset();
	}

	public void RefreshCount()
	{
		float perc = Count / Max;
		WCBar.rectTransform.localScale = new Vector3(perc, 1, 1);
		mText.text = "REMAINING: " + Count.ToString();
	}

	public void ChangeCount()
	{
		Count--;
		RefreshCount();
	}

	public void Reset()
	{
		Count = 0;
	}
}
