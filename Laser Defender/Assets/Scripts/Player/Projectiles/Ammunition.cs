using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammunition : MonoBehaviour 
{
	[SerializeField]
	private GameObject GOLaser;
	[SerializeField]
	private GameObject GORocket;

	public GameObject GetLaser()
	{
		return GOLaser;
	}
		
	public GameObject GetRocket()
	{
		return GORocket;
	}
}
