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
	}
		
	public void DeactivatePlayerCustomizationMenu()
	{
		PlayerCustomizeMenuTransform.gameObject.SetActive(false);
	}

	public void ActivatePauseMenu()
	{
		PauseMenuTransform.gameObject.SetActive(true);
		PlayerCustomizeMenuTransform.gameObject.SetActive(false);
	}
		
	public void ActivatePlayerCustomizationMenu()
	{
		PlayerCustomizeMenuTransform.gameObject.SetActive(true);
		PauseMenuTransform.gameObject.SetActive(false);
	}
}
