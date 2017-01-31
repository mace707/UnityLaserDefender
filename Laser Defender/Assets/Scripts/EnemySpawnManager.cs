using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour 
{
	[SerializeField]
	public FactionManager Factions;

	Vector3 TopLeft, TopRight;

	bool EnemiesSpawned = false;

	int SpawnCount = 0;
	int FactionIndex = 0;
	int EnemyIndex = 0;
	// Use this for initialization
	void Start () 
	{
		float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
		TopLeft = Camera.main.ViewportToWorldPoint (new Vector3 (0, 1, distanceToCamera));
		TopRight = Camera.main.ViewportToWorldPoint (new Vector3 (1, 1, distanceToCamera));
		TopLeft += new Vector3(1, -2, 0);
		TopRight += new Vector3(-1, -2, 0);
	}

	// Update is called once per frame
	void Update () 
	{
		if(!EnemiesSpawned)
		{
			InvokeRepeating("SpawnNewShip", 1, 1);
			EnemiesSpawned = true;
		}
	}

	void SpawnNewShip()
	{
		Transform factionTransform = Factions.Factions[FactionIndex].transform;
		GameObject enemy =	factionTransform.GetChild(0).GetChild(EnemyIndex).gameObject;

		EnemyScript.TravelPath path = enemy.GetComponent<EnemyScript>().TravelRoute;

		Vector3 startPos = TopLeft;

		if(path == EnemyScript.TravelPath.BasicRightToLeft || path == EnemyScript.TravelPath.RightToLeft2Down1Up)
			startPos = TopRight;

		Instantiate(enemy, startPos, Quaternion.identity);

		if(SpawnCount > 100)
		{
			EnemyIndex++;
			SpawnCount = 0;
		}
		if(EnemyIndex > 3)
		{
			EnemyIndex = 0;
			FactionIndex++;
		}

		SpawnCount++;
	}
}
