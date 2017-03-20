using System.Collections;

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

	private EnemySpawner EnemySpawner;

	private int EnemiesAliveInLevel;

	[SerializeField] private GameObject MenuHandlerGO;
	private InGameMenuHandler MenuHandler;

	private bool MenuActive = false;
	// Use this for initialization
	void Start () 
	{
		MenuHandler = MenuHandlerGO.GetComponent<InGameMenuHandler>();
		ActiveLevelIndex = PlayerPrefs.GetInt("Level", 0);
		AllLevelsGO = GameObject.Find("Levels");
		EnemySpawner = EnemySpawnerGO.GetComponent<EnemySpawner>();
		MenuHandler.ActivateCoundDownTimer();
		LoadActiveLevel();
	}

	void Update()
	{
		// Rather do this in the CheckComplete function... Just check if we have reached the end of our waves in this level.
		if(AllEnemiedInLevelKilled() && !MenuActive)
		{
			MenuActive = true;
			Invoke("ActivatePauseMenu", 5);
		}
	}

	// This will be called via a menu item.
	public void ChangeActiveLevel()
	{
		ActiveLevelIndex++;
		MenuHandler.ActivateCoundDownTimer();
		LoadActiveLevel();
		MenuActive = false;
	}

	private void LoadActiveLevel()
	{
		ActiveLevelGO = AllLevelsGO.transform.GetChild(ActiveLevelIndex).gameObject;
		FormationsInLevel = ActiveLevelGO.transform.GetChild(0);
		EnemiesAliveInLevel = CountEnemiesInAllFormations(FormationsInLevel);
		FormationIndex = 0;
		SpawnFormations();
	}

	private void SpawnFormations(bool delayed = false)
	{
		if(FormationIndex >= FormationsInLevel.childCount)
		{
			CancelInvoke("SpawnFormations");
			return;
		}

		Transform activeFormation = FormationsInLevel.GetChild(FormationIndex);
		activeFormation.gameObject.SetActive(true);
		EnemySpawner.Setup(activeFormation,delayed);
		EnemySpawner.SpawnFormation();
		FormationIndex++;
		InvokeRepeating("WaveDefeatedCheck", 5, 5);
	}

	private int CountEnemiesInAllFormations(Transform formationsInLevel)
	{
		int count = 0;
		foreach(Transform formation in formationsInLevel)
		{
			foreach(Transform subformation in formation)
				count += subformation.childCount;
		}
		return count;
	}

	private bool AllEnemiedInActiveFormationKilled()
	{
		Transform activeFormation = FormationsInLevel.GetChild(FormationIndex - 1);
		foreach(Transform subFormation in activeFormation)
		{
			foreach(Transform position in subFormation)
			{
				if(position.childCount > 0)
					return false;
			}
		}
		return true;
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
		MenuHandler.ActivatePauseMenu(GetUniqueEnemiesInLevel());
	}

	public void DeactivatePauseMenu()
	{
		MenuHandler.ActivateCoundDownTimer();
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
				foreach (Enemy e in subEnemyList)
				{
					bool addEnemy = true;

					foreach(Enemy le in enemyList)
					{
						if(le == e)
						{
							addEnemy = false;
							break;
						}
					}

					if(addEnemy)
						enemyList.Add(e);
				}
			}
		}
		return enemyList;
	}


	private void WaveDefeatedCheck()
	{
		if(AllEnemiedInActiveFormationKilled())
		{
			CancelInvoke("WaveDefeatedCheck");
			SpawnFormations(true);
		}
	}
}