using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawnerTemp : MonoBehaviour 
{

	[SerializeField]
	public float Width = 10f;
	[SerializeField]
	public float Height = 5f;
	[SerializeField]
	public GameObject[] Bosses;


	public float SpawnDelay = 0.5f;

	private bool MovingRight = true;

	float XMin = -5;
	float XMax = 5;

	[SerializeField]
	float Speed = 5.0f;

	// Use this for initialization
	void Start () 
	{
		SpawnUntilFull();
		//Distance between the camera and the object.
		float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
		XMin = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distanceToCamera)).x ;
		XMax = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, distanceToCamera)).x;
	}
	
	// Update is called once per frame
	void Update () 
	{
		HandleMovement();
	}

	public void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(Width, Height));
	}


	void HandleMovement()
	{
		if (MovingRight) transform.position += Vector3.right * Speed * Time.deltaTime;
		else transform.position += Vector3.left * Speed * Time.deltaTime;

		if (transform.position.x < XMin + Width / 2) MovingRight = true;
		else if (transform.position.x > XMax - Width / 2) MovingRight = false;
	}

	bool AllMemeberDead()
	{
		foreach(Transform child in transform)
		{
			if(child.childCount > 0)
				return false;
		}
		return true;
	}

	void SpawnUntilFull()
	{
		Transform freePosition = NextFreePosition();

		if(freePosition)
		{
			GameObject enemy =	Bosses[0];
			GameObject spawnedEnemy = (GameObject)Instantiate(enemy, freePosition.position, Quaternion.identity);
			spawnedEnemy.transform.SetParent(freePosition);
		}

		if (NextFreePosition())
			Invoke("SpawnUntilFull", SpawnDelay);
	}

	Transform NextFreePosition()
	{
		foreach(Transform child in transform)
		{
			if(child.childCount == 0)
				return child;
		}
		return null;
	}
}
