using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon 
{
	public abstract void Fire(GameObject goTarget);
	public abstract void Fire(Vector3 begin, Vector3 dir);

	protected GameObject GOProjectile;
}