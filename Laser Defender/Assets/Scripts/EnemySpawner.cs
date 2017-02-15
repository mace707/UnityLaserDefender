using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour 
{
	public int FactionSpawnIndex;
	public int EnemySpawnIndex;

	private Enemy Enemy;
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

	public Player Player;

	EnemyCountText mSpawnCounter;

	public GameObject MenuHandlerGO;
	private InGameMenuHandler MenuHandler;

	public GameObject[] Factions;

	// Use this for initialization
	void Start () 
	{
		//Distance between the camera and the object.
		MenuHandler = MenuHandlerGO.GetComponent<InGameMenuHandler>();
		StartSpawningNextWave = true;
		EnemySpawningActive = false;
		EnemySpawnBothSides = false;
		EnemySpawnIndex = -1;
		FactionSpawnIndex = 0;
		EnemySpawnCountMax = 0;
		EnemySpawnCount = 0;
		mSpawnCounter = GameObject.Find(StringConstants.TEXTSpawnCount).GetComponent<EnemyCountText>();
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

		EnemyGO = Factions[FactionSpawnIndex].transform.GetChild(EnemySpawnIndex).gameObject;
		Enemy = EnemyGO.GetComponent<Enemy>();

		EnemySpawnCountMax = Enemy.SpawnCount;
		EnemySpawnCount = 0;

		mSpawnCounter.SetMax(EnemySpawnCountMax);

		switch(Enemy.Path)
		{
		case Enemy.TravelPath.LeftToCenter:
		case Enemy.TravelPath.LeftToRight:
			EnemySpawnPosition = TopLeft;
			EnemySpawnBothSides = false;
			break;
		case Enemy.TravelPath.RightToCenter:
		case Enemy.TravelPath.RightToLeft:
			EnemySpawnPosition = TopRight;
			EnemySpawnBothSides = false;
			break;
		case Enemy.TravelPath.SidesToCenter:
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

				Enemy esl = enemyGOLeft.GetComponent<Enemy>();
				Enemy esr = enemyGORight.GetComponent<Enemy>();

				esl.Path = Enemy.TravelPath.LeftToCenter;
				esr.Path = Enemy.TravelPath.RightToCenter;

				enemyGOLeft.transform.SetParent(this.transform);
				enemyGORight.transform.SetParent(this.transform);

				EnemySpawnCount += 2;
			}
		}
	}

	void PostSpawnEnemy()
	{
		MenuHandler.ActivatePauseMenu();
		CancelInvoke ("SpawnEnemy");
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
		Player.ResetValues();
		Player.UpdateHealthBar();
		Player.UpdateShieldPointBar();
		MenuHandler.DeactivatePauseMenu();
		StartSpawningNextWave = true;
	}
}
