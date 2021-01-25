using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FriendGift : ImportedItem
{
	public RawImage PlayerIMG;
	public Text PlayerNameTxt,GiftStatus;
	public GameObject AcceptBTNGO,SendBTNGO;
	private GiftO MyFriendGift;

	public void SetGiftItemDetails(GiftO ThisGiftItem){
		Activated = this;
		MyFriendGift = ThisGiftItem;
		if (MyFriendGift.SendOrReceived == 0) {
			PlayerNameTxt.text = ThisGiftItem.RName;
			FBManager.Instance.LoadThisPhoto (ThisGiftItem.RID,PlayerIMG);
		} else {
			PlayerNameTxt.text = ThisGiftItem.SName;
			FBManager.Instance.LoadThisPhoto (ThisGiftItem.SID,PlayerIMG);
		}
		SetGiftStatus ();
	}

	void SetGiftStatus ()
	{
		AcceptBTNGO.SetActive (false);
		SendBTNGO.SetActive (false);
		GiftStatus.text = "";
		if (MyFriendGift.SendOrReceived == 0) {
				SendBTNGO.SetActive (true);
				GiftStatus.text = "";	
		} else {
				AcceptBTNGO.SetActive (true);
				GiftStatus.text = "";	
		}
	}

	public void AcceptBTNClick(){
		SoundsManager.Instance.PlaySFX (0);
		if (PlayerManager.Instance.MyPlayer.ReceiveGiftsCounter >= 5) {
			PopMSG.Instance.ShowPopMSG ("You can accept only 5 gift everyday!");
		} else {
			FireBaseManager.Instance.RemoveThisGift (MyFriendGift.GiftID);
			PlayerManager.Instance.MyPlayer.ReceiveGiftsCounter++;
			int RandomGift = Random.Range (0,4);
			if(RandomGift!=3)
				GiftsShowerManager.Instance.GiveABombGift ();
			else
				GiftsShowerManager.Instance.GiveACoinsGift();
		}
	}

	public void SendBTNClick(){
		SoundsManager.Instance.PlaySFX (0);
		FriendsGiftManager.Instance.SendGift(MyFriendGift);
	}

	string GetNameOfThisBomb (int BombID)
	{
		switch(BombID){
		case 0:
			return "Blue";
		case 1:
			return "Green";
		case 2:
			return "Red";
		case 3:
			return "Pink";
		case 4:
			return "Gold";
		case 5:
			return "Teal";
		case 6:
			return "White";
		default:
			return "Blue";
		}
	}

}