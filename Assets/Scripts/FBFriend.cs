using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FBFriend : ImportedItem {

	public RawImage IMG;
	public Text NameTxt;
	private FBFriendO MyFBFriend;

	public void SetFriendData(FBFriendO ThisFriend){
		Activated = this;
		MyFBFriend = ThisFriend;
		NameTxt.text = ThisFriend.name;
		FBManager.Instance.LoadThisPhoto (ThisFriend.id,IMG);
	}

	public void FriendClick(){
		SoundsManager.Instance.PlaySFX (0);
		Debug.Log ("Here\t"+MyFBFriend.id);
		SelectFBFriend.Instance.HideMenu ();
		OnlineGameManager.Instance.SendInvite (MyFBFriend);
	}

}