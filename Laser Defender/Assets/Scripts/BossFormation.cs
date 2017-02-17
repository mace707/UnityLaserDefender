using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFormation : MonoBehaviour 
{
	[SerializeField]
	public float Width = 0;
	[SerializeField]
	public float Height = 0;

	public bool FreezeMovement = false;
	public bool FreezeMovementX = false;
	public bool FreezeMovementY = false;

	private bool MovingRight = false;

	private bool MovingDown = false;

	private float XMin = 0;
	private float XMax = 0;

	private float YMin = 0;
	private float YMax = 0;

	void Start()
	{
		float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
		XMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distanceToCamera)).x;
		XMax = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distanceToCamera)).x;
		YMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.35f, distanceToCamera)).y;
		YMax = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, distanceToCamera)).y;
	}

	void Update()
	{
		if(GlobalConstants.FreezeAllNoTimeScale)
			return;

		if (!FreezeMovement)
		{
			if(!FreezeMovementX)
			{
				if(MovingRight)
					transform.position += Vector3.right * 5 * Time.deltaTime;
				else
					transform.position += Vector3.left * 5 * Time.deltaTime;

				if(transform.position.x < XMin + Width / 2)
					MovingRight = true;
				else if(transform.position.x > XMax - Width / 2)
					MovingRight = false;
			}

			if(!FreezeMovementY)
			{
				if(MovingDown)
					transform.position += Vector3.down * 5 * Time.deltaTime;
				else
					transform.position += Vector3.up * 5 * Time.deltaTime;

				if(transform.position.y < YMin + Height / 2)
					MovingDown = false;
				else if(transform.position.y > YMax - Height / 2)
					MovingDown = true;
			}
		}
	}
	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(Width, Height));
	}
}
