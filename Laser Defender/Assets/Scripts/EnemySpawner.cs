using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour 
{
	[SerializeField]
	public GameObject[] EnemyPrefabs;

	[SerializeField]
	public GameObject[] BossPrefabs;

	[SerializeField]
	public float Width = 10f;
	[SerializeField]
	public float Height = 5f;

	public float SpawnDelay = 0.5f;

	private bool MovingRight = true;

	int EnemyPrefabIndex = 0;

	float XMin = -5;
	float XMax = 5;

	[SerializeField]
	float Speed = 5.0f;

	int EnemyFormationIndex = 0;

	// Use this for initialization
	void Start () 
	{
		SpawnUntilFull();
		//Distance between the camera and the object.
		float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
		XMin = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distanceToCamera)).x ;
		XMax = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, distanceToCamera)).x;

	}

	void SpawnBoss()
	{
		Transform freePosition = NextFreePosition();
		GameObject enemy = (GameObject)Instantiate(BossPrefabs[0], freePosition.position, Quaternion.identity);
		enemy.transform.SetParent(freePosition);
	}

	void SpawnUntilFull()
	{
		Transform freePosition = NextFreePosition();
		if(freePosition)
		{
			GameObject enemy = (GameObject)Instantiate(EnemyPrefabs[EnemyPrefabIndex], freePosition.position, Quaternion.identity);
			enemy.transform.SetParent(freePosition);
		}
		if (NextFreePosition())
			Invoke("SpawnUntilFull", SpawnDelay);
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

		if (transform.position.x < XMin + Width / 2) MovingRight = true;
		else if (transform.position.x > XMax - Width / 2) MovingRight = false;

		if (AllMemeberDead())
		{
			EnemyPrefabIndex += 1;
			if(EnemyPrefabIndex < EnemyPrefabs.Length)
			{
				EnemyFormationIndex = Random.Range(0, transform.childCount - 1);
				SpawnUntilFull();
			}
			else
			{
				EnemyFormationIndex = transform.childCount - 1;
				SpawnBoss();
			}
		}
	}

	Transform NextFreePosition()
	{
		foreach(Transform childPositionGameObj in transform.GetChild(EnemyFormationIndex).transform)
		{
			if(childPositionGameObj.childCount == 0)
				return childPositionGameObj;
		}
		return null;
	}

	bool AllMemeberDead()
	{
		foreach(Transform childPositionGameObj in transform.GetChild(EnemyFormationIndex).transform)
		{
			if(childPositionGameObj.childCount > 0)
				return false;
		}
		return true;
	}
}
