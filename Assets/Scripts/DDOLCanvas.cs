using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDOLCanvas : MonoBehaviour {

	private static DDOLCanvas Instance;

	void Awake(){
		Singlton ();
	}

	void Singlton ()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy (gameObject);
		DontDestroyOnLoad (this);
	}
}
