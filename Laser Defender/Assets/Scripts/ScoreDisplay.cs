using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		Debug.Log("Log");
		Text mText = GetComponent<Text>();
		mText.text = "SCORE: " + ScoreKeeper.Score.ToString();
		ScoreKeeper.Reset();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
