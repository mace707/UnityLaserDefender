using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalConstants : MonoBehaviour 
{
	// I should put purchasable perks inside here.
	public static bool FreezeAllNoTimeScale = false;

	// The max an min possible probability... better than entering 0 and 1 all over the code.
	public const float ProbabilityMin = 0.0f;
	public const float ProbabilityMax = 1.0f;
}
