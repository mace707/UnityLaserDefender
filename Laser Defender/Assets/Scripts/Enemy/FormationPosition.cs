using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationPosition : MonoBehaviour 
{
	public GameObject EnemyToSpawn;

	void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, 1);
	}
}
