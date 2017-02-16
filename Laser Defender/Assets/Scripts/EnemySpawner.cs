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

	bool BossRound = false;

	[SerializeField]
	GameObject BossFormations;

	private Transform BossFormationTransform;

	public GameObject CountDownToBossRound;

	// Use this for initialization
	void Start () 
	{
		//Distance between the camera and the object.
		MenuHandler = MenuHandlerGO.GetComponent<InGameMenuHandler>();
		StartSpawningNextWave = true;
		EnemySpawningActive = false;
		EnemySpawnBothSides = false;
		//EnemySpawnIndex = -1;
		//FactionSpawnIndex = 0;
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
			if(!BossRound)
			{
				InvokeRepeating("SpawnEnemy", 1, 1);
				Invoke("BeginSpawnTracker", 1.5f);
			}
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

		BossRound = Enemy.IsBoss;

		if(!BossRound)
		{
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
		else
		{
			StartSpawningBosses();
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

	private List<Enemy> SpawnedBosses;
	private BossFormation ActiveFormation;

	private void StartSpawningBosses()
	{
		SpawnedBosses = new List<Enemy>();
		BossFormationTransform = BossFormations.transform.GetChild(FactionSpawnIndex).transform;
		ActiveFormation = BossFormations.transform.GetChild(FactionSpawnIndex).GetComponent<BossFormation>();
		SpawnBossUntilFull();
	}

	private void SpawnBossUntilFull()
	{
		Transform freePosition = NextFreePosition();

		if(freePosition)
		{
			GameObject enemyGO = (GameObject)Instantiate(EnemyGO, EnemySpawnPosition, Quaternion.identity);
			Enemy enemy = enemyGO.GetComponent<Enemy>();
			enemy.Invincible = true;
			SpawnedBosses.Add(enemy);
			enemyGO.transform.SetParent(freePosition);
			EnemySpawnCount++;
		}

		if(NextFreePosition())
			Invoke("SpawnBossUntilFull", 1);
		else
		{
			Invoke("BossRoundReady", 3);
			CountDownToBossRound.SetActive(true);
		}
	}

	void BossRoundReady()
	{
		for(int i = 0; i < SpawnedBosses.Count; i++)
			SpawnedBosses[i].Invincible = false;

		ActiveFormation.FreezeAll = false;
		CountDownToBossRound.GetComponent<CountDownText>().StartCounter();
	}

	private Transform NextFreePosition()
	{
		foreach(Transform child in BossFormationTransform)
		{
			if(child.childCount == 0)
				return child;
		}
		return null;
	}

	public void ShowWaitText()
	{
		CountDownToBossRound.SetActive(true);
		CountDownToBossRound.GetComponent<Text>().text = "WAIT";
	}

	private void UpdateWaitText()
	{
		
	}

	public void ShowCountDownText()
	{
		
	}
}
