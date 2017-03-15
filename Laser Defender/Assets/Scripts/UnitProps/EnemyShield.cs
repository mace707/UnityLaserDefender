using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour, IShield 
{
	[SerializeField] private GameObject GOActiveShield = null;

	public void Activate(Transform parent)
	{
		GOActiveShield = Instantiate(gameObject, parent.position, Quaternion.identity);
		GOActiveShield.transform.SetParent(parent);
	}

	public void Deactivate()
	{
		Destroy(GOActiveShield);
	}		
}
