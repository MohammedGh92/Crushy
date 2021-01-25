using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingCrush : MonoBehaviour {

	public GameObject MyContainerGO,FireWorksGO;

	public void ShowIt(){
		StartCoroutine (ShowItCor());
	}

	private IEnumerator ShowItCor(){
		MyContainerGO.SetActive (true);
		SoundsManager.Instance.PlaySFX (16);
		yield return new WaitForSeconds (0.75f);
		FireWorksGO.SetActive (true);
		yield return new WaitForSeconds (0.4f);
		GameManager.Instance.ToBlue ();
		yield return new WaitForSeconds (1.5f);
		MyContainerGO.SetActive (false);
		FireWorksGO.SetActive (false);
	}

}