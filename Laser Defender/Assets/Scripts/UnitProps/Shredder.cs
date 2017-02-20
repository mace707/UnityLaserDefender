using UnityEngine;
using System.Collections;

public class Shredder : MonoBehaviour 
{
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "PSP")
			col.gameObject.GetComponent<Animator>().SetBool ("IsTravelling", false);
		else
			Destroy (col.gameObject);
	}
}
