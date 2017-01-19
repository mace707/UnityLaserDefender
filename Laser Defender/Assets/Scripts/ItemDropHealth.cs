using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropHealth : MonoBehaviour 
{
	public void Hit()
	{
		Destroy(gameObject);
	}
}
