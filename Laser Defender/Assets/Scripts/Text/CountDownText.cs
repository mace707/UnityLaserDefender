using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class CountDownText : MonoBehaviour 
{
	void Start () 
	{
		GetComponent<Text>().text = "GET READY";
	}
}
