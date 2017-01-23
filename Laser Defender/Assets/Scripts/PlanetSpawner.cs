using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSpawner : MonoBehaviour 
{

	public GameObject[] Planets;

	// Use this for initialization
	void Start () 
	{
		SpawnPlanet();
	}

	void Update()
	{
		CheckForPlanets();
	}

	void CheckForPlanets()
	{
		foreach(Transform child in transform)
		{
			if(child.childCount != 0)
				return;
		}
		SpawnPlanet();
	}

	public void SpawnPlanet()
	{
		int livePlanets = 0;

		foreach(Transform child in transform)
			livePlanets += child.childCount;

		if(livePlanets == transform.childCount)
			return;

		int randChildIndex = Random.Range(0, transform.childCount);
		int randPlanetIndex = Random.Range(0, Planets.Length);

		if(transform.GetChild(randChildIndex).childCount == 0)
		{
			Animator mAnimator = transform.GetChild(randChildIndex).GetComponent<Animator>();

			mAnimator.SetBool("IsTravelling", true);

			Transform childPosition = transform.GetChild(randChildIndex).transform;
			GameObject spawnedPlanet = (GameObject)Instantiate(Planets[randPlanetIndex], childPosition.position, Quaternion.identity);
			spawnedPlanet.transform.SetParent(childPosition);
		}
		else
			SpawnPlanet();
	}
}
