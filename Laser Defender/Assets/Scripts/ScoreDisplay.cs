using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour {

	// Use this for initialization
	// End Game Display
	void Start () 
	{
		Debug.Log("Log");
		Text mText = GetComponent<Text>();
		mText.text = "SCORE: " + ScoreText.Score.ToString();
		ScoreText.Reset();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
