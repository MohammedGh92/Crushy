using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour {

	public int TimeScaleSpeed;
	public bool DoIt=true;

	void Update(){
		if (DoIt) {
			DoIt = false;
			Time.timeScale = TimeScaleSpeed;
		}
	}

}