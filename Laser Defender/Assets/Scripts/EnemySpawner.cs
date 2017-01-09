using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour 
{
	[SerializeField]
	public GameObject EnemyPrefab;
	[SerializeField]
	public float Width = 10f;
	[SerializeField]
	public float Height = 5f;

	private bool MovingRight = true;

	float XMin = -5;
	float XMax = 5;

	[SerializeField]
	float Speed = 5.0f;

	// Use this for initialization
	void Start () 
	{
		foreach (Transform child in transform) 
		{
			GameObject enemy = (GameObject)Instantiate(EnemyPrefab, child.transform.position, Quaternion.identity);
			enemy.transform.SetParent(child);
		}

		//Distance between the camera and the object.
		float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
		XMin = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distanceToCamera)).x ;
		XMax = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, distanceToCamera)).x;

	}

	public void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(Width, Height));
	}

	// Update is called once per frame
	void Update () 
	{
		
		if (MovingRight) transform.position += Vector3.right * Speed * Time.deltaTime;
		else transform.position += Vector3.left * Speed * Time.deltaTime;

		if (transform.position.x < XMin + Width / 2)
			MovingRight = true;
		else if (transform.position.x > XMax - Width / 2)
			MovingRight = false;
	}
}
