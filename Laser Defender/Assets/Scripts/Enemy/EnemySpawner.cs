using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour 
{
	private Transform Formation;
	private bool Delayed;

	public void SpawnFormation()
	{
		Transform freePosition = NextFreePosition(Formation);
		if(freePosition)
		{
			GameObject assignedEnemyGO = freePosition.gameObject.GetComponent<FormationPosition>().EnemyToSpawn;
			GameObject instantiatedEnemyGO = (GameObject)Instantiate(assignedEnemyGO, freePosition.position, Quaternion.identity);
			instantiatedEnemyGO.transform.SetParent(freePosition);
		}

		if(NextFreePosition(Formation))
		{
			if(Delayed)		Invoke("SpawnFormation", 0.2f);
			else			SpawnFormation();
		}
	}

	private Transform NextFreePosition(Transform formation)
	{
		foreach(Transform subFormation in formation)
		{
			foreach(Transform child in subFormation)
			{
				FormationPosition fp = child.gameObject.GetComponent<FormationPosition>();
				if(child.childCount == 0 && fp.GetAllowChild())
					return child;
			}
		}
		return null;
	}

	public void Setup(Transform formation, bool delayed)
	{
		Formation = formation;
		Delayed = delayed;
	}
}
