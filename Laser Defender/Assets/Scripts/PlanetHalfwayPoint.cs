﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetHalfwayPoint : MonoBehaviour 
{
	public float SpawnProbability;

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.gameObject.tag == "PSP")
		{	
			PlanetSpawner ps = col.gameObject.transform.parent.GetComponent<PlanetSpawner>();

			float val = Random.value;
			Debug.Log(val);
			if(ps && val <= SpawnProbability)
				ps.SpawnPlanet();
		}
	}
}
