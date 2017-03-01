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

	private EnemySpawner_New EnemySpawner;

	// Use this for initialization
	void Start () 
	{
		ActiveLevelIndex = PlayerPrefs.GetInt("Level", 0);
		AllLevelsGO = GameObject.Find("Levels");
		EnemySpawner = EnemySpawnerGO.GetComponent<EnemySpawner_New>();
		LoadActiveLevel();
	}
	
	public void ChangeActiveLevel()
	{
		ActiveLevelIndex++;
		LoadActiveLevel();
	}

	private void LoadActiveLevel()
	{
		ActiveLevelGO = AllLevelsGO.transform.GetChild(ActiveLevelIndex).gameObject;
		FormationsInLevel = ActiveLevelGO.transform.GetChild(0);
		int enemyCount = CountEnemiesInFormation();
		InvokeRepeating("SpawnFormations", 0, 5);
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

	private int CountEnemiesInFormation()
	{
		int count = 0;
		foreach(Transform formation in FormationsInLevel)
		{
			foreach(Transform subformation in formation)
				count += subformation.childCount;
		}
		return count;
	}
}
