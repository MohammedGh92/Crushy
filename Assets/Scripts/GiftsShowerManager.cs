using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftsShowerManager : MonoBehaviour {

	public static GiftsShowerManager Instance;
	private Animator MyAnim;
	public GameObject[] BombsGO,CoinsGO;
	public Text TitleTxt,CoinsTxt;

	void Awake(){
		Singlton ();
		MyAnim = GetComponent<Animator> ();
	}

	public void GiveRandomGift(){
		int RandomNO = Random.Range (0,2);
		if (RandomNO == 0)
			GiveABombGift ();
		else
			GiveACoinsGift();
	}

	void Singlton ()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy (gameObject);
		DontDestroyOnLoad (this);
	}

	public void GiveABombGift(){
		TitleTxt.text = "Gift";
		ResetData ();
		StartCoroutine (GiveABombGiftCor());
	}

	public void GiveDailyBomb(){
		TitleTxt.text = "Daily Gift";
		ResetData ();
		GiveABombGift (PlayerManager.Instance.MyPlayer.DailyGiftsCounter);
	}

	private void ResetData(){
		StopAllCoroutines ();
		for (int i = 0; i < BombsGO.Length; i++)
			BombsGO [i].SetActive (false);
		for (int i = 0; i < CoinsGO.Length; i++)
			CoinsGO [i].SetActive (false);
		SoundsManager.Instance.PlaySFX (9);
	}

	IEnumerator GiveABombGiftCor(){
		CoinsTxt.enabled = false;
		int RandomBombNO = Random.Range (0,GetBombsRange());
		PlayerManager.Instance.MyPlayer.Bombs [RandomBombNO]+=1;
		PlayerManager.Instance.SavePlayerData ();
		BombsGO [RandomBombNO].SetActive (true);
		MyAnim.SetBool ("IsShow",true);
		yield return new WaitForSeconds (1.5f);
		MyAnim.SetBool ("IsShow",false);
	}

	int GetBombsRange ()
	{
		int CurrentNoOfFivesInLevelNO = NoOfFivesInLevelNO (PlayerManager.Instance.MyPlayer.LevelProgress);
		if (CurrentNoOfFivesInLevelNO > 4)
			CurrentNoOfFivesInLevelNO = 4;
		return 3 + CurrentNoOfFivesInLevelNO;
	}

	public void GiveACoinsGift(){
		TitleTxt.text = "Gift";
		ResetData ();
		StartCoroutine (GiveACoinsGiftCor());
	}

	public void Give5DaysCoinsGift(bool CoinsPack){
		TitleTxt.text = "Daily Gift";
		ResetData ();
		if(CoinsPack)
			GiveACoinsGift (1);
		else
			GiveACoinsGift (0);
	}

	IEnumerator GiveACoinsGiftCor(){
		CoinsTxt.enabled = true;
		int RandomCoinsNO = Random.Range (0,3);
		int CoinsNo = 300;
		if (RandomCoinsNO == 1)
			CoinsNo = 500;
		else if (RandomCoinsNO == 2)
			CoinsNo = 1000;
		CoinsTxt.text = CoinsNo+" Coins";
		PlayerManager.Instance.MyPlayer.Coins += CoinsNo;
		PlayerManager.Instance.SavePlayerData ();
		CoinsGO [RandomCoinsNO].SetActive (true);
		MyAnim.SetBool ("IsShow",true);
		yield return new WaitForSeconds (1.5f);
		MyAnim.SetBool ("IsShow",false);
	}

	private int NoOfFivesInLevelNO (int NO)
	{
		int ThisLEvelNO = NO;
		int FivesCounter = 0;
		while (ThisLEvelNO > 0) {
			ThisLEvelNO -= 5;
			if (ThisLEvelNO > 0)
				FivesCounter++;
		}
		return FivesCounter;
	}

	public void GiveABombGift(int BombID){
		ResetData ();
		StartCoroutine (GiveABombGiftCor(BombID));
	}

	private IEnumerator GiveABombGiftCor(int BombID){
		CoinsTxt.enabled = false;
		PlayerManager.Instance.MyPlayer.Bombs [BombID]+=1;
		PlayerManager.Instance.SavePlayerData ();
		BombsGO [BombID].SetActive (true);
		MyAnim.SetBool ("IsShow",true);
		yield return new WaitForSeconds (1.5f);
		MyAnim.SetBool ("IsShow",false);
	}

	public void GiveACoinsGift(int CoinsID){
		ResetData ();
		StartCoroutine (GiveACoinsGiftCor(CoinsID));
	}

	private IEnumerator GiveACoinsGiftCor(int CoinsID){
		CoinsTxt.enabled = true;
		int CoinsNo = 300;
		if (CoinsID == 1)
			CoinsNo = 500;
		else if (CoinsID == 2)
			CoinsNo = 1000;
		else if (CoinsID == 3)
			CoinsNo = 2000;
		else if (CoinsID == 4)
			CoinsNo = 5000;
		CoinsTxt.text = CoinsNo+" Coins";
		PlayerManager.Instance.MyPlayer.Coins += CoinsNo;
		PlayerManager.Instance.SavePlayerData ();
		if (CoinsID > 2)
			CoinsID = 2;
		CoinsGO [CoinsID].SetActive (true);
		MyAnim.SetBool ("IsShow",true);
		yield return new WaitForSeconds (1.5f);
		MyAnim.SetBool ("IsShow",false);
	}
}