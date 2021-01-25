using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ImagesArray : MonoBehaviour
{
	public Image[] RowScoreNoIMGS;
	private Animator MyAnim;

	void Start(){
		MyAnim = GetComponent<Animator> ();
	}

	public void ShowThisNo(){
		StartCoroutine (ShowThisNoCor());
	}

private IEnumerator ShowThisNoCor(){
		MyAnim.SetBool ("IsShow",true);
		yield return new WaitForSeconds (1f);
		MyAnim.SetBool ("IsShow",false);
		gameObject.SetActive (false);
	}
}