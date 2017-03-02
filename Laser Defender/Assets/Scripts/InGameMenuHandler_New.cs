using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenuHandler_New : MonoBehaviour 
{
	public enum MenuItem
	{
		MenuItemPause,
		MenuItemCustomize,
	};
	//When adding a new ingame menu, ensure it is in the same position as the corresponding enum.
	List<GameObject> InGameMenus;

	void Start () 
	{
		InGameMenus = new List<GameObject>();
		foreach(Transform menu in transform)
			InGameMenus.Add(menu.gameObject);
	}

	public void ActivateMenu(MenuItem menu)
	{
		int menuInt = (int)menu;
		for(int i = 0; i < InGameMenus.Count; i++)
		{
			if(i == menuInt) 	InGameMenus[i].SetActive(true);
			else				InGameMenus[i].SetActive(false);
		}
	}

	public void DeactivateMenu(MenuItem menu)
	{
		InGameMenus[(int)menu].SetActive(false);
	}
}
