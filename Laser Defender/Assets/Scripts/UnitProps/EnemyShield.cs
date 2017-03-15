using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : Shield 
{
	public override void ActivateShield(Transform parent)
	{
		GOActiveShield = Instantiate(gameObject, parent.position, Quaternion.identity);
		GOActiveShield.transform.SetParent(parent);
	}

	public override void DeactivateShield()
	{
		Destroy(GOActiveShield);
	}		
}
