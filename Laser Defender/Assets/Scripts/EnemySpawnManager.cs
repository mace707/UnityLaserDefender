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
	public int FactionIndex = 0;
	public int EnemyIndex = 0;
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

		EnemyScript.TravelPath path = enemy.GetComponent<EnemyScript>().Path;

		if(path == EnemyScript.TravelPath.SidesToCenter)
		{
			GameObject enemyLeft = (GameObject)Instantiate(enemy, TopLeft, Quaternion.identity);
			GameObject enemyRight = (GameObject)Instantiate(enemy, TopRight, Quaternion.identity);

			EnemyScript esLeft = enemyLeft.GetComponent<EnemyScript>();
			EnemyScript esRight = enemyRight.GetComponent<EnemyScript>();

			esLeft.MovingRight = true;
			esLeft.Path = EnemyScript.TravelPath.LeftToCenter;

			esRight.MovingRight = false;
			esRight.Path = EnemyScript.TravelPath.RightToCenter;

		}
		else if (path == EnemyScript.TravelPath.LeftToRight)
		{
			GameObject enemyLeft = (GameObject)Instantiate(enemy, TopLeft, Quaternion.identity);
			enemyLeft.GetComponent<EnemyScript>().MovingRight = true;
		}
		else if (path == EnemyScript.TravelPath.RightToLeft)
		{
			GameObject enemyRight = (GameObject)Instantiate(enemy, TopRight, Quaternion.identity);
			enemyRight.GetComponent<EnemyScript>().MovingRight = false;
		}

		if(SpawnCount > 5)
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
