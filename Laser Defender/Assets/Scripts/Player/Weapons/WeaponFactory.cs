using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFactory 
{
	public enum WeaponType
	{
		WeaponTypeLaserGun,
		WeaponTypeRocketLauncher,
		WeaponTypeScatterGun,
	}

	public static Weapon GetWeapon(WeaponType type)
	{
		switch (type)
		{
		case WeaponType.WeaponTypeLaserGun:			return new LaserGun();
		case WeaponType.WeaponTypeRocketLauncher:	return new RocketLauncher();
		case WeaponType.WeaponTypeScatterGun:		return new ScatterGun();
		default:
			return null;
		}
	}
}
