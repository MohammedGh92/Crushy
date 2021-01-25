using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;
	public ShapesManager ShapesManagerScript;
	public Transform CamTrans;
	public GameObject YouLoseWindow, GameOverLineGO, PauseMenuGO;
	private float AddNewLineSpeed,StoredScore;
	public int RowsNo, BombRandomRange, BlackBombRandomRange, ColoredSquareRandomRange;
	[Range (2, 7)]
	public int ColorsNumber;
	public int LevelNO;
	private Transform SquareFatherGO;
	private float TimerValue, Timer;
	[HideInInspector]
	public bool IsTotorialLevel,IsBonusLevel, ReviveWindowShowed;
	[HideInInspector]
	public int RowsGroupNo, NoOfEmptyRows,ReviveCoins;
	private static bool Once;
	public WinWindowHolder WinWindowsHolderScript;
	public GameUIHolder GameUIHolderScript;
	[HideInInspector]
	public int[] BombsForCurrentLevel;
	private int StarsCounter;
	public OnlineUIHolder OnlineUIHolderScript;
	public AlertZone MyAlarmZone;
	[HideInInspector]
	public int tutorialClickPhase;
	public List<float> CurrentLevelScoresList;
	public KingCrush MyKingCrushScript;

	void Awake ()
	{
		Instance = this;
		Application.targetFrameRate = 30;
		Screen.SetResolution (720,1280,true);
	}

	void Start ()
	{
		SetInitials ();
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape))
			BackBTN ();
	}

	void BackBTN ()
	{
		if (ShapesManagerScript.GameIsOver) {
			Time.timeScale = 1;
			SceneManager.LoadScene ("MainScene");
			return;
		}
		if (PauseMenuGO.activeSelf) {
			Time.timeScale = 1;
			GameUIHolderScript.PauseMenuAnim.SetBool ("IsShow",false);
		} else if (GameUIHolderScript.ReviveWindowAnim.GetBool ("IsShow")) {
			GiveUp ();
		} else {
			Time.timeScale = 0;
			GameUIHolderScript.PauseMenuAnim.SetBool ("IsShow",true);
		}
	}

	public void KingCrushHere(){
		MyKingCrushScript.ShowIt ();
	}

	public void ToBlue(){
		ShapesManagerScript.ToBlue ();
	}

	private bool FirstRefreshRank;
	private int LastI;
	public void RefreshYourRank ()
	{
		if (LevelsManager.Instance.IsOnlineGame)
			return;
		if (LevelsManager.Instance.IsBonusLevel)
			return;
		if (LevelsManager.Instance.IsTotorial)
			return;
		float CurrentScore = ShapesManagerScript.score;
		if (StoredScore > CurrentScore)
			CurrentScore = StoredScore;
		for (int i = 0; i < CurrentLevelScoresList.Count; i++) {
			if (i == CurrentLevelScoresList.Count-1) {
				if (i == 99)
					GameUIHolderScript.RankNOTxt.text = "#>100";
				else
					GameUIHolderScript.RankNOTxt.text = "#" + (int)(i + 1);
				return;
			}
			if (CurrentScore > CurrentLevelScoresList [i]) {
				if (LastI == i)
					return;
				else {
					LastI = i;
				}
				if (!FirstRefreshRank) {
					GameUIHolderScript.RankNOTxt.text = "#" + i;
					FirstRefreshRank = true;
					return;
				}
				GameUIHolderScript.RankNOTxt.text = "#" + (int)(i + 1);
				return;
			}
		}
	}

	private void GiveUp(){
		ReviveWindowShowed = true;
		Time.timeScale = 1;
		GameIsOver (false);
	}

	void SetInitials ()
	{
		try {
			if (LevelsManager.Instance.IsOnlineGame) {
				GameUIHolderScript.ScoreRect.GetComponent<Animator> ().enabled = false;
				GameUIHolderScript.CoinsRect.GetComponent<Animator> ().enabled = false;
				GameUIHolderScript.ScoreRect.localScale = new Vector3 (0, 0, 0);
				GameUIHolderScript.CoinsRect.localScale = new Vector3 (0, 0, 0);
				OnlineUIHolderScript.ContainerGO.SetActive (true);
			}
		} catch {
		}
		try {
			IsBonusLevel = LevelsManager.Instance.IsBonusLevel;
			LevelNO = LevelsManager.Instance.ChoosedLevelNO;
		} catch {
		}
		if (LevelsManager.Instance.IsTotorial || LevelsManager.Instance.IsBonusLevel) {
			GameUIHolderScript.LevelNOTxt.text = "-";
			GameUIHolderScript.RankNOTxt.text = "-";
		} else
			GameUIHolderScript.LevelNOTxt.text = "" + LevelNO;
		ReviveCoins = LevelNO * 100;
		GameUIHolderScript.ReviveTxt.text = "Using Coins " + ReviveCoins;
		SetLevelDifficaulty ();
		if (IsBonusLevel)
			RowsNo = 13;
		Constants.Rows = RowsNo;
		if (IsBonusLevel) {
			Timer = 10;
			RowsGroupNo = 13;
			CamTrans.position = new Vector3 (0, (Constants.Rows * ShapesManagerScript.CandySize.y) - 4.54f, -10);
			AddNewLineSpeed = 0.1f;
		} else {
			Timer = 45+(NoOfFivesInLevelNO()*18);
			RowsGroupNo = 1;
			AddNewLineSpeed = (Timer / RowsNo) * TimerFactor (RowsNo);
		}
		GameOverLineGO.SetActive (!IsBonusLevel);
		SquareFatherGO = ShapesManagerScript.SquareFatherGO.transform;
		if (!IsBonusLevel)
			for (int i = 0; i < ColorsNumber; i++)
				ShapesManagerScript.SelectionBombs [i].SetActive (true);
		if (!IsBonusLevel) {
			CamTrans.position = new Vector3 (0, (Constants.Rows * ShapesManagerScript.CandySize.y) + 4.68f, -10);
			StartCoroutine (ShowSquaresLinesCor ());
		}
		StartCoroutine (TimerCountDownCor ());
		if (LevelsManager.Instance.IsTotorial)
			StartCoroutine (TotorialCor ());
		else
			SetBombsStuff ();
		if (LevelsManager.Instance.IsOnlineGame)
			return;
		CurrentLevelScoresList = new List<float> ();
		FireBaseManager.Instance.GetLevelHighScoresForCurrentLevelScores (LevelNO);
		StoredScore = PlayerManager.Instance.MyPlayer.MyLevelScores.LevelHighScore [LevelNO];
		StartCoroutine (TestCor());
	}

	private IEnumerator TestCor(){
		yield return new WaitForSeconds (0.5f);
	}

	public void SetScoresListAndREfresh(List<float> ThisCurrentLevelScores){
		GameManager.Instance.CurrentLevelScoresList = ThisCurrentLevelScores;
		RefreshYourRank ();
	}

	public void ShowBombTut(){
		StartCoroutine (ShowBombTutCor());
	}

	public void ShowNextSimpleTut(){
		for (int i = 0; i < GameUIHolderScript.TutorialGO.Length; i++)
			GameUIHolderScript.TutorialGO [i].SetActive (false);
		if (tutorialClickPhase > 5) {
			GameIsOver (true);
			Time.timeScale = 1;
			return;
		}
		Time.timeScale = 0;
		GameUIHolderScript.TutorialGO [tutorialClickPhase].SetActive (true);
		tutorialClickPhase += 1;
	}

	private IEnumerator ShowBombTutCor(){
		Time.timeScale = 1;
		GameUIHolderScript.TutorialGO [0].SetActive (false);
		tutorialClickPhase = 1;
		yield return new WaitForSeconds (1);
		Time.timeScale = 0;
		tutorialClickPhase = 1;
		GameUIHolderScript.TutorialGO [1].SetActive (true);
	}

	private IEnumerator TotorialCor(){
		IsTotorialLevel = true;
		SetBombsStuff ();
		tutorialClickPhase = -1;
		yield return new WaitForSeconds (2.5f);
		tutorialClickPhase = 0;
		Time.timeScale = 0;
		GameUIHolderScript.TutorialGO [0].SetActive (true);
	}

	void SetBombsStuff ()
	{
		BombsForCurrentLevel = new int[7];
		if (!IsTotorialLevel) {
			for (int i = 0; i < 7; i++)
				BombsForCurrentLevel [i] = PlayerManager.Instance.MyPlayer.Bombs [i];
		} else {
			BombsForCurrentLevel = new int[7]{3,3,3,0,0,0,0};
		}
		for (int i = 0; i < ColorsNumber; i++) {
			if (BombsForCurrentLevel [i] > 0) {
				GameUIHolderScript.Bombs [i].SetActive (true);
				GameUIHolderScript.BombsNOTxt [i].text = BombsForCurrentLevel [i].ToString ();
			}
		}
	}

	void SetLevelDifficaulty ()
	{
		int ThisNoOfFiver = NoOfFivesInLevelNO ();
		ColorsNumber = 3;
		while (ColorsNumber < 8 && ThisNoOfFiver != 0) {
			ThisNoOfFiver--;
			ColorsNumber += 1;
		}
		if (ColorsNumber > 7)
			ColorsNumber = 7;
		if (IsBonusLevel)
			return;
		RowsNo = GetTheRowsNO (LevelNO);
		ColoredSquareRandomRange += LevelNO;
		if (ColoredSquareRandomRange > 80)
			ColoredSquareRandomRange = 80;
		BombRandomRange += LevelNO;
		if (BombRandomRange > 80)
			BombRandomRange = 80;
	}

	int GetTheRowsNO (int NoOFLevel)
	{
		int ReturnedNO = 38;
		for (int i = 0; i < NoOFLevel; i++)
			ReturnedNO += Get10Percentofthis (ReturnedNO);
		return ReturnedNO;
	}

	private int Get10Percentofthis(int NO){
		return Mathf.CeilToInt(NO*0.03f);
	}

	private int NoOfFivesInLevelNO ()
	{
		int ThisLEvelNO = LevelNO;
		int FivesCounter = 0;
		while (ThisLEvelNO > 0) {
			ThisLEvelNO -= 5;
			if (ThisLEvelNO > 0)
				FivesCounter++;
		}
		return FivesCounter;
	}

	private int NoOfTensInLevelNO ()
	{
		int ThisLEvelNO = LevelNO;
		int FivesCounter = 0;
		while (ThisLEvelNO > 0) {
			ThisLEvelNO -= 10;
			if (ThisLEvelNO > 0)
				FivesCounter++;
		}
		return FivesCounter;
	}

	float TimerFactor (int rowsNo)
	{
		if (rowsNo <= 10)
			return 1;
		return 1 - ((int)(rowsNo / 10) * 0.004f);
	}

	private IEnumerator ShowSquaresLinesCor ()
	{
		Vector3 TargetedCamTrans = new Vector3 (0, CamTrans.transform.position.y - Constants.DragValue, -10);
		int RowsCounter = RowsNo;
		while (RowsCounter > 0) {
			TargetedCamTrans = new Vector3 (0, CamTrans.transform.position.y - Constants.DragValue, -10);
			StartCoroutine (MoveFromTo (CamTrans, CamTrans.transform.position, TargetedCamTrans, Constants.CamSpeed));
			RowsCounter--;
			yield return new WaitForSeconds (AddNewLineSpeed);
			CreateNewRowsGroup ();
		}
	}

	private  IEnumerator MoveFromTo (Transform objectToMove, Vector3 a, Vector3 b, float speed)
	{
		float step = (speed / (a - b).magnitude) * Time.fixedDeltaTime;
		float t = 0;
		while (t <= 1) {
			t += step;
			objectToMove.position = Vector3.Lerp (a, b, t);
			yield return new WaitForFixedUpdate ();
		}
		objectToMove.position = b;
	}	

	private IEnumerator TimerCountDownCor ()
	{
		TimerValue = Timer;
		if (!IsBonusLevel)
			TimerValue += 5;
		while (TimerValue > 0) {
			TimerValue -= 1;
			if (TimerValue <= 5)
				SoundsManager.Instance.PlaySFX (13);
			GameUIHolderScript.TimerTxt.text = TimerValue.ToString ("00");
			if (TimerValue <= 0) {
				GameIsOver (true);
				yield break;
			}
			yield return new WaitForSeconds (1);
		}
	}

	public void CreateNewRowsGroup ()
	{
		if (ShapesManagerScript.CurrentRowNo >= Constants.Rows)
			return;
		ShapesManagerScript.CreateRowsGroup ();
	}

	public void ShowWaitingWindowUntilOtherPlayerFinishs (bool ShowOrHide)
	{
		OnlineUIHolderScript.WaitingOtherPlayerToFinishGOAnim.SetBool ("IsShow",ShowOrHide);
	}

	private bool GameIsOverOnce;
	public void GameIsOver (bool WinOrLose)
	{
		if (GameIsOverOnce)
			return;
		GameIsOverOnce = true;
		if (IsTotorialLevel) {
			StartCoroutine (GameIsOverCorTutorial());
			return;
		}
		SoundsManager.Instance.SFXAudioSource.Stop ();
		ShapesManagerScript.GameIsOver = true;
		if(PlayerManager.Instance.MyPlayer.ViberationOn==1)
			Handheld.Vibrate ();
		if(WinOrLose)
			SoundsManager.Instance.PlayBackGroundMusicSound(5);
		else
			SoundsManager.Instance.PlayBackGroundMusicSound(6);
		ShapesManagerScript.ScoreShowerScript.HideAll ();
		GameUIHolderScript.ReviveWindowAnim.SetBool ("IsShow", false);
		try{
			if (LevelsManager.Instance.IsOnlineGame)
				OnlineGameManager.Instance.GameIsOver ();
		}catch{
		}
		StopAllCoroutines ();
		if (WinOrLose) {
			SetStars ();
			if (!IsBonusLevel && PlayerManager.Instance.MyPlayer.MyLevelScores.LevelsStarsNO [LevelNO] < StarsCounter)
				PlayerManager.Instance.MyPlayer.MyLevelScores.LevelsStarsNO [LevelNO] = StarsCounter;
			if (!IsBonusLevel && PlayerManager.Instance.MyPlayer.LevelProgress <= LevelNO)
				PlayerManager.Instance.MyPlayer.LevelProgress += 1;
			if (!LevelsManager.Instance.IsOnlineGame) {
				ShapesManagerScript.Coins = (int)(0.2f * ShapesManagerScript.Coins);
			}
			WinWindowsHolderScript.BonusBTNGO.SetActive (NextIsBonusLevel());
			WinWindowsHolderScript.NextBTNGO.SetActive (!NextIsBonusLevel());
			WinWindowsHolderScript.NextBTNGO.GetComponent<Button>().interactable = (LevelNO+1 < Constants.NoOfLevels);
		}
		try{
			if (IsBonusLevel||LevelsManager.Instance.IsOnlineGame) {
				WinWindowsHolderScript.RestartBTNGO.GetComponent<Button> ().interactable = false;
				WinWindowsHolderScript.BonusBTNGO.SetActive(false);
			}}catch{
		}
		RefreshUsedBombs ();
		GameIsOverCorFun (WinOrLose);
	}

	private IEnumerator GameIsOverCorTutorial(){
		LevelsManager.Instance.IsBonusLevel = false;
		LevelsManager.Instance.IsTotorial = false;
		IsTotorialLevel = false;
		IsBonusLevel = false;
		if (PlayerManager.Instance.MyPlayer.LevelProgress == 0) {
			PlayerManager.Instance.MyPlayer.LevelProgress = 1;
			PlayerManager.Instance.SavePlayerData ();
		}
		PopMSG.Instance.ShowPopMSG ("Very Good, You are ready to GO!");
		yield return new WaitForSeconds (2.5f);
		PlayerManager.Instance.ShowLevelOnStart = 1;
		SceneManager.LoadScene ("MainScene");
	}

	private bool NextIsBonusLevel(){
		return !IsBonusLevel&&LevelNO % 5 == 0;
	}

	void RefreshUsedBombs ()
	{
		if(!IsTotorialLevel)
		for (int i = 0; i < BombsForCurrentLevel.Length; i++)
			PlayerManager.Instance.MyPlayer.Bombs [i] = BombsForCurrentLevel [i];
	}

	public void ShowHighScore(){
		WinWindowsHolderScript.HighScoreGO.SetActive (true);
	}

	public void GameIsOverCorFun(bool WinOrLose){
		StartCoroutine (GameIsOverCor(WinOrLose));
	}

	private IEnumerator GameIsOverCor(bool WinOrLose){
		if (WinOrLose) {
			WinWindowsHolderScript.FireWorksGO.SetActive (IsBonusLevel || StarsCounter >= 2);
			ShapesManagerScript.AddEmptyRowsBonus ();
			ShapesManagerScript.ScoreShowerScript.ShowEmptyRowsScores (NoOfEmptyRows, (LevelNO * 100).ToString ());
			yield return new WaitForSeconds ((NoOfEmptyRows * 0.35f));
			WinWindowsHolderScript.WinSpriteWindowAnim.SetBool ("IsShow", true);
			yield return new WaitForSeconds (2f);
			WinWindowsHolderScript.WinUIWindowAnim.SetBool ("IsShow", true);
			WinWindowsHolderScript.ScoreTxt.text = ShapesManagerScript.score.ToString ("00");
			WinWindowsHolderScript.CoinsTxt.text = ShapesManagerScript.Coins.ToString ("00");
			PlayerManager.Instance.AddScore (LevelNO,float.Parse(WinWindowsHolderScript.ScoreTxt.text));
			PlayerManager.Instance.MyPlayer.Coins += int.Parse(WinWindowsHolderScript.CoinsTxt.text);
			yield return new WaitForSeconds (0.5f);
			for (int i = 1; i <= StarsCounter; i++) {
				if (i > 3)
					break;
				SoundsManager.Instance.PlaySFX (i + 4);
				WinWindowsHolderScript.StarsGO [i - 1].SetActive (true);
				yield return new WaitForSeconds (0.35f);
			}
			PlayerManager.Instance.SavePlayerData ();
			if (StarsCounter > 0) {
				GiftsShowerManager.Instance.GiveABombGift ();
				yield return new WaitForSeconds (1.75f);
				AdsManager.Instance.ShowBigAd ();
			} else {
				yield return new WaitForSeconds (0.15f);
				AdsManager.Instance.ShowBigAd ();
			}
		} else {
			PlayerManager.Instance.SavePlayerData ();
			GameUIHolderScript.LoseUIWindowAnim.SetBool ("IsShow", true);
			yield return new WaitForSeconds (1f);
			AdsManager.Instance.ShowBigAd ();
		}
	}

	private int[] LevelStarsScores;
	private void SetStars ()
	{
		Debug.Log ("SetStars ");
		if (IsBonusLevel) {
			StarsCounter = 3;
			return;
		}
		LevelStarsScores = new int[3];
		LevelStarsScores[0] = 10000+(LevelNO*5000);
		LevelStarsScores[1] = 20000+(LevelNO*5000);
		LevelStarsScores[2] = 30000+(LevelNO*5000);
		for (int i = 0; i < LevelStarsScores.Length; i++)
			Debug.Log ("LevelStarsScores\t"+LevelStarsScores [i]);
		if (ShapesManagerScript.score >= LevelStarsScores[0])
			StarsCounter++;
		if (ShapesManagerScript.score >= LevelStarsScores[1])
			StarsCounter++;
		if (ShapesManagerScript.score >= LevelStarsScores[2])
			StarsCounter++;
	}

	public void BTNSFunction ()
	{
		GameObject CurrentBTNGO = EventSystem.current.currentSelectedGameObject;SoundsManager.Instance.PlaySFX (0);
		string BTNName = CurrentBTNGO.name;
		switch (BTNName) {
		case "RestartBTN":
			PlayerManager.Instance.SavePlayerData ();
			Time.timeScale = 1;
			PlayerManager.Instance.ShowLevelOnStart = LevelsManager.Instance.ChoosedLevelNO;
			SceneManager.LoadScene ("MainScene");
			break;
		case "MenuBTN":
			PlayerManager.Instance.SavePlayerData ();
			Time.timeScale = 1;
			SceneManager.LoadScene ("MainScene");
			break;
		case "ResumeBTN":
			Time.timeScale = 1;
			GameUIHolderScript.PauseMenuAnim.SetBool ("IsShow",false);
			break;
		case "NextBTN":
			LevelsManager.Instance.IsBonusLevel = false;
			IsBonusLevel = false;
			LevelsManager.Instance.ChoosedLevelNO += 1;
			PlayerManager.Instance.ShowLevelOnStart = LevelsManager.Instance.ChoosedLevelNO;
			SceneManager.LoadScene ("MainScene");
			break;
		case "BonusBTN":
			LevelsManager.Instance.IsBonusLevel = true;
			SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
			break;
		case "PauseBTN":
			Time.timeScale = 0;
			GameUIHolderScript.PauseMenuAnim.SetBool ("IsShow",true);
			break;
		case "WatchVideoBTN":
			ReviveWindowShowed = true;
			AdsManager.Instance.ShowVideo ();
			break;
		case "UsingCoinsBTN":
			ReviveWindowShowed = true;
			if (PlayerManager.Instance.MyPlayer.Coins >= ReviveCoins) {
				PlayerManager.Instance.MyPlayer.Coins -= ReviveCoins;
				PlayerManager.Instance.SavePlayerData ();
				RevivePlayerNow ();
			}
			break;	
		case "GiveUPBTN":
			GiveUp ();
			break;
		}
	}


	bool CanMatchPhase ()
	{
		return (tutorialClickPhase == 0 || tutorialClickPhase > 4);
	}

	bool CanMatchBombPhase ()
	{
		return (tutorialClickPhase == 1 || tutorialClickPhase > 4);
	}

	public void SelectBomb ()
	{
		if (IsTotorialLevel && tutorialClickPhase == 1) {
			Time.timeScale = 1;
			GameUIHolderScript.TutorialGO [1].SetActive (false);
		}
		if (IsTotorialLevel && !CanMatchBombPhase ())
			return;
		GameObject CurrentBTNGO = EventSystem.current.currentSelectedGameObject;SoundsManager.Instance.PlaySFX (0);
		int BombID = CurrentBTNGO.transform.GetSiblingIndex ();
		if (BombsForCurrentLevel [BombID] <= 0)
			return;
		ShapesManagerScript.FindMatchesAndCollapse (BombID);
		BombsForCurrentLevel [BombID] -= 1;
		GameUIHolderScript.BombsNOTxt [BombID].text = BombsForCurrentLevel [BombID].ToString ();
		GameUIHolderScript.Bombs [BombID].gameObject.SetActive (BombsForCurrentLevel [BombID] > 0);
		RefreshUsedBombs ();
	}

	public void RevivePlayerNow ()
	{
		StartCoroutine (RevivePlayerNowCor ());
	}

	private IEnumerator RevivePlayerNowCor ()
	{
		Time.timeScale = 1;
		ShapesManagerScript.DestroyFirst7RowsForRevive ();
		GameUIHolderScript.ReviveWindowAnim.SetBool ("IsShow",false);
		yield return new WaitForSeconds (0.1f);
		MyAlarmZone.AlarmOn = 0;
		yield return new WaitForSeconds (0.5f);
		GameOverLineGO.SetActive (true);
	}

	public void SetPlayersData (RoomO CurrentRoom)
	{
		FBManager.Instance.LoadThisPhoto (CurrentRoom.PlayerSIDs [0], OnlineUIHolderScript.PlayersIMG [0]);
		FBManager.Instance.LoadThisPhoto (CurrentRoom.PlayerSIDs [1], OnlineUIHolderScript.PlayersIMG [1]);
		if (PlayerManager.Instance.IsCreator ())
			FireBaseManager.Instance.SetRoomStatus (CurrentRoom.ID, "3");
	}

	public void SetScores (RoomO CurrentRoom)
	{
		OnlineUIHolderScript.PlayersTxts [0].text = CurrentRoom.PlayersScores [0] + "";
		OnlineUIHolderScript.PlayersTxts [1].text = CurrentRoom.PlayersScores [1] + "";
	}

	public void ShowReviveWindow(){
		GameUIHolderScript.UsingCoinsBTN.interactable = PlayerManager.Instance.MyPlayer.Coins >= ReviveCoins;
		GameUIHolderScript.ReviveWatchVideoBTN.interactable = PlayerManager.Instance.InternetAvailability;
		GameUIHolderScript.ReviveWindowAnim.SetBool ("IsShow",true);
	}
}