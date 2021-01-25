using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectFB : MenuFather{

	public static ConnectFB Instance;

	void Awake(){
		Instance=this;
	}
}