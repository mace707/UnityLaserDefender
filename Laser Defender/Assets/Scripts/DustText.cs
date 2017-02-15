using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DustText : MonoBehaviour 
{
	public static int Dust = 0;
	private Text mText;

	void Start()
	{
		mText = GetComponent<Text>();
		Dust = PlayerPrefs.GetInt(StringConstants.PPDust, DustText.Dust);
		mText.text = "$" + Dust.ToString();
	}

	public void SetScore(int dust)
	{
		Dust += dust;
		mText.text = "$" + Dust.ToString();
	}

	public static void Reset()
	{
		Dust = 0;
	}

}
