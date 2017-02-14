using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenuHandler : MonoBehaviour 
{
	public Transform PauseMenuTransform;
	public Transform PlayerCustomizeMenuTransform;

	void Start () 
	{

	}

	void Update () 
	{
		
	}

	public void DeactivatePauseMenu()
	{
		PauseMenuTransform.gameObject.SetActive(false);
		Time.timeScale = 1;
	}
		
	public void DeactivatePlayerCustomizationMenu()
	{
		PlayerCustomizeMenuTransform.gameObject.SetActive(false);
		Time.timeScale = 1;
	}

	public void ActivatePauseMenu()
	{
		PauseMenuTransform.gameObject.SetActive(true);
		PlayerCustomizeMenuTransform.gameObject.SetActive(false);
		Time.timeScale = 0;
	}
		
	public void ActivatePlayerCustomizationMenu()
	{
		PlayerCustomizeMenuTransform.gameObject.SetActive(true);
		PauseMenuTransform.gameObject.SetActive(false);
		Time.timeScale = 0;
	}
}
