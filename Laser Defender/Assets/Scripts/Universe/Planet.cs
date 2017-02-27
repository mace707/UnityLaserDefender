using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour 
{
	private float RotateDir;
	private float RotateSpeed;

	// Use this for initialization
	void Start () 
	{
		float value = Random.value;
		RotateDir = value <= 0.5 ? -1 : 1;
		RotateSpeed = Random.Range(5, 10);
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.RotateAround(transform.position, new Vector3(0, 0, RotateDir), Time.deltaTime * RotateSpeed);
	}
}
