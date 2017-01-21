using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSpawner : MonoBehaviour 
{

	public GameObject[] Planets;

	private GameObject SpawnedPlanet;


	// Use this for initialization
	void Start () 
	{
		SpawnPlanet();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!SpawnedPlanet)
		{
			SpawnPlanet();
		}
	}

	void SpawnPlanet()
	{
		int randChildIndex = Random.Range(0, this.transform.childCount-1);
		int randPlanetIndex = Random.Range(0, Planets.Length-1);



		Animator mAnimator = transform.GetChild(randChildIndex).GetComponent<Animator>();
		mAnimator.SetBool("IsTravelling", true);

		Transform childPosition = transform.GetChild(randChildIndex).transform;
		SpawnedPlanet = (GameObject)Instantiate(Planets[randPlanetIndex], childPosition.position, Quaternion.identity);
		SpawnedPlanet.transform.SetParent(childPosition);
	}

}
