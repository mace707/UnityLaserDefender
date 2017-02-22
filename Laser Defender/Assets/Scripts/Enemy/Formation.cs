using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation : MonoBehaviour 
{
	[SerializeField]
	public float Width = 0;
	[SerializeField]
	public float Height = 0;

	public bool FreezeMovement = true;
	public bool FreezeMovementX = true;
	public bool FreezeMovementY = true;
	public bool FreezeRotation = true;

	public bool MovingRight = false;

	public bool MovingDown = false;

	private float XMin = 0;
	private float XMax = 0;

	private float YMin = 0;
	private float YMax = 0;

	private Quaternion qForward = Quaternion.Euler(0.0f, 0.0f, 0.0f);
	private Quaternion qBack = Quaternion.Euler(0.0f, 0.0f, 90f);

	private int LeftBounceCount = 0;
	private int RightBounceCount = 0;

	public int MoveDownAfterLeftCountIs = 0;
	public int MoveDownAfterRightCountIs = 0;

	private bool MoveDownIntentionally = false;

	private float CurrentYValue = 0;

	public bool MaxXIsCenter = false;
	public bool MinXIsCenter = false;

	void Start()
	{
		float distanceToCamera = transform.position.z - Camera.main.transform.position.z;

		float maxXVal = MaxXIsCenter ? 0.5f : 1;
		float minXVal = MinXIsCenter ? 0.5f : 0;

		XMin = Camera.main.ViewportToWorldPoint(new Vector3(minXVal, 0, distanceToCamera)).x;
		XMax = Camera.main.ViewportToWorldPoint(new Vector3(maxXVal, 0, distanceToCamera)).x;
		YMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.35f, distanceToCamera)).y;
		YMax = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, distanceToCamera)).y;
	}

	void Update()
	{
		if(GlobalConstants.FreezeAllNoTimeScale)
			return;

		if(!FreezeRotation)
		{
			//RotateFormationToPoint();
			RotateFormationConstantly();
		}

		if (!FreezeMovement)
		{
			if(!FreezeMovementX)
			{
				if(MovingRight)		transform.position += Vector3.right * 5 * Time.deltaTime;
				else				transform.position += Vector3.left * 5 * Time.deltaTime;

				if(transform.position.x < XMin + Width / 2)
				{
					MovingRight = true;
					LeftBounceCount++;
				}
				else if(transform.position.x > XMax - Width / 2)
				{
					MovingRight = false;
					RightBounceCount++;
				}

				if(RightBounceCount >= MoveDownAfterRightCountIs || LeftBounceCount >= MoveDownAfterLeftCountIs)
				{
					MoveDownIntentionally = true;
					FreezeMovementX = true;
					CurrentYValue = 0;
				}
			}

			if(!FreezeMovementY)
			{
				if(MovingDown)		transform.position += Vector3.down * 5 * Time.deltaTime;
				else				transform.position += Vector3.up * 5 * Time.deltaTime;

				if(transform.position.y < YMin + Height / 2)		MovingDown = false;
				else if(transform.position.y > YMax - Height / 2)	MovingDown = true;
			}

			if(MoveDownIntentionally)
			{
				MoveDown();
			}
		}
	}

	void MoveDown()
	{
		Vector3 vertVal = Vector3.down * Time.deltaTime * 2;
		transform.position += vertVal;
		CurrentYValue += vertVal.y;

		if(Mathf.Abs(CurrentYValue) >= 2)
		{
			MoveDownIntentionally = false;
			FreezeMovementX = false;
			RightBounceCount = 0;
			LeftBounceCount = 0;
		}
	}

	void RotateFormationToPoint()
	{
		transform.rotation = Quaternion.RotateTowards(transform.rotation, qBack, Time.deltaTime * 20.0f);
		CounterRotateChildrenToPoint();
	}

	private void CounterRotateChildrenToPoint()
	{
		foreach(Transform child in transform)
			child.rotation = Quaternion.RotateTowards(child.rotation, qForward, Time.deltaTime * 20.0f);
	}

	void RotateFormationConstantly()
	{
		transform.RotateAround(transform.position, new Vector3(0,0,1), Time.deltaTime * 20.0f);
		CounterRotateChildrenConstantly();
	}

	private void CounterRotateChildrenConstantly()
	{
		foreach(Transform child in transform)
			child.transform.RotateAround(child.transform.position, new Vector3(0,0,-1), Time.deltaTime * 20.0f);
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(Width, Height));
	}
}

