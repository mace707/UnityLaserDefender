using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PropertyKeeper : MonoBehaviour 
{
	public void SetScore(float val)
	{
		GetComponent<Text>().text = val.ToString();
	}
}
