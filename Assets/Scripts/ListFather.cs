using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ListFather : MonoBehaviour {

	[HideInInspector]
	public int ItemsCounter;
	public RectTransform ContentRect;
	public GameObject ItemPrefab;
	public List<GameObject> MyItemsGO;

	public void DisActivateAllItems(){
		for (int i = 0; i < MyItemsGO.Count; i++)
			MyItemsGO[i].GetComponent<ImportedItem>().DisActivate ();
		ItemsCounter = 0;
	}

	public void DestroyAllItems(){
		for (int i = 0; i < ContentRect.childCount; i++)
			Destroy (ContentRect.GetChild(i).gameObject);
		ItemsCounter = 0;
	}

}