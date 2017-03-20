﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
	public void OnTriggerEnter2D(Collider2D col)
	{
		Projectile laser = col.gameObject.GetComponent<Projectile>();
		if(laser)
			laser.Hit();
	}
}
