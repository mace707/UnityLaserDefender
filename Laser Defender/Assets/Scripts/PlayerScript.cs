using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
	[SerializeField]
	float Speed = 15.0f;

	float XMin = -5;
	float XMax = 5;
	float Padding = 0.5f;

	// Use this for initialization
	void Start () 
	{
		//Distance between the camera and the object.
		float distance = transform.position.z - Camera.main.transform.position.z;

		// The vector in brackets is reperesents the screen, where 0 is left most and 
		// 1 is right most with 0.5 being center.
		XMin = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distance)).x + Padding;
		XMax = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, distance)).x - Padding;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Time.Deltatime (Time between frames) makes it frame rate independant. 
		// If a frame takes longer to render, it will move at a higher speed.
		if (Input.GetKey (KeyCode.LeftArrow)) 
			transform.position += Vector3.left * Speed * Time.deltaTime;
		else if (Input.GetKey (KeyCode.RightArrow)) 
			transform.position += Vector3.right * Speed * Time.deltaTime;

		// Restrict the player to the game space.
		float newX = Mathf.Clamp(transform.position.x, XMin, XMax);
		transform.position = new Vector3 (newX, transform.position.y, transform.position.z);

	}
}
