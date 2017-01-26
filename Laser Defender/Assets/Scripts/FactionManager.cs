using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionManager : MonoBehaviour 
{
	public GameObject[] Factions;
	// Use this for initialization
	void Start () 
	{
		EnemyScript enemy = Factions[0].transform.GetChild(0).GetChild(0).gameObject.GetComponent<EnemyScript>();
		if(enemy)
		{
			Debug.Log("Found");
		} 
		else
		{
			Debug.Log("Not Found");
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
