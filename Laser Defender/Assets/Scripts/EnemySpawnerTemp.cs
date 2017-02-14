using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawnerTemp : MonoBehaviour 
{
	public FactionManager mFactionMgr;

	public int FactionSpawnIndex;
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

	public PlayerScript Player;

	SpawnCounter mSpawnCounter;

	public GameObject MenuHandlerGO;
	private InGameMenuHandler MenuHandler;

	bool IsBossRound = false;

	float XMin = -5;
	float XMax = 5;

	// Use this for initialization
	void Start () 
	{
		//Distance between the camera and the object.
		float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
		XMin = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distanceToCamera)).x ;
		XMax = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, distanceToCamera)).x;

		MenuHandler = MenuHandlerGO.GetComponent<InGameMenuHandler>();
		StartSpawningNextWave = true;
		EnemySpawningActive = false;
		EnemySpawnBothSides = false;
		EnemySpawnIndex = -1;
		FactionSpawnIndex = 0;
		EnemySpawnCountMax = 0;
		EnemySpawnCount = 0;
		mSpawnCounter = GameObject.Find(StringConstants.TEXTSpawnCount).GetComponent<SpawnCounter>();
		ScreenSetup();
	}

	// Update is called once per frame
	void Update () 
	{
		if(IsBossRound)
		{
			HandleMovement();
		}
		else
		{
			if(StartSpawningNextWave)
			{
				StartSpawningNextWave = false;
				PreSpawnEnemy();
				InvokeRepeating("SpawnEnemy", 1, 1);
				Invoke("BeginSpawnTracker", 1.5f);
			}

			if(EnemySpawningActive && transform.childCount == 5)
			{
				EnemySpawningActive = false;
				Invoke("PostSpawnEnemy", 5);
			}
		}
	}

	void PreSpawnEnemy()
	{
		EnemySpawnIndex++;

		if(EnemySpawnIndex > 2)
		{
			IsBossRound = true;
			SpawnUntilFull();
			return;
		}

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

	[SerializeField]
	public float Width = 10f;
	[SerializeField]
	public float Height = 5f;
	[SerializeField]
	public GameObject[] Bosses;


	public float SpawnDelay = 0.5f;

	private bool MovingRight = true;

	[SerializeField]
	float Speed = 5.0f;

	public void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(Width, Height));
	}


	void HandleMovement()
	{
		if (MovingRight) transform.position += Vector3.right * Speed * Time.deltaTime;
		else transform.position += Vector3.left * Speed * Time.deltaTime;

		if (transform.position.x < XMin + Width / 2) MovingRight = true;
		else if (transform.position.x > XMax - Width / 2) MovingRight = false;
	}

	bool AllMemeberDead()
	{
		foreach(Transform child in transform)
		{
			if(child.childCount > 0)
				return false;
		}
		return true;
	}

	void SpawnUntilFull()
	{
		Transform freePosition = NextFreePosition();

		if(freePosition)
		{
			GameObject enemy =	Bosses[0];
			GameObject spawnedEnemy = (GameObject)Instantiate(enemy, freePosition.position, Quaternion.identity);
			spawnedEnemy.transform.SetParent(freePosition);
		}

		if (NextFreePosition())
			Invoke("SpawnUntilFull", SpawnDelay);
	}

	Transform NextFreePosition()
	{
		foreach(Transform child in transform)
		{
			if(child.childCount == 0)
				return child;
		}
		return null;
	}
}
