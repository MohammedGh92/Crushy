using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectFBFriend : MenuFather {

	public static SelectFBFriend Instance;
	public GameObject ContainerGO;
	public FBFriendsList FBFriendsList;

	void Awake(){
		Instance=this;
	}

	public void BTNSFunction ()
	{
		GameObject CurrentBTNGO = EventSystem.current.currentSelectedGameObject;SoundsManager.Instance.PlaySFX (0);
		string BTNName = CurrentBTNGO.name;
		switch(BTNName){
		case "CloseBTN":
			MyAnim.SetBool ("IsShow",false);
			break;
		}
	}

	public void ShowThisFriendsList(){
		MyAnim.SetBool ("IsShow",true);
		FBFriendsList.DisActivateAllItems ();
		for(int i=0;i<PlayerManager.Instance.MyPlayer.MyFBFriends.Count;i++)
			FBFriendsList.AddItem (PlayerManager.Instance.MyPlayer.MyFBFriends[i]);
	}
}
