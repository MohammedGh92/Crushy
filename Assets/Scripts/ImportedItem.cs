using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImportedItem : MonoBehaviour
{
	[HideInInspector]
	public bool Activated;
	public void Activate(){
		Activated = true;
		gameObject.SetActive (true);
	}

	public void DisActivate(){
		Activated = false;
		gameObject.SetActive (false);	
	}
}
