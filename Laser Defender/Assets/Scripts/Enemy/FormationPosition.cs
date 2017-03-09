using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationPosition : MonoBehaviour 
{
	public GameObject EnemyToSpawn;

	private bool AllowChild = true;

	void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, 1);
	}

	void Update()
	{
		if(transform.childCount > 0 && AllowChild)
		{
			AllowChild = false;
			Invoke("AllowChildTrue", 10);
		}
	}

	public bool GetAllowChild()
	{
		return AllowChild;
	}

	void AllowChildTrue()
	{
		AllowChild = true;
	}
}
