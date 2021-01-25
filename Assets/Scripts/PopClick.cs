using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopClick : MonoBehaviour {

	private RectTransform MyRectTrans;
	private Vector3 FirstRectScale;

	void Start(){
		MyRectTrans = GetComponent<RectTransform> ();
		FirstRectScale = MyRectTrans.localScale;
		try{GetComponent<Button> ().onClick.AddListener (PopAnim);}catch{
		}
	}

	public void PopAnim ()
	{
		StopAllCoroutines ();
		try{StartCoroutine (PopAnimCor());}catch{
		}
	}

	void OnDisable(){
		StopAllCoroutines ();
	}

	IEnumerator PopAnimCor ()
	{
		Vector3 ScaleBeforPop = FirstRectScale;
		Vector3 SmallerPopTarget = new Vector3 (ScaleBeforPop.x-0.2f,ScaleBeforPop.y-0.2f,ScaleBeforPop.z-0.2f);
		while(MyRectTrans.localScale.x>SmallerPopTarget.x+0.1f){
			MyRectTrans.localScale = Vector3.Lerp (MyRectTrans.localScale,SmallerPopTarget,Time.deltaTime*12);
			yield return new WaitForSeconds (0.01f);
		}
		MyRectTrans.localScale = SmallerPopTarget;
		while(MyRectTrans.localScale.x<ScaleBeforPop.x-0.05f){
			MyRectTrans.localScale = Vector3.Lerp (MyRectTrans.localScale,ScaleBeforPop,Time.deltaTime*12);
			yield return new WaitForSeconds (0.01f);
		}
		MyRectTrans.localScale = ScaleBeforPop;
	}
}