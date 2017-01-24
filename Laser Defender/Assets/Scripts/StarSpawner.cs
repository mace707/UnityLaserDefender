using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSpawner : MonoBehaviour 
{
	private bool MovingRight = true;

	float XMin = -5;
	float XMax = 5;

	bool InvokeStar = true;

	public GameObject ShootingStar;

	void Start()
	{
		float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
		XMin = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distanceToCamera)).x ;
		XMax = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, distanceToCamera)).x;
	}

	void Update()
	{
		if (MovingRight) transform.position += Vector3.right * 5 * Time.deltaTime;
		else transform.position += Vector3.left * 5 * Time.deltaTime;

		if (transform.position.x < XMin) MovingRight = true;
		else if (transform.position.x > XMax) MovingRight = false;

		if(InvokeStar)
		{
			InvokeRepeating("SpawnStar", 2f, 2f);
			InvokeStar = false;
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere (transform.position, 1);
	}

	void SpawnStar()
	{
		GameObject beam = (GameObject)Instantiate (ShootingStar, transform.position, Quaternion.identity);
		beam.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -20);
	}

}
