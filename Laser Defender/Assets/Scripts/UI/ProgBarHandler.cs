using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgBarHandler
{
	public static void UpdateUIComponent(string uiTxtName, string uiImgName, char unit, float value, float max)
	{
		Image uiImg = GameObject.Find(uiImgName).GetComponent<Image>();
		Text uiTxt = GameObject.Find(uiTxtName).GetComponent<Text>();
		float ratio = value / max;
		uiImg.rectTransform.localScale = new Vector3(ratio, 1, 1);
		uiTxt.text = unit + " " + value + "/" + max;
	}

}
