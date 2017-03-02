using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner_New : MonoBehaviour 
{
	public void SpawnFormation(Transform formation)
	{
		Transform freePosition = NextFreePosition(formation);
		if(freePosition)
		{
			GameObject assignedEnemyGO = freePosition.gameObject.GetComponent<FormationPosition>().EnemyToSpawn;
			GameObject instantiatedEnemyGO = (GameObject)Instantiate(assignedEnemyGO, freePosition.position, Quaternion.identity);
			instantiatedEnemyGO.transform.SetParent(freePosition);
		}

		if(NextFreePosition(formation))
			SpawnFormation(formation);
	}

	private Transform NextFreePosition(Transform formation)
	{
		foreach(Transform subFormation in formation)
		{
			foreach(Transform child in subFormation)
			{
				if(child.childCount == 0)
					return child;
			}
		}
		return null;
	}
}
