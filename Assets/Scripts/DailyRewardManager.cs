using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRewardManager : MenuFather {

	public static DailyRewardManager Instance;
	public GameObject[] CheckedGO;

	void Awake(){
		Instance=this;
	}

	public void ShowDailyProgress(){
		StartCoroutine (ShowDailyProgressCor());
	}

	private IEnumerator ShowDailyProgressCor(){
		ShowMenu ();
		int DailyGiftsProgress = PlayerManager.Instance.MyPlayer.DailyGiftsCounter;
		for (int i = 0; i < CheckedGO.Length; i++)
			CheckedGO [i].SetActive (false);
		for (int i = 0; i <= DailyGiftsProgress; i++)
			CheckedGO [i].SetActive (true);
		for (int i = 0; i < 8; i++) {
			yield return new WaitForSeconds (0.25f);
			CheckedGO [DailyGiftsProgress].SetActive (false);
			yield return new WaitForSeconds (0.25f);
			CheckedGO [DailyGiftsProgress].SetActive (true);
		}
		yield return new WaitForSeconds (1);
		HideMenu ();
	}
}