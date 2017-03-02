using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuHandler : MonoBehaviour 
{
	[SerializeField]
	GameObject LevelHandlerGo;

	LevelHandler ActiveLevelHandler;

	private List<Enemy> EnemyList;

	private bool AlreadyActivatedByEndOfRound = false;

	void Start ()
	{
		ActiveLevelHandler = LevelHandlerGo.GetComponent<LevelHandler>();
	}
	

	void Update () 
	{
		if(gameObject.activeInHierarchy && !AlreadyActivatedByEndOfRound)
		{
			AlreadyActivatedByEndOfRound = true;
			EnemyList = ActiveLevelHandler.GetUniqueEnemiesInLevel();
		}
	}
}
