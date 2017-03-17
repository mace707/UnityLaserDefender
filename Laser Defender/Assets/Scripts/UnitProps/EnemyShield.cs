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

	public bool IsActive()
	{
		return GOActiveShield != null;
	}

	public void OnTriggerEnter2D(Collider2D col)
	{
		Projectile laser = col.gameObject.GetComponent<Projectile>();
		if(laser)
			laser.Hit();
	}
}
