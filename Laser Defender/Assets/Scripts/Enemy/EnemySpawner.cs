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

	private bool EnemySpawningActive;

	public Transform PauseCanvas;

	private bool StartSpawningNextWave;

	public Player Player;

	EnemyCountText mSpawnCounter;

	public GameObject MenuHandlerGO;
	private InGameMenuHandler MenuHandler;

	public GameObject[] Factions;

	bool BossRound = false;

	[SerializeField]
	GameObject BossFormations;

	private Transform BossSubFormationTransform;

	public GameObject CountDownToBossRound;

	public GameObject BossRoundCountDown;

	private int Timer = 0;

	private Text CountDownTimerText;

	// Use this for initialization
	void Start () 
	{
		//Distance between the camera and the object.
		MenuHandler = MenuHandlerGO.GetComponent<InGameMenuHandler>();
		StartSpawningNextWave = true;
		EnemySpawningActive = false;
	//	EnemySpawnIndex = -1;
		FactionSpawnIndex = 0;
		EnemySpawnCountMax = 0;
		EnemySpawnCount = 0;
		mSpawnCounter = GameObject.Find(StringConstants.TEXTSpawnCount).GetComponent<EnemyCountText>();
		ScreenSetup();
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
			if(!BossRound)
				SpawnEnemies();
		}
			
		if(EnemySpawningActive && transform.childCount == 0 && EnemySpawnCount == EnemySpawnCountMax)
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

		BossRound = Enemy.IsBoss;

		EnemySpawnCountMax = Enemy.SpawnCount;
		EnemySpawnCount = 0;

		mSpawnCounter.SetMax(EnemySpawnCountMax);

		if(BossRound)
		{
			StartSpawningBosses();
		}
	}

	void SpawnEnemies()
	{
		Vector3 topLeftSpawn = TopLeft;
		Vector3 topRightSpawn = TopRight;
		for(int i = 0; i < EnemySpawnCountMax; i += 2)
		{
			GameObject enemyGOLeft = (GameObject)Instantiate(EnemyGO, topLeftSpawn, Quaternion.identity);
			GameObject enemyGORight = (GameObject)Instantiate(EnemyGO, topRightSpawn, Quaternion.identity);

			topLeftSpawn -= new Vector3(enemyGOLeft.GetComponent<Collider2D>().bounds.size.x, 0, 0);
			topRightSpawn += new Vector3(enemyGORight.GetComponent<Collider2D>().bounds.size.x, 0, 0);

			Enemy esl = enemyGOLeft.GetComponent<Enemy>();
			Enemy esr = enemyGORight.GetComponent<Enemy>();

			esl.LeftToCenter = true;
			esr.LeftToCenter = false;

			enemyGOLeft.transform.SetParent(this.transform);
			enemyGORight.transform.SetParent(this.transform);

			EnemySpawnCount += 2;
		}

		Invoke("BeginSpawnTracker", 1.5f);
	}

	void PostSpawnEnemy()
	{
		MenuHandler.ActivatePauseMenu();
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


	private void StartSpawningBosses()
	{
		GlobalConstants.FreezeAllNoTimeScale = true;
		BossRoundCountDown.SetActive(true);
		BossSubFormationTransform = BossFormations.transform.GetChild(FactionSpawnIndex).transform;
		SpawnBossUntilFull();
	}

	private void SpawnBossUntilFull()
	{
		Transform freePosition = NextFreePosition();

		if(freePosition)
		{
			GameObject enemyGO = (GameObject)Instantiate(EnemyGO, freePosition.position , Quaternion.identity);
			enemyGO.transform.SetParent(freePosition);
			EnemySpawnCount++;
		}

		if(NextFreePosition())
			Invoke("SpawnBossUntilFull", 1);
		else
		{
			Invoke("StartTimer", 3);
			BeginSpawnTracker();
		}
	}

	void StartTimer()
	{
		CountDownTimerText = CountDownToBossRound.GetComponent<Text>();
		Timer = 3;
		CountDownTimerText.text = Timer.ToString();
		InvokeRepeating("UpdateTimer", 1, 1);
	}

	public void UpdateTimer()
	{
		Timer--;
		if(Timer <= 0)
		{
			BossRoundCountDown.SetActive(false);
			CancelInvoke("UpdateCounter");
			GlobalConstants.FreezeAllNoTimeScale = false;
		}
		CountDownTimerText.text = Timer.ToString();
	}

	private Transform NextFreePosition()
	{
		foreach(Transform subFormation in BossSubFormationTransform)
		{
			foreach(Transform child in subFormation)
			{
				if(child.childCount == 0)
					return child;
			}
		}
		return null;
	}

	private bool AllBossesDead()
	{
		foreach(Transform subFormation in BossSubFormationTransform)
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
