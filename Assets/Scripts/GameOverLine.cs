using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverLine : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col)
	{
		if (!ShapesManager.Instance.GameIsOver) {
			if (!GameManager.Instance.ReviveWindowShowed&&CanRevive()&&CanRevive()) {
				Time.timeScale = 0;
				GameManager.Instance.ShowReviveWindow ();
				return;
			}else if (GameManager.Instance.ReviveWindowShowed||!CanRevive())
				GameManager.Instance.GameIsOver (false);
		}
	}

	private bool CanRevive(){
		if (PlayerManager.Instance.MyPlayer.Coins >= GameManager.Instance.ReviveCoins)
			return true;
		else if (PlayerManager.Instance.InternetAvailability == true)
			return true;
		else
			return false;
	}

}