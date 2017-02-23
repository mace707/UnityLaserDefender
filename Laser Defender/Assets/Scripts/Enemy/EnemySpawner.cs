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

	private bool WaveFullySpawned;

	private bool BeginNewRound;

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

	private int FormationIndex = -1;

	// Use this for initialization
	void Start () 
	{
		//Distance between the camera and the object.
		MenuHandler = MenuHandlerGO.GetComponent<InGameMenuHandler>();
		EnemySpawnIndex = -1;
		FactionSpawnIndex = 0;
		mSpawnCounter = GameObject.Find(StringConstants.TEXTSpawnCount).GetComponent<EnemyCountText>();
		Reset();
	}

	void Reset()
	{
		BeginNewRound = true;
		WaveFullySpawned = false;
		EnemySpawnCountMax = 0;
		EnemySpawnCount = 0;
		Player.ResetValues();
		Player.UpdateHealthBar();
		Player.UpdateShieldPointBar();
	}

	// Update is called once per frame
	void Update ()
	{
		// Add an event for when enemies have died.
		if(GlobalConstants.FreezeAllNoTimeScale)
			return;

		if(BeginNewRound)
			BeginRound();
			
		if(WaveFullySpawned && mSpawnCounter.Count == 0)
			EndRound();
	}

	void BeginRound()
	{
		GlobalConstants.FreezeAllNoTimeScale = true;
		BeginNewRound = false;
		PreSpawn();
		Spawn();
		WaveFullySpawned = true;
		Invoke("StartTimer", 3);
	}

	void PreSpawn()
	{
		EnemySpawnIndex++;

		if (FormationIndex + 1 < Formations.transform.childCount)
			FormationIndex++;

		if(EnemySpawnIndex > 3)
		{
			EnemySpawnIndex = 0;
			FactionSpawnIndex++;
		}

		EnemyGO = Factions[FactionSpawnIndex].transform.GetChild(EnemySpawnIndex).gameObject;

		EnemySpawnCountMax = Formations.transform.GetChild(FormationIndex).GetComponent<FormationParent>().FormationCount;
		EnemySpawnCount = 0;

		mSpawnCounter.SetMax(EnemySpawnCountMax);

		CountDown.SetActive(true);
		FormationTransform = Formations.transform.GetChild(FormationIndex).transform;
	}

	private void Spawn()
	{
		Transform freePosition = NextFreePosition();

		if(freePosition)
		{
			GameObject enemyGO = (GameObject)Instantiate(EnemyGO, freePosition.position, Quaternion.identity);
			enemyGO.transform.SetParent(freePosition);
			EnemySpawnCount++;
		}

		if(NextFreePosition())
		{
			if(DelayBetweenSpawns)
				Invoke("Spawn", 1);
			else
				Spawn();
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
			CountDownTimerText.text = "GET READY";
			CountDown.SetActive(false);
			CancelInvoke("UpdateTimer");
			GlobalConstants.FreezeAllNoTimeScale = false;
		}
		CountDownTimerText.text = Timer.ToString();
	}

	private Transform NextFreePosition()
	{
		foreach(Transform subFormation in FormationTransform)
		{
			subFormation.gameObject.GetComponent<Formation>().ResetPosition();
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

	void EndRound()
	{
		WaveFullySpawned = false;
		Invoke("PauseForEnd", 5);
	}

	void PauseForEnd()
	{
		MenuHandler.ActivatePauseMenu();
		GlobalConstants.FreezeAllNoTimeScale = true;
	}

	// Menu handle calls
	public void SetStartSpawningNextWave()
	{
		MenuHandler.DeactivatePauseMenu();
		Reset();
		GlobalConstants.FreezeAllNoTimeScale = false;
	}

}
