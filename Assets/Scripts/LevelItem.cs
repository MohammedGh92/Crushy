using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelItem : MonoBehaviour {
	[HideInInspector]
	public LevelItemO MyLevelItem;
	public Image[] StarsIMG;
	public Text LevelNO;
	public Image LockedIMG;

	public void SetLevelDetails(LevelItemO ThisLevelItem){
		MyLevelItem = ThisLevelItem;
		LevelNO.text = MyLevelItem.LevelID.ToString("00");
		GetComponent<Button> ().interactable = (MyLevelItem.IsUnLocked == 1);
		GetComponent<Button> ().onClick.AddListener (OnLevelClick);
		LockedIMG.enabled = MyLevelItem.IsUnLocked==0;
		if (MyLevelItem.IsUnLocked == 0) {
			LevelNO.color = Color.clear;
			for (int i = 0; i < 3; i++)
				StarsIMG [i].enabled = false;
			return;
		}
		for (int i = 0; i < 3; i++)
			StarsIMG [i].enabled = true;
		for (int i = 1; i <= MyLevelItem.StarsNO; i++) {
			if (i > 3)
				break;
			StarsIMG [i - 1].color = Color.white;
		}
	}

	public void OnLevelClick(){
		SoundsManager.Instance.PlaySFX (0);
		if (MyLevelItem.LevelID == 0){
			LevelsManager.Instance.ChoosedLevelNO = 1;
			LevelsManager.Instance.IsTotorial = true;
			LevelsManager.Instance.PlayBTN ();
			return;
		}
		LevelsManager.Instance.ShowLevelDetails (MyLevelItem.LevelID);
	}
}