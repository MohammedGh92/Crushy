using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerManager : MonoBehaviour {

	public static PlayerManager Instance;
	[HideInInspector]
	public PlayerO MyPlayer;
	[HideInInspector]
	public int ShowLevelOnStart,OnlinePlayerID,BigAdCounter;
	const string glyphs= "abcdefghijklmnopqrstuvwxyz0123456789";
	public bool TestSave,InternetAvailability;

	void Awake(){
		Singlton ();
//		ClearAllData ();
		LoadPlayerData ();
	}

	void Singlton ()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy (gameObject);
		DontDestroyOnLoad (this);
	}

	void Update(){
		if (TestSave) {
			SavePlayerData ();
			TestSave = false;
		}
	}

	void Start(){
		if (IsNewDay ()) {
			MyPlayer.SendGiftsCounter = 0;
			MyPlayer.ReceiveGiftsCounter = 0;
			MyPlayer.ComulativeDailyScore = 0;
			MyPlayer.MyLevelScores.LevelDailyScore = new float[Constants.NoOfLevels];
			MyPlayer.NoOfDaysOnLastAdd = DateTime.UtcNow.DayOfYear;
			MyPlayer.YearOfLastAdd = DateTime.UtcNow.Year;
		}
		if (IsNewWeek ()) {
			MyPlayer.ComulativeWeeklyScore = 0;
			MyPlayer.NoOfDaysOnWeekOnLastAdd = 0;
			MyPlayer.MyLevelScores.LevelWeeklyScore = new float[Constants.NoOfLevels];
			MyPlayer.NoOfDaysOnWeekOnLastAdd = DateTime.UtcNow.DayOfYear+7;
			if (MyPlayer.NoOfDaysOnWeekOnLastAdd >= 360)
				MyPlayer.NoOfDaysOnWeekOnLastAdd -= 360;
			MyPlayer.YearOfLastAdd = DateTime.UtcNow.Year;
		}
		if(IsNewDayOneDayPass())
			StartCoroutine (CheckDailyGiftCor());
		SavePlayerData ();
	}

	bool IsNewDayOneDayPass ()
	{
		return Mathf.Abs (DateTime.UtcNow.DayOfYear - MyPlayer.NoOfDaysOnLastAdd) == 1;
	}

	IEnumerator CheckDailyGiftCor ()
	{
		MyPlayer.DailyGiftsCounter++;
		if (MyPlayer.DailyGiftsCounter >= 5)
			MyPlayer.DailyGiftsCounter = 0;
		yield return new WaitForSeconds (0.1f);
		DailyRewardManager.Instance.ShowDailyProgress ();
		yield return new WaitForSeconds (1);
		if (MyPlayer.DailyGiftsCounter == 4)
			GiftsShowerManager.Instance.Give5DaysCoinsGift (true);
		else if(MyPlayer.DailyGiftsCounter==3)
			GiftsShowerManager.Instance.Give5DaysCoinsGift (false);
		else
			GiftsShowerManager.Instance.GiveDailyBomb();
	}

	private bool IsNewDay(){
		return (DateTime.UtcNow.Year!=MyPlayer.YearOfLastAdd)||(DateTime.UtcNow.DayOfYear!=MyPlayer.NoOfDaysOnLastAdd);
	}

	private bool IsNewWeek(){
		return (DateTime.UtcNow.Year!=MyPlayer.YearOfLastAdd)||(DateTime.UtcNow.DayOfYear>MyPlayer.NoOfDaysOnWeekOnLastAdd);
	}

	public void SavePlayerData(){
		string MyPlayerJson = JsonUtility.ToJson (MyPlayer);
		PlayerPrefs.SetString ("MyPlayer",MyPlayerJson);
		PlayerPrefs.Save ();
	}

	public void LoadPlayerData(){
		string MyPlayerJson = PlayerPrefs.GetString ("MyPlayer","");
		if (MyPlayerJson.Length > 5)
			JsonUtility.FromJsonOverwrite (MyPlayerJson, MyPlayer);
		else {
			MyPlayer = new PlayerO ();
			MyPlayer.PlayerID = GetRandomID ();
		}
	}

	public void ClearAllData(){
		PlayerPrefs.DeleteAll ();
		PlayerPrefs.Save ();
	}

	public void AddScore(int LevelNO,float Score){
		MyPlayer.NoOfDaysOnLastAdd = DateTime.UtcNow.DayOfYear;
		if(MyPlayer.NoOfDaysOnWeekOnLastAdd==0)
			MyPlayer.NoOfDaysOnWeekOnLastAdd = DateTime.UtcNow.DayOfYear+ (6-(int)DateTime.UtcNow.DayOfWeek);
		if (MyPlayer.NoOfDaysOnWeekOnLastAdd >= 360)
			MyPlayer.NoOfDaysOnWeekOnLastAdd -= 360;
		MyPlayer.YearOfLastAdd = DateTime.UtcNow.Year;
		MyPlayer.ComulativeScore += Score;
		MyPlayer.ComulativeDailyScore += Score;
		MyPlayer.ComulativeWeeklyScore += Score;
		ScoreO TempScore = new ScoreO ();
		TempScore.PlayerID = MyPlayer.PlayerID;
		TempScore.LevelNO = LevelNO;
		TempScore.Name = MyPlayer.PlayerName;
		TempScore.Score = Score;
		TempScore.PlayerProgress = PlayerManager.Instance.MyPlayer.LevelProgress;
		if (Score > MyPlayer.MyLevelScores.LevelDailyScore [LevelNO]) {
			MyPlayer.MyLevelScores.LevelDailyScore [LevelNO] = Score;
			FireBaseManager.Instance.AddLevelScore (TempScore,0);
		}
		if (Score > MyPlayer.MyLevelScores.LevelWeeklyScore [LevelNO]) {
			MyPlayer.MyLevelScores.LevelWeeklyScore [LevelNO] = Score;
			FireBaseManager.Instance.AddLevelScore(TempScore,1);
		}
		if (Score > MyPlayer.MyLevelScores.LevelHighScore [LevelNO]) {
			GameManager.Instance.ShowHighScore ();
			float DiffPoints = Score - MyPlayer.MyLevelScores.LevelHighScore [LevelNO];
			MyPlayer.AbsScore += DiffPoints;
			MyPlayer.MyLevelScores.LevelHighScore [LevelNO] = Score;
			FireBaseManager.Instance.AddLevelScore(TempScore,2);
		}
		TempScore.Score = MyPlayer.ComulativeDailyScore;
		FireBaseManager.Instance.AddPlayerScore (TempScore,0);
		TempScore.Score = MyPlayer.ComulativeWeeklyScore;
		FireBaseManager.Instance.AddPlayerScore (TempScore,1);
		TempScore.Score = MyPlayer.ComulativeScore;
		FireBaseManager.Instance.AddPlayerScore (TempScore,2);
	}

	public bool HaveFBID(){
		return MyPlayer.PlayerID.Length >= 13;
	}

	public void ToggleNotificationOnOrOff(){
		if (MyPlayer.NotificationsOn == 0)
			MyPlayer.NotificationsOn = 1;
		else
			MyPlayer.NotificationsOn = 0;
		SavePlayerData ();
	}

	public void ToggleViberationOnOrOff ()
	{
		if (MyPlayer.ViberationOn == 0)
			MyPlayer.ViberationOn = 1;
		else
			MyPlayer.ViberationOn = 0;
		SavePlayerData ();
	}

	public string GetRandomID(){
		string myString = "";
		int charAmount = UnityEngine.Random.Range(7, 11); //set those to the minimum and maximum length of your string
		for(int i=0; i<charAmount; i++)
			myString += glyphs[UnityEngine.Random.Range(0, glyphs.Length)];
		return myString;
	}

	public void SaveThisFBFriendsList(List<FBFriendO> ThisFriendsList){
		MyPlayer.MyFBFriends.Clear ();
		MyPlayer.MyFBFriends = ThisFriendsList;
		SavePlayerData ();
		switch(FBManager.Instance.CurrentMenuFriendsShow){
		case 0:
			FriendsGiftManager.Instance.ShowSendGiftsFriendsList ();
			break;
		case 1:
			FriendsGiftManager.Instance.ShowReceivedGiftsFriendsList ();
			break;
		case 2:
			ScoreBoardManager.Instance.ShowScoresListForPlayerFriendsNOW ();
			break;
		case 3:
			SelectFBFriend.Instance.ShowThisFriendsList ();
			break;
		}
	}

	public bool IsCreator(){
		return OnlinePlayerID == 0;
	}

	public bool HaveToken(){
		return MyPlayer.FCMToken.Length > 5;
	}
}