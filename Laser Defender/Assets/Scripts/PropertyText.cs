using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PropertyText : MonoBehaviour 
{
	public void SetScore(float val)
	{
		GetComponent<Text>().text = val.ToString();
	}
}
