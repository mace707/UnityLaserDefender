using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSpawner : MonoBehaviour 
{
	[SerializeField]
	public GameObject MeteorPrefab;

	public GameObject Planet;

	public void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere (transform.position, 1);
	}

	void SpawnPlanet()
	{
		Planet = (GameObject)Instantiate(MeteorPrefab, transform.position, Quaternion.identity);
	}
	// Update is called once per frame
	void Update ()
	{
		if (!Planet)
			SpawnPlanet();
	}
}

