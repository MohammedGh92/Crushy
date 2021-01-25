using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FriendsGiftList : ListFather
{
	public void AddItem(GiftO ThisFriendGift){
		ItemsCounter++;
		if (MyItemsGO.Count >= ItemsCounter) {
			MyItemsGO [ItemsCounter - 1].GetComponent<FriendGift>().Activate ();
			MyItemsGO [ItemsCounter - 1].GetComponent<FriendGift> ().SetGiftItemDetails (ThisFriendGift);
		} else {
			GameObject NewITem = Instantiate (ItemPrefab, ContentRect.transform);
			NewITem.GetComponent<FriendGift> ().SetGiftItemDetails (ThisFriendGift);
			MyItemsGO.Add (NewITem);
		}
	}
}