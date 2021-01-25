using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertZone : MonoBehaviour {

	public int AlarmOn;

	void OnTriggerStay2D(Collider2D col)
	{
		if (AlarmOn == 0)
			AlarmOn = 1;
	}

	private float TimerCounter;
	void Update(){
		if (AlarmOn==1) {
			if (GameManager.Instance.IsBonusLevel || GameManager.Instance.ShapesManagerScript.GameIsOver)
				AlarmOn = 2;
			TimerCounter += 1 * Time.deltaTime;
			if (TimerCounter >= 0.7f) {
				TimerCounter = 0;
				SoundsManager.Instance.PlaySFX (14);
			}
		}
	}
}