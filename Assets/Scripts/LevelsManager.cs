using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelsManager : MonoBehaviour {

	public static LevelsManager Instance;
	public List<LevelItem> LevelsItems;
	public GameObject LevelsContainerGO;
	public Text PlayerCoinsTxt,PlayerScoreTxt,PlayerNameTxt;
	public RawImage PlayerRawIMG;
	[HideInInspector]
	public int ChoosedLevelNO;
	[HideInInspector]
	public bool IsBonusLevel,IsOnlineGame,IsTotorial;
	public LevelDetailsWindow LevelDetailsWindowScript;
	private RectTransform MyRect;

	void Awake(){
		Instance=this;
	}

	void Start(){
		MyRect = GetComponent<RectTransform> ();
		SetDetails ();
		SetLevelsItems ();
		IsTotorial = false;
		if (PlayerManager.Instance.ShowLevelOnStart>0)
			StartCoroutine (ShowLevelOnStartCor());
	}

	private IEnumerator ShowLevelOnStartCor(){
		yield return new WaitForSeconds (0.1f);
		MainMenuManager.Instance.PlayBTN ();
	}

	public void SlideToShow(bool Show){
		StopAllCoroutines ();
		StartCoroutine (SlideToShowCor(Show));
	}

	IEnumerator SlideToShowCor (bool Show)
	{
		if(Show)
			LevelsContainerGO.SetActive (Show);
		Vector2 TargetedPos = new Vector2();
		Vector2 FarTargetedPos = new Vector2();
		if (Show) {
			TargetedPos = new Vector2 (0, 0);
			FarTargetedPos = new Vector2 (TargetedPos.x-Constants.SlidingMargin, 0);
		} else {
			TargetedPos = new Vector2 (1080, 0);
			FarTargetedPos = new Vector2 (TargetedPos.x+Constants.SlidingMargin, 0);
		}
		if (Show) {
			while(MyRect.localPosition.x>TargetedPos.x){
				MyRect.localPosition = Vector2.Lerp (MyRect.localPosition,FarTargetedPos,Time.deltaTime*Constants.SlidingSpeed);
				yield return new WaitForSeconds (0.01f);
			}
		} else {
			while(MyRect.localPosition.x<TargetedPos.x){
				MyRect.localPosition = Vector2.Lerp (MyRect.localPosition,FarTargetedPos,Time.deltaTime*Constants.SlidingSpeed);
				yield return new WaitForSeconds (0.01f);
			}
		}
		MyRect.localPosition = TargetedPos;
		if(!Show)
			LevelsContainerGO.SetActive (Show);
		if (Show&&PlayerManager.Instance.ShowLevelOnStart>0) {
			ShowLevelDetails (PlayerManager.Instance.ShowLevelOnStart);
			PlayerManager.Instance.ShowLevelOnStart = 0;
		}
	}


	public void SetDetails ()
	{
		PlayerCoinsTxt.text = PlayerManager.Instance.MyPlayer.Coins.ToString ("00");
		PlayerScoreTxt.text = PlayerManager.Instance.MyPlayer.AbsScore.ToString ("00");
	}

	void SetLevelsItems ()
	{
		for (int i = 0; i <= PlayerManager.Instance.MyPlayer.LevelProgress; i++) {
			if (i >= Constants.NoOfLevels)
				break;
			LevelItemO NewLevelO = new LevelItemO ();
			NewLevelO.IsUnLocked = 1;
			NewLevelO.LevelID = i;
			NewLevelO.StarsNO = PlayerManager.Instance.MyPlayer.MyLevelScores.LevelsStarsNO [i];
			LevelsItems [i].SetLevelDetails (NewLevelO);
		}
	}

	public void BTNSFunction(){
		GameObject CurrentBTNGO = EventSystem.current.currentSelectedGameObject;SoundsManager.Instance.PlaySFX (0);
		string BTNName = CurrentBTNGO.name;
		switch(BTNName){
		case "CloseBTN":
			PopCloseBTN ();
			break;
		case "PlayBTN":
			PlayBTN ();
			break;
		case "OptionsBTN":
			OptionsMenu.Instance.ShowMenu ();
			break;
		case "FBPlayBTN":
			CreateOnlineGame ();
			break;
		case "ScoreBoardBTN":
			ScoreBoardManager.Instance.ShowScoresListForTopPlayers (2);
			break;
		case "LevelScoreBoardBTN":
			ScoreBoardManager.Instance.ShowScoresListForThisLevel (ChoosedLevelNO,2);
			break;
		case "StoreBTN":
			StoreManager.Instance.ShowStore ();
			break;
		case "BackBTN":
			BackBTN ();
			break;
		}
	}

	public void BackBTN ()
	{
		SlideToShow (false);
		MainMenuManager.Instance.SlideToShow (true);
	}

	public void PopCloseBTN ()
	{
		ChoosedLevelNO = 0;
		LevelDetailsWindowScript.IsShowed = false;
		LevelDetailsWindowScript.LevelDetailsPopAnim.SetBool ("IsShow",false);
	}

	public void PlayBTN ()
	{
		LoadingWindow.Instance.ShowMenu ();
		PlayerManager.Instance.SavePlayerData ();
		SceneManager.LoadScene ("GamePlay");
	}

	void CreateOnlineGame ()
	{
		FBManager.Instance.CurrentMenuFriendsShow = 3;
		FBManager.Instance.UpdateMyFBFriendsList ();
	}

	public void ShowLevelDetails(int LevelNO){
		ChoosedLevelNO = LevelNO;
		LevelDetailsWindowScript.IsShowed = true;
		LevelDetailsWindowScript.LevelDetailsPopAnim.SetBool ("IsShow",true);
		LevelDetailsWindowScript.LevelNOTxt.text = "Level "+(LevelNO);
		LevelDetailsWindowScript.LevelScoreTxt.text = PlayerManager.Instance.MyPlayer.MyLevelScores.LevelHighScore [LevelNO].ToString("00");
	}
}