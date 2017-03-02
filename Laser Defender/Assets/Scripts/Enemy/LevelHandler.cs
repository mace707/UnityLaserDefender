﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHandler : MonoBehaviour 
{
	[SerializeField]
	public int ActiveLevelIndex;

	[SerializeField]
	public GameObject EnemySpawnerGO;

	private GameObject AllLevelsGO;
	private GameObject ActiveLevelGO;

	private Transform FormationsInLevel;

	private int FormationIndex;

	private EnemySpawner_New EnemySpawner;

	private int EnemiesAliveInLevel;

	[SerializeField]
	GameObject MenuHandlerGO;

	private InGameMenuHandler_New MenuHandler;

	// Use this for initialization
	void Start () 
	{
		MenuHandler = MenuHandlerGO.GetComponent<InGameMenuHandler_New>();
		ActiveLevelIndex = PlayerPrefs.GetInt("Level", 0);
		AllLevelsGO = GameObject.Find("Levels");
		EnemySpawner = EnemySpawnerGO.GetComponent<EnemySpawner_New>();
		LoadActiveLevel();
	}

	void Update()
	{
		if(AllEnemiedInLevelKilled())
			Invoke("ActivatePauseMenu", 5);
	}

	// This will be called via a menu item.
	public void ChangeActiveLevel()
	{
		ActiveLevelIndex++;
		LoadActiveLevel();
	}

	private void LoadActiveLevel()
	{
		ActiveLevelGO = AllLevelsGO.transform.GetChild(ActiveLevelIndex).gameObject;
		FormationsInLevel = ActiveLevelGO.transform.GetChild(0);
		EnemiesAliveInLevel = CountEnemiesInFormationsInLevel(FormationsInLevel);
		InvokeRepeating("SpawnFormations", 0, 40);
	}

	private void SpawnFormations()
	{
		if(FormationIndex >= FormationsInLevel.childCount)
		{
			CancelInvoke("SpawnFormations");
			return;
		}

		Transform activeFormation = FormationsInLevel.GetChild(FormationIndex);
		activeFormation.gameObject.SetActive(true);
		EnemySpawner.SpawnFormation(activeFormation);
		FormationIndex++;
	}

	private int CountEnemiesInFormationsInLevel(Transform formationsInLevel)
	{
		int count = 0;
		foreach(Transform formation in formationsInLevel)
		{
			foreach(Transform subformation in formation)
				count += subformation.childCount;
		}
		return count;
	}
	
	private bool AllEnemiedInLevelKilled()
	{
		return EnemiesAliveInLevel <= 0;
	}

	// Called when an enemy is killed.
	public void EnemyKilled()
	{
		EnemiesAliveInLevel--; 
	}

	private void ActivatePauseMenu()
	{
		GlobalConstants.FreezeAllNoTimeScale = true;
		MenuHandler.ActivateMenu(InGameMenuHandler_New.MenuItem.MenuItemPause);
	 	List<Enemy> EnemyList = GetUniqueEnemiesInLevel();
	}

	public List<Enemy> GetUniqueEnemiesInLevel()
	{
		int nextActiveLevel = ActiveLevelIndex + 1;
		GameObject nextActiveLevelGO = AllLevelsGO.transform.GetChild(nextActiveLevel).gameObject;
		Transform formationsInLevel = nextActiveLevelGO.transform.GetChild(0);

		List<Enemy> enemyList = new List<Enemy>();
		foreach(Transform formation in formationsInLevel)
		{
			foreach(Transform subformation in formation)
			{
				List<Enemy> subEnemyList = subformation.gameObject.GetComponent<Formation>().GetUniqueEnemiesInFormation();
				for(int i = 0; i < subEnemyList.Count; i++)
					enemyList.Add(subEnemyList[i]);
			}
		}
		return enemyList;
	}
}
