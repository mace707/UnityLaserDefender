using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour 
{
	public int FactionSpawnIndex;
	public int EnemySpawnIndex;

	private GameObject EnemyGO;

	private int EnemySpawnCountMax;
	private int EnemySpawnCount;

	private bool EnemySpawningActive;

	private bool StartSpawningNextWave;

	public Player Player;

	EnemyCountText mSpawnCounter;

	public GameObject MenuHandlerGO;
	private InGameMenuHandler MenuHandler;

	public GameObject[] Factions;

	[SerializeField]
	GameObject Formations;

	private Transform FormationTransform;

	public GameObject CountDownToRound;

	public GameObject CountDown;

	private int Timer = 0;

	private Text CountDownTimerText;

	public bool DelayBetweenSpawns = false;
	// Use this for initialization
	void Start () 
	{
		//Distance between the camera and the object.
		MenuHandler = MenuHandlerGO.GetComponent<InGameMenuHandler>();
		StartSpawningNextWave = true;
		EnemySpawningActive = false;
		EnemySpawnIndex = -1;
		FactionSpawnIndex = 0;
		EnemySpawnCountMax = 0;
		EnemySpawnCount = 0;
		mSpawnCounter = GameObject.Find(StringConstants.TEXTSpawnCount).GetComponent<EnemyCountText>();
	}

	// Update is called once per frame
	void Update ()
	{
		// Add an event for when enemies have died.
		if(GlobalConstants.FreezeAllNoTimeScale)
			return;


		if(StartSpawningNextWave)
		{
			StartSpawningNextWave = false;
			PreSpawnEnemy();
		}
			
		if(EnemySpawningActive && mSpawnCounter.Count == 0)
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

		EnemySpawnCountMax = Formations.transform.GetChild(0).GetComponent<FormationParent>().FormationCount;
		EnemySpawnCount = 0;

		mSpawnCounter.SetMax(EnemySpawnCountMax);

		StartSpawningEnemies();

	}

	void PostSpawnEnemy()
	{
		MenuHandler.ActivatePauseMenu();
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


	private void StartSpawningEnemies()
	{
		GlobalConstants.FreezeAllNoTimeScale = true;
		CountDown.SetActive(true);
		FormationTransform = Formations.transform.GetChild(0).transform; 
		SpawnUntilFull();
	}

	private void SpawnUntilFull()
	{
		Transform freePosition = NextFreePosition();

		if(freePosition)
		{
			GameObject enemyGO = (GameObject)Instantiate(EnemyGO, freePosition.position , Quaternion.identity);
			enemyGO.transform.SetParent(freePosition);
			EnemySpawnCount++;
		}

		if(NextFreePosition())
		{
			if(DelayBetweenSpawns)
				Invoke("SpawnUntilFull", 1);
			else
				SpawnUntilFull();
		}
		else
		{
			Invoke("StartTimer", 3);
			BeginSpawnTracker();
		}
	}

	void StartTimer()
	{
		CountDownTimerText = CountDownToRound.GetComponent<Text>();
		Timer = 3;
		CountDownTimerText.text = Timer.ToString();
		InvokeRepeating("UpdateTimer", 1, 1);
	}

	public void UpdateTimer()
	{
		Timer--;
		if(Timer <= 0)
		{
			CountDown.SetActive(false);
			CancelInvoke("UpdateCounter");
			GlobalConstants.FreezeAllNoTimeScale = false;
		}
		CountDownTimerText.text = Timer.ToString();
	}

	private Transform NextFreePosition()
	{
		foreach(Transform subFormation in FormationTransform)
		{
			foreach(Transform child in subFormation)
			{
				if(child.childCount == 0)
					return child;
			}
		}
		return null;
	}

	// IF All Dead, then reset and Refill the Formation
	private bool AllDeadInFormation()
	{
		foreach(Transform subFormation in FormationTransform)
		{
			foreach(Transform child in subFormation)
			{
				if(child.childCount >= 1)
					return false;
			}
		}
		return true;
	}

}
