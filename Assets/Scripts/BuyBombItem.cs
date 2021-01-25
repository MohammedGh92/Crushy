using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyBombItem : MonoBehaviour {

	public Text AmountTxt,PriceTxt;
	public int Amount,Price;

	void Start(){
		Price =((int)((transform.GetSiblingIndex ()+1) * Price * 150f));
		PriceTxt.text = Price.ToString ("00");
	}

	public void AddOrTakeAmount(bool AddOrTake){
		SoundsManager.Instance.PlaySFX (0);
		if (AddOrTake)
			Amount += 1;
		else if (Amount > 0)
			Amount -= 1;
		AmountTxt.text = Amount.ToString ("00");
	}

	public void ResetAmount(){
		Amount = 0;
		AmountTxt.text = Amount.ToString ("00");
	}

}