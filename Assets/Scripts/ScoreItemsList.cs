using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreItemsList : ListFather
{
	public void AddItem(ScoreO ThisScoretem){
		ItemsCounter++;
		if (MyItemsGO.Count >= ItemsCounter) {
			MyItemsGO [ItemsCounter - 1].GetComponent<ScoreItem>().Activate ();
			MyItemsGO [ItemsCounter - 1].GetComponent<ScoreItem> ().SetScoreItemDetails (ThisScoretem);
		} else {
			GameObject NewITem = Instantiate (ItemPrefab, ContentRect.transform);
			NewITem.GetComponent<ScoreItem> ().Activate ();
			NewITem.GetComponent<ScoreItem> ().SetScoreItemDetails (ThisScoretem);
			MyItemsGO.Add (NewITem);
		}
	}
}