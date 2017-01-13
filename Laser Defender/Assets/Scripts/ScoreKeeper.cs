using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour
{
	public static int Score = 0;
	private Text mText;

	void Start()
	{
		mText = GetComponent<Text>();
		Reset();
	}

	public void SetScore(int points)
	{
		Score += points;
		mText.text = Score.ToString();
	}

	public static void Reset()
	{
		Score = 0;
	}
}
