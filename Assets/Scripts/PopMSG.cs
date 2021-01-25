using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopMSG : MonoBehaviour {

	public static PopMSG Instance;
	public Text PopMSGTxt;
	public GameObject ContainerGO;
	private Animator MyAnim;

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

	void Start(){
		MyAnim = GetComponent<Animator> ();
	}

	public void ShowPopMSG(float TimeToShow,string MSGTxt){
		StartCoroutine (ShowPopMSGCor(TimeToShow,MSGTxt));
	}

	public void ShowPopMSG(string MSGTxt){
		StartCoroutine (ShowPopMSGCor(2.5f,MSGTxt));
	}

	private IEnumerator ShowPopMSGCor(float TimeToShow,string MSGTxt){
		PopMSGTxt.text = MSGTxt;
		MyAnim.SetBool ("IsShow",true);
		yield return new WaitForSeconds (TimeToShow);
		MyAnim.SetBool ("IsShow",false);
	}

}
