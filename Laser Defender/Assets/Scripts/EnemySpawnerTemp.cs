using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawnerTemp : MonoBehaviour 
{
	public FactionManager mFactionMgr;

	[SerializeField]
	public int FactionSpawnIndex;
	[SerializeField]
	public int EnemySpawnIndex;

	private EnemyScript Enemy;
	private GameObject EnemyGO;

	private int EnemySpawnCountMax;
	private int EnemySpawnCount;

	private Vector3 TopLeft;
	private Vector3 TopRight;
	private Vector3 EnemySpawnPosition;

	private bool EnemySpawningActive;

	public Transform PauseCanvas;

	private bool EnemySpawnBothSides;

	private bool StartSpawningNextWave;

	SpawnCounter mSpawnCounter;
	// Use this for initialization
	void Start () 
	{
		StartSpawningNextWave = true;
		EnemySpawningActive = false;
		EnemySpawnBothSides = false;
		EnemySpawnIndex = -1;
//		FactionSpawnIndex = 0;
		EnemySpawnCountMax = 0;
		EnemySpawnCount = 0;
		mSpawnCounter = GameObject.Find("SpawnCountText").GetComponent<SpawnCounter>();
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

		EnemyGO = mFactionMgr.Factions[FactionSpawnIndex].transform.GetChild(0).GetChild(EnemySpawnIndex).gameObject;
		Enemy = EnemyGO.GetComponent<EnemyScript>();

		EnemySpawnCountMax = Enemy.SpawnCount;
		EnemySpawnCount = 0;

		mSpawnCounter.SetMax(EnemySpawnCountMax);

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
				EnemyScript esr = enemyGORight.GetComponent<EnemyScript>();

				esl.Path = EnemyScript.TravelPath.LeftToCenter;
				esr.Path = EnemyScript.TravelPath.RightToCenter;

				enemyGOLeft.transform.SetParent(this.transform);
				enemyGORight.transform.SetParent(this.transform);

				EnemySpawnCount += 2;
			}
		}
	}

	void PostSpawnEnemy()
	{
		if(!PauseCanvas.gameObject.activeInHierarchy)
		{
			PauseCanvas.gameObject.SetActive(true);
			CancelInvoke ("SpawnEnemy");
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
		PauseCanvas.gameObject.SetActive(false);
		Time.timeScale = 1;
		StartSpawningNextWave = true;
	}
}
