using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoreManager : MenuFather {

	public static StoreManager Instance;
	public List<BuyBombItem> BuyBombsItemsList;
	public GameObject ContainerGO;
	public Text CoinsTxt;
	private IAP MyIAP;

	void Awake(){
		Instance=this;
	}

	protected override void Start(){
		MyIAP = GetComponent<IAP> ();
		MyAnim = GetComponent<Animator> ();
	}

	public void ShowStore(){
		CoinsTxt.text = PlayerManager.Instance.MyPlayer.Coins.ToString("00");
		ShowMenu ();
	}

	private void BuyBombs(){
		int TotalCost = 0;
		for (int i = 0; i < BuyBombsItemsList.Count; i++)
			if (BuyBombsItemsList [i].Amount> 0)
				TotalCost += BuyBombsItemsList [i].Amount * BuyBombsItemsList [i].Price;
		if (TotalCost == 0) {
			SoundsManager.Instance.PlaySFX (11);
			PopMSG.Instance.ShowPopMSG ("Add Bombs Amount");
			return;
		}
		if (PlayerManager.Instance.MyPlayer.Coins >= TotalCost) {
			SoundsManager.Instance.PlaySFX (10);
			PopMSG.Instance.ShowPopMSG ("Purchase Success");
			PlayerManager.Instance.MyPlayer.Coins -= TotalCost;
			for (int i = 0; i < BuyBombsItemsList.Count; i++)
				if (BuyBombsItemsList [i].Amount > 0) {
					PlayerManager.Instance.MyPlayer.Bombs [i] += BuyBombsItemsList [i].Amount;
					BuyBombsItemsList [i].ResetAmount ();
				}
			PlayerManager.Instance.SavePlayerData ();
		} else {
			SoundsManager.Instance.PlaySFX (11);
			PopMSG.Instance.ShowPopMSG ("Need More Coins!");
		}
		RefreshCoins ();
	}

	public void BTNSFunction ()
	{
		GameObject CurrentBTNGO = EventSystem.current.currentSelectedGameObject;SoundsManager.Instance.PlaySFX (0);
		string BTNName = CurrentBTNGO.name;
		switch (BTNName) {
		case "BuyBombsBTN":
			BuyBombs ();
			break;
		case "CloseBTN":
			MyAnim.SetBool ("IsShow",false);
			break;
		case "CoinsPackBTN":
			int CoinsPackID = CurrentBTNGO.transform.GetSiblingIndex ();
			MyIAP.BuyCoins (CoinsPackID);
			break;
		}
	}

	public void RefreshCoins(){
		CoinsTxt.text = PlayerManager.Instance.MyPlayer.Coins.ToString ("00");
		LevelsManager.Instance.SetDetails ();
	}
}