using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropShield : MonoBehaviour 
{
	public void Hit()
	{
		Destroy(gameObject);
	}
}
