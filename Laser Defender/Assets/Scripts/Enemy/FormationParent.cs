using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationParent : MonoBehaviour 
{
	public int FormationCount = 0;
	// Use this for initialization
	void Start () 
	{
		foreach(Transform SubFormation in transform)
			FormationCount += SubFormation.childCount;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
