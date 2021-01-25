using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class MenuFather :MonoBehaviour
{
	protected Animator MyAnim;
	[HideInInspector]
	public bool IsShowed;

	protected virtual void Start(){
		MyAnim = GetComponent<Animator> ();
	}

	public virtual void ShowMenu(){
		IsShowed = true;
		MyAnim.SetBool ("IsShow",true);
	}

	public virtual void HideMenu(){
		IsShowed = false;
		MyAnim.SetBool ("IsShow",false);
	}
}
