using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBFriendsList : ListFather {

	public void AddItem(FBFriendO ThisFriend){
		ItemsCounter++;
		if (MyItemsGO.Count >= ItemsCounter) {
			MyItemsGO [ItemsCounter - 1].GetComponent<FBFriend>().Activate ();
			MyItemsGO [ItemsCounter - 1].GetComponent<FBFriend> ().SetFriendData (ThisFriend);
		} else {
			GameObject NewITem = Instantiate (ItemPrefab, ContentRect.transform);
			NewITem.GetComponent<FBFriend> ().SetFriendData (ThisFriend);
			MyItemsGO.Add (NewITem);
		}
	}

}