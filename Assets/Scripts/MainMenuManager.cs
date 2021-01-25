using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour {

	public static MainMenuManager Instance;
	public GameObject[] WindowsGO;
	public Text PlayerNameTxt,VersionTxt;
	public RawImage PlayerRawIMG;
	public Animator WaitingOtherPlayerOnlineAnim,RateUsAnim;
	private Animator MyAnim;
	private RectTransform MyRect;
	private Vector2 FirstPos;
	public GameObject FBLoginBTNGO;

	void Awake(){
		Instance = this;
		Application.targetFrameRate = 30;
		Screen.SetResolution (720,1280,true);
	}

	IEnumerator Start(){
		MyRect = GetComponent<RectTransform> ();
		SetInitials ();
		FireBaseManager.Instance.VersionCheck ();
		SetNameAndFPPhoto ();
		MyAnim = GetComponent<Animator> ();
		FirstPos = MyRect.localPosition;
		yield return new WaitForSeconds (1.5f);
		CheckRating ();
		CheckFBConnect ();
		yield return new WaitForSeconds (1.1f);
		PlayerManager.Instance.SavePlayerData ();
	}

	void CheckFBConnect ()
	{
		if (PlayerManager.Instance.HaveFBID ())
			return;
			ConnectFB.Instance.ShowMenu ();
	}

	void CheckRating ()
	{
		if (PlayerManager.Instance.MyPlayer.RateTaken==1)
			return;
		PlayerManager.Instance.MyPlayer.GamesCounter ++;
		if (PlayerManager.Instance.MyPlayer.GamesCounter >= 8) {
			PlayerManager.Instance.MyPlayer.GamesCounter = 0;
			RateUsAnim.SetBool ("IsShow",true);
		}
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.Escape))
			BackBTN ();
	}

	void BackBTN ()
	{
		if (ConnectFB.Instance.IsShowed) {
			ConnectFB.Instance.HideMenu ();
			return;
		}
		if (LoadingWindow.Instance.IsShowed) {
			LoadingWindow.Instance.HideMenu ();
			return;
		}
		if (FortuneWheelManager.Instance.IsShowed) {
			FortuneWheelManager.Instance.HideMenu ();
			return;
		}
		if (LevelsManager.Instance.LevelDetailsWindowScript.IsShowed) {
			LevelsManager.Instance.PopCloseBTN ();
			return;
		}
		if (ScoreBoardManager.Instance.IsShowed) {
			ScoreBoardManager.Instance.HideMenu ();
			return;
		}
		if (OptionsMenu.Instance.IsShowed) {
			OptionsMenu.Instance.HideMenu ();
			return;
		}
		if (StoreManager.Instance.IsShowed) {
			StoreManager.Instance.HideMenu ();
			return;
		}
		if (OnlineGameManager.Instance.IsShowed) {
			OnlineGameManager.Instance.HideMenu ();
			return;
		}
		if (SelectFBFriend.Instance.IsShowed) {
			SelectFBFriend.Instance.HideMenu ();
			return;
		}
		if (LevelsManager.Instance.LevelsContainerGO.activeSelf) {
			LevelsManager.Instance.BackBTN();
			return;
		}
		WindowsGO [4].GetComponent<Animator> ().SetBool ("IsShow",!WindowsGO [4].GetComponent<Animator> ().GetBool("IsShow"));
	}

	public void SlideToShow(bool Show){
		StopAllCoroutines ();
		StartCoroutine (SlideToShowCor(Show));
	}

	IEnumerator SlideToShowCor (bool Show)
	{
		if (Show) {
			SetNameAndFPPhoto ();
			WindowsGO [0].SetActive (true);
		}
		Vector2 TargetedPos = new Vector2();
		Vector2 FarTargetedPos = new Vector2();
		if (Show) {
			TargetedPos = new Vector2 (0, 0);
			FarTargetedPos = new Vector2 (TargetedPos.x+Constants.SlidingMargin, 0);
		} else {
			TargetedPos = new Vector2 (-1080, 0);
			FarTargetedPos = new Vector2 (TargetedPos.x-Constants.SlidingMargin, 0);
		}
		if (Show) {
			while(MyRect.localPosition.x<TargetedPos.x){
				MyRect.localPosition = Vector2.Lerp (MyRect.localPosition,FarTargetedPos,Time.deltaTime*Constants.SlidingSpeed);
				yield return new WaitForEndOfFrame ();
			}
		} else {
			while(MyRect.localPosition.x>TargetedPos.x){
				MyRect.localPosition = Vector2.Lerp (MyRect.localPosition,FarTargetedPos,Time.deltaTime*Constants.SlidingSpeed);
				yield return new WaitForEndOfFrame ();
			}
		}
		MyRect.localPosition = TargetedPos;
		if(!Show)
			WindowsGO[0].SetActive (false);
	}

	public void CancelMyAnim(){
		MyAnim.enabled = false;
	}

	void SetInitials ()
	{
		VersionTxt.text = "Version " + Application.version;
		FBLoginBTNGO.SetActive (!PlayerManager.Instance.HaveFBID());
	}

	public void UpdateNow(){
		Debug.Log ("UpdateNow");
		WindowsGO [3].GetComponent<Animator> ().SetBool ("IsShow",true);
	}

	public void BTNSFunction(){
		GameObject CurrentBTNGO = EventSystem.current.currentSelectedGameObject;SoundsManager.Instance.PlaySFX (0);
		string BTNName = CurrentBTNGO.name;
		switch(BTNName){
		case "PlayBTN":
			PlayBTN ();
			break;
		case "OptionsBTN":
			OptionsMenu.Instance.ShowMenu ();
			break;
		case "StoreBTN":
			StoreManager.Instance.ShowStore ();
			break;
		case "RankBTN":
			ScoreBoardManager.Instance.ShowScoresListForTopPlayers (1);
			break;
		case "RateUs":
			Application.OpenURL ("https://play.google.com/store/apps/details?id="+Application.identifier);
			break;
		case "ShareBTN":
			#if UNITY_ANDROID
			NativeShare.Share("Try Crushy and challenge your friends now: https://play.google.com/store/apps/details?id="+Application.identifier, "", "https://play.google.com/store/apps/details?id="+Application.identifier, "", "text/plain", true, "");
			#elif UNITY_IOS
			NativeShare.Share("Try Crushy and challenge your friends now:"+Constants.AppStoreLink, "", Constants.AppStoreLink, "", "text/plain", true, "");
			#endif
			break;
		case "CancelBTN":
			OnlineGameManager.Instance.CancelInvitation ();
			break;
		case "FortuneWheelBTN":
			FortuneWheelManager.Instance.ShowSpinner (true);
			break;
		case "QuitYesBTN":
			Application.Quit ();
			break;
		case "QuitNoBTN":
			WindowsGO [4].GetComponent<Animator> ().SetBool ("IsShow",false);
			break;
		case "RateYesBTN":
			PlayerManager.Instance.SavePlayerData ();
			PlayerManager.Instance.MyPlayer.RateTaken = 1;
			Application.OpenURL ("https://play.google.com/store/apps/details?id=" + Application.identifier);
			RateUsAnim.SetBool ("IsShow", false);
			PlayerManager.Instance.SavePlayerData ();
			break;
		case "RateNoBTN":
			PlayerManager.Instance.SavePlayerData ();
			RateUsAnim.SetBool ("IsShow",false);
			break;
		case "FBBTN":
			FBManager.Instance.FBLogInBTNClick ();
			break;
		case "UpdateBTN":
			Application.OpenURL ("https://play.google.com/store/apps/details?id=" + Application.identifier);
			break;
		}
	}

	public void PlayBTN ()
	{
		SlideToShow (false);
		LevelsManager.Instance.SlideToShow (true);
	}

	public void SetNameAndFPPhoto ()
	{
		PlayerNameTxt.text = PlayerManager.Instance.MyPlayer.PlayerName;
		if (PlayerManager.Instance.HaveFBID ()) {
			FBManager.Instance.LoadThisPhoto (PlayerManager.Instance.MyPlayer.PlayerID, PlayerRawIMG);
			FBLoginBTNGO.SetActive (false);
		}
	}

	public void ShowWaitingOnlinePlayer(bool ShowOrHide){
		WaitingOtherPlayerOnlineAnim.SetBool ("IsShow",ShowOrHide);
	}
}