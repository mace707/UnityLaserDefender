using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerTemp : MonoBehaviour 
{
	private int FactionSpawnIndex;
	private int EnemySpawnIndex;

	public FactionManager mFactionMgr;

	private EnemyScript Enemy;
	private GameObject EnemyGO;

	private int EnemySpawnCountMax;
	private int EnemySpawnCount;

	private Vector3 TopLeft;
	private Vector3 TopRight;
	private Vector3 EnemySpawnPosition;

	public bool StartSpawningNextWave;
	private bool EnemySpawningActive;

	public Transform Canvas;

	private bool EnemySpawnBothSides;

	// Use this for initialization
	void Start () 
	{
		StartSpawningNextWave = true;
		EnemySpawningActive = false;
		EnemySpawnBothSides = false;
		EnemySpawnIndex = -1;
		FactionSpawnIndex = 0;
		EnemySpawnCountMax = 0;
		EnemySpawnCount = 0;
		ScreenSetup();
	}

	// Update is called once per frame
	void Update () 
	{
		if(StartSpawningNextWave)
		{
			StartSpawningNextWave = false;
			PreSpawnEnemy();
			InvokeRepeating("SpawnEnemy", 1, 1);
			Invoke("BeginSpawnTracker", 1.5f);
		}

		if(EnemySpawningActive && transform.childCount == 0)
		{
			EnemySpawningActive = false;
			Invoke("PostSpawnEnemy", 5);
		}
	}

	void PreSpawnEnemy()
	{
		EnemySpawnIndex++;

		if(EnemySpawnIndex > 3)
		{
			EnemySpawnIndex = 0;
			FactionSpawnIndex++;
		}

		Debug.Log(EnemySpawnIndex);
		Debug.Log(FactionSpawnIndex);

		EnemyGO = mFactionMgr.Factions[FactionSpawnIndex].transform.GetChild(0).GetChild(EnemySpawnIndex).gameObject;
		Enemy = EnemyGO.GetComponent<EnemyScript>();

		EnemySpawnCountMax = Enemy.SpawnCount;
		EnemySpawnCount = 0;

		switch(Enemy.Path)
		{
		case EnemyScript.TravelPath.LeftToCenter:
		case EnemyScript.TravelPath.LeftToRight:
			EnemySpawnPosition = TopLeft;
			EnemySpawnBothSides = false;
			break;
		case EnemyScript.TravelPath.RightToCenter:
		case EnemyScript.TravelPath.RightToLeft:
			EnemySpawnPosition = TopRight;
			EnemySpawnBothSides = false;
			break;
		case EnemyScript.TravelPath.SidesToCenter:
			EnemySpawnBothSides = true;
			break;
		}
	}

	void SpawnEnemy()
	{
		if(EnemySpawnCount < EnemySpawnCountMax)
		{
			if(!EnemySpawnBothSides)
			{
				GameObject enemyGO = (GameObject)Instantiate(EnemyGO, EnemySpawnPosition, Quaternion.identity);
				enemyGO.transform.SetParent(this.transform);
				EnemySpawnCount++;
			}
			else
			{
				GameObject enemyGOLeft = (GameObject)Instantiate(EnemyGO, TopLeft, Quaternion.identity);
				GameObject enemyGORight = (GameObject)Instantiate(EnemyGO, TopRight, Quaternion.identity);

				EnemyScript esl = enemyGOLeft.GetComponent<EnemyScript>();
				EnemyScript esr = enemyGOLeft.GetComponent<EnemyScript>();

				esl.MovingRight = true;
				esl.Path = EnemyScript.TravelPath.LeftToCenter;

				esr.MovingRight = false;
				esr.Path = EnemyScript.TravelPath.RightToCenter;

				enemyGOLeft.transform.SetParent(this.transform);
				enemyGORight.transform.SetParent(this.transform);

				EnemySpawnCount += 2;
			}
		}
	}

	void PostSpawnEnemy()
	{
		if(!Canvas.gameObject.activeInHierarchy)
		{
			Canvas.gameObject.SetActive(true);
			Time.timeScale = 0;
		}
	}

	private void ScreenSetup()
	{
		float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
		TopLeft = Camera.main.ViewportToWorldPoint (new Vector3 (0, 1, distanceToCamera));
		TopRight = Camera.main.ViewportToWorldPoint (new Vector3 (1, 1, distanceToCamera));
		TopLeft += new Vector3(1, -2, 0);
		TopRight += new Vector3(-1, -2, 0);
	}

	private void BeginSpawnTracker()
	{
		EnemySpawningActive = true;
	}

	public void SetStartSpawningNextWave()
	{
		Canvas.gameObject.SetActive(false);
		Time.timeScale = 1;
		StartSpawningNextWave = true;
	}
}
