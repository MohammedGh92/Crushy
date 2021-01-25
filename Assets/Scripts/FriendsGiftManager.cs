using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FriendsGiftManager : MenuFather {
	
	public static FriendsGiftManager Instance;
	public GameObject ContainerGO;
	public FriendsGiftList GiftsList;

	void Awake(){
		Instance=this;
	}

	void Start(){
		MyAnim = GetComponent<Animator> ();
	}

	public void BTNSFunction ()
	{
		GameObject CurrentBTNGO = EventSystem.current.currentSelectedGameObject;SoundsManager.Instance.PlaySFX (0);
		string BTNName = CurrentBTNGO.name;
		switch(BTNName){
		case "SendTabBTN":
			FBManager.Instance.CurrentMenuFriendsShow = 0;
			FBManager.Instance.UpdateMyFBFriendsList ();
			break;
		case "ReceiveTab":
			FBManager.Instance.CurrentMenuFriendsShow = 1;
			FBManager.Instance.UpdateMyFBFriendsList ();
			break;
		case "CloseBTN":
			CloseBTN ();
			break;
		}
	}

	public void CloseBTN ()
	{
		IsShowed = false;
		MyAnim.SetBool ("IsShow",false);
	}

	public void ShowSendGiftsFriendsList(){
		List<GiftO> FriendsList = new List<GiftO> ();
		List<FBFriendO> PlayerFBFriendsList = PlayerManager.Instance.MyPlayer.MyFBFriends;
		for (int i = 0; i < PlayerFBFriendsList.Count; i++) {
			GiftO NewGift = new GiftO();
			NewGift.GiftID = RandomIDGenerator.Instance.GetRandomID ();
			NewGift.SName= PlayerManager.Instance.MyPlayer.PlayerName;
			NewGift.RID = PlayerFBFriendsList[i].id;
			NewGift.RName=PlayerFBFriendsList[i].name;
			NewGift.SendOrReceived = 0;
			FriendsList.Add (NewGift);
		}
		ShowThisGiftsList (FriendsList);
	}

	public void ShowReceivedGiftsFriendsList(){
		FireBaseManager.Instance.GetPlayerGiftsList ();
	}

	public void ShowThisGiftsList(List<GiftO> GiftsItems){
		IsShowed = true;
		MyAnim.SetBool ("IsShow",true);
		GiftsList.DisActivateAllItems ();
		for(int i=0;i<GiftsItems.Count;i++)
			GiftsList.AddItem (GiftsItems[i]);
	}

	public void SendGift(GiftO ThisGift){
		PlayerManager.Instance.SavePlayerData ();
		if (PlayerManager.Instance.MyPlayer.SendGiftsCounter >= 5)
			PopMSG.Instance.ShowPopMSG ("You can send only 5 gifts everyday!");
		else {
			PlayerManager.Instance.MyPlayer.SendGiftsCounter++;
			PopMSG.Instance.ShowPopMSG ("Gift Sent!");
			FireBaseManager.Instance.SendGift (ThisGift);
		}
	}
}