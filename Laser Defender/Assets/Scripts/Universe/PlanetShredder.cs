using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetShredder : MonoBehaviour 
{
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "PlanetPositionTag")
			col.gameObject.GetComponent<Animator>().SetBool ("IsArriving", false);
		else
			Destroy (col.gameObject);
	}

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void OnDrawGizmos()
	{

	}

}
