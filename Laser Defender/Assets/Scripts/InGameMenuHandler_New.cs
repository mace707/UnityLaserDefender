using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenuHandler_New : MonoBehaviour 
{
	public enum MenuItem
	{
		MenuItemPause,
		MenuItemCustomize,
		MenuItemCountDownTimer,
	};
	//When adding a new ingame menu, ensure it is in the same position as the corresponding enum.
	[SerializeField]
	public List<GameObject> InGameMenus;

	// Pause
	private List<Enemy> NextLevelEnemies;
	private int NextLevelEnemyIndex;
	Slider NextLevelEnemySlider;

	// Timer
	Text mCountDownTimerText;
	int TimerValue;

	void Start () 
	{
		NextLevelEnemyIndex = 0;
	}

	private void ActivateMenu(MenuItem menu)
	{
		int menuInt = (int)menu;
		for(int i = 0; i < InGameMenus.Count; i++)
		{
			if(i == menuInt) 	InGameMenus[i].SetActive(true);
			else				InGameMenus[i].SetActive(false);
		}
	}

	private void DeactivateMenu(MenuItem menu)
	{
		InGameMenus[(int)menu].SetActive(false);
	}
		
	/**************************************************************************************************************************/
	/********************************************************PAUSE MENU********************************************************/

	public void ActivatePauseMenu(List<Enemy> enemies)
	{
		ActivateMenu(MenuItem.MenuItemPause);

		NextLevelEnemies = enemies;
		NextLevelEnemyIndex = 0;
		NextLevelEnemySlider = GameObject.Find("UniqueEnemiesInNextRoundSlider").GetComponent<Slider>();

		if(NextLevelEnemies.Count >= 1)	
			NextLevelEnemySlider.maxValue = NextLevelEnemies.Count - 1;
		else
			NextLevelEnemySlider.gameObject.SetActive(false);

		ShowNextLevelEnemyWrapper();
	}

	public void NextLevelEnemySliderChange()
	{
		NextLevelEnemyIndex = (int)NextLevelEnemySlider.value;
		ShowNextLevelEnemyWrapper();
	}

	private void ShowNextLevelEnemyWrapper()
	{
		if(NextLevelEnemies.Count == 0)
			return;

		GameObject.Find("NextEnemyHealthTxt").GetComponent<Text>().text 		= NextLevelEnemies[NextLevelEnemyIndex].Health.ToString();
		GameObject.Find("NextEnemyDamageTxt").GetComponent<Text>().text 		= NextLevelEnemies[NextLevelEnemyIndex].Damage.ToString();
		GameObject.Find("NextEnemyBulletSpeedTxt").GetComponent<Text>().text 	= NextLevelEnemies[NextLevelEnemyIndex].ProjectileSpeed.ToString();
		GameObject.Find("NextEnemyMovementSpeedTxt").GetComponent<Text>().text 	= "5";
		GameObject.Find("NextEnemyImage").GetComponent<Image>().sprite 			= NextLevelEnemies[NextLevelEnemyIndex].GetComponent<SpriteRenderer>().sprite;
	}

	public void DeactivatePauseMenu()
	{
		DeactivateMenu(MenuItem.MenuItemPause);
	}

	/********************************************************PAUSE MENU********************************************************/
	/**************************************************************************************************************************/

	/********************************************************************************************************************************/
	/********************************************************COUNT DOWN TIMER********************************************************/

	public void ActivateCoundDownTimer()
	{
		GlobalConstants.FreezeAllNoTimeScale = true;
		ActivateMenu(MenuItem.MenuItemCountDownTimer);
		StartCountDownTimer();
	}

	private void StartCountDownTimer()
	{
		mCountDownTimerText = GameObject.Find("CountDownTimerText").GetComponent<Text>();
		mCountDownTimerText.text = "GET READY";
		TimerValue = 3;
		InvokeRepeating("BeginTimer", 3, 1);
	}

	private void BeginTimer()
	{
		mCountDownTimerText.text = TimerValue.ToString();
		if(TimerValue == 0)
		{
			CancelInvoke("BeginTimer");
			DeactivateCountDownTimer();
		}
		TimerValue--;
	}

	public void DeactivateCountDownTimer()
	{
		DeactivateMenu(MenuItem.MenuItemCountDownTimer);
		GlobalConstants.FreezeAllNoTimeScale = false;
	}

	/********************************************************COUNT DOWN TIMER********************************************************/
	/********************************************************************************************************************************/



}
