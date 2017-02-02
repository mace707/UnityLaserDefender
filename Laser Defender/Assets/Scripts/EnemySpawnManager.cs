using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawnManager : MonoBehaviour 
{
	[SerializeField]
	public FactionManager Factions;

	Vector3 TopLeft, TopRight;

	bool EnemiesSpawned = false;

	int SpawnCount = 0;
	public int FactionIndex = 0;
	public int EnemyIndex = 0;

	bool SpawnWave = true;

	bool TrackSpawnedChildren = false;

	/// <Menu>
	// Move to its own source file and then add objects as necessary.
	public Transform Canvas;
	/// </Menu>

	public void EndOfWave()
	{
		if(!Canvas.gameObject.activeInHierarchy)
		{
			Canvas.gameObject.SetActive(true);
			Time.timeScale = 0;
		}
	}

	public void SpawnNextWave()
	{
		Canvas.gameObject.SetActive(false);
		Time.timeScale = 1;
		EnemyIndex++;
		SpawnWave = true;
	}

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

		if(TrackSpawnedChildren && transform.childCount == 0)
		{
			TrackSpawnedChildren = false;
			Invoke("EndOfWave", 5);
		}
	}

	void SpawnNewShip()
	{
		if(!SpawnWave)
			return;

		Transform factionTransform = Factions.Factions[FactionIndex].transform;
		GameObject enemy =	factionTransform.GetChild(0).GetChild(EnemyIndex).gameObject;

		EnemyScript.TravelPath path = enemy.GetComponent<EnemyScript>().Path;

		if(path == EnemyScript.TravelPath.SidesToCenter)
		{
			GameObject instantiatedEnemyLeft = (GameObject)Instantiate(enemy, TopLeft, Quaternion.identity);
			GameObject instantiatedEnemyRight = (GameObject)Instantiate(enemy, TopRight, Quaternion.identity);

			EnemyScript esLeft = instantiatedEnemyLeft.GetComponent<EnemyScript>();
			EnemyScript esRight = instantiatedEnemyRight.GetComponent<EnemyScript>();

			esLeft.MovingRight = true;
			esLeft.Path = EnemyScript.TravelPath.LeftToCenter;

			esRight.MovingRight = false;
			esRight.Path = EnemyScript.TravelPath.RightToCenter;

			esRight.transform.SetParent(this.transform);
			esLeft.transform.SetParent(this.transform);

			SpawnCount += 2;

			if(SpawnCount >= esRight.SpawnCount)
			{
				SpawnWave = false;
				SpawnCount = 0;
			}
		}
		else if(path == EnemyScript.TravelPath.LeftToRight)
		{
			GameObject instantiatedEnemy = (GameObject)Instantiate(enemy, TopLeft, Quaternion.identity);
			EnemyScript es = instantiatedEnemy.GetComponent<EnemyScript>();
			es.MovingRight = true;
			es.transform.SetParent(this.transform);

			SpawnCount++;

			if(SpawnCount >= es.SpawnCount)
			{
				SpawnWave = false;
				SpawnCount = 0;
			}

		}
		else if(path == EnemyScript.TravelPath.RightToLeft)
		{
			GameObject instantiatedEnemy = (GameObject)Instantiate(enemy, TopRight, Quaternion.identity);
			EnemyScript es = instantiatedEnemy.GetComponent<EnemyScript>();
			es.MovingRight = false;
			es.transform.SetParent(this.transform);

			SpawnCount++;

			if(SpawnCount >= es.SpawnCount)
			{
				SpawnWave = false;
				SpawnCount = 0;
			}
		}

		TrackSpawnedChildren = true;
	}
}
