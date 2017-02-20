using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class CountDownText : MonoBehaviour 
{
	Text mText;
	void Start () 
	{
		mText = GetComponent<Text>();
		mText.text = "Wait";
	}

	int Timer = 0;

	public void StartCounter()
	{
		Timer = 3;
		mText.text = Timer.ToString();
		InvokeRepeating("UpdateCounter", 1, 1);
	}

	public void UpdateCounter()
	{
		Timer--;
		if(Timer <= -2)
			gameObject.SetActive(false);
		mText.text = Timer.ToString();
	}

}
