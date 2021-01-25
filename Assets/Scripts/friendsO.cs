using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;
using System.IO;
using System;
using SimpleJSON;

[Serializable]
public class friendsO
{
	public List<FBFriendO> data;
	public friendsO(){
		data = new List<FBFriendO> ();
	}
}