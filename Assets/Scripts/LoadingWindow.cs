using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingWindow : MonoBehaviour {

	public static LoadingWindow Instance;
	public GameObject ContainerGO;
	public bool IsShowed;

	void Awake(){
		Instance=this;
	}

	public void ShowMenu(){
		IsShowed = true;
		ContainerGO.SetActive (true);
	}

	public void HideMenu(){
		IsShowed = false;
		ContainerGO.SetActive (false);
	}
}