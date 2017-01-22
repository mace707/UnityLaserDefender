using UnityEngine;
using System.Collections;

public class Position : MonoBehaviour 
{
	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere (transform.position, 1);
	}
}
