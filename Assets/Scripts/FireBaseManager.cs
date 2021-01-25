using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Unity.Editor;
using System.Linq;
using SimpleJSON;
using System;

public class FireBaseManager : MonoBehaviour {

	public static FireBaseManager Instance;
	[HideInInspector]
	public DatabaseReference reference;
	private string DataBaseUrl = "https://crushy-e4145.firebaseio.com/";
	private int CurrentScene;
	private List<ScoreO> CurrentCreatedScoresList;
	private List<FBFriendO> CurrentCreatedPlayerFBFriendsList;
	public bool GoOffLine;

	void Awake(){
		Singlton ();
	}

	void Singlton ()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy (gameObject);
		DontDestroyOnLoad (this);
	}

	void Start ()
	{
		InitilizeFireBase ();
		CurrentCreatedScoresList = new List<ScoreO> ();
		SetHandle ();
		SceneManager.sceneLoaded += OnSceneFinishLoading;
		if (GoOffLine)
			reference.Database.GoOffline ();
	}

	private void SetHandle(){
		reference.Child ("Players").Child (PlayerManager.Instance.MyPlayer.PlayerID).Child("CurrentOnlineRoom").ValueChanged += MyCurrentOnlineRoom;
	}

	private string CurrentRoomID;
	void MyCurrentOnlineRoom (object sender, ValueChangedEventArgs e)
	{
		if (e.DatabaseError != null)
			return;
		try{
		string RoomID = e.Snapshot.GetRawJsonValue ().Replace ("\"","");
		if (RoomID != "0") {
			CurrentRoomID = RoomID;
			OnlineGameManager.Instance.InvitationReceived (RoomID);
			}}catch{
		}
	}
		
	void OnSceneFinishLoading (Scene arg0, LoadSceneMode arg1)
	{
		CurrentScene = arg0.buildIndex;
	}

	public void AddPlayerScore(ScoreO ThisScore,int DailyWeeklyOrAllTime){
		try{
		string ScoreJson = JsonUtility.ToJson (ThisScore);
			reference.Child("Scores").Child ("TopPlayersScores").Child (GetTimeInterval(DailyWeeklyOrAllTime)).Child(""+GetScoreID(ThisScore)).SetRawJsonValueAsync (ScoreJson);
		}catch(Exception E){Debug.Log (E.Message);
		}
	}


	public void SetMyPlayerOnlineData ()
	{
		reference.Child ("Players").Child (PlayerManager.Instance.MyPlayer.PlayerID).Child ("FCMToken").SetValueAsync (PlayerManager.Instance.MyPlayer.FCMToken);
	}

	public void AddLevelScore(ScoreO ThisScore,int DailyWeeklyOrAllTime){
		try{
			string ScoreJson = JsonUtility.ToJson (ThisScore);
			reference.Child("Scores").Child("LevelsScores").Child (GetTimeInterval(DailyWeeklyOrAllTime)).Child (""+ThisScore.LevelNO).Child(""+GetScoreID(ThisScore)).SetRawJsonValueAsync (ScoreJson);
		}catch(Exception E){
			Debug.Log (E.Message);
		}
	}

	private string GetScoreID(ScoreO ThisScore){
			return ThisScore.PlayerID;
	}

	public void GetPlayerFriendsScores(List<FBFriendO> PlayerFriendSID,int DailyWeeklyOrAllTime,int LevelNO){
		ScoreBoardManager.Instance.CurrentChoosedType = 2;
		CurrentCreatedScoresList.Clear ();
		CurrentCreatedScoresList.Add (MyScoreO(DailyWeeklyOrAllTime,LevelNO));
		for (int i = 0; i < PlayerFriendSID.Count; i++)
			GetThisPlayerScore (DailyWeeklyOrAllTime,PlayerFriendSID[i].id,LevelNO,i==PlayerFriendSID.Count-1);
	}

	private ScoreO MyScoreO(int DailyWeeklyOrAllTime,int LevelNO){
		ScoreO ReturnedScoreO = new ScoreO ();
		ReturnedScoreO.PlayerID = PlayerManager.Instance.MyPlayer.PlayerID;
		ReturnedScoreO.Name = PlayerManager.Instance.MyPlayer.PlayerName;
		ReturnedScoreO.LevelNO = LevelNO;
		if (LevelNO > 0) {
			if (DailyWeeklyOrAllTime == 0)
				ReturnedScoreO.Score = PlayerManager.Instance.MyPlayer.MyLevelScores.LevelDailyScore[LevelNO];
			else if (DailyWeeklyOrAllTime == 1)
				ReturnedScoreO.Score = PlayerManager.Instance.MyPlayer.MyLevelScores.LevelWeeklyScore[LevelNO];
			else
				ReturnedScoreO.Score = PlayerManager.Instance.MyPlayer.MyLevelScores.LevelHighScore[LevelNO];
		} else {
			if (DailyWeeklyOrAllTime == 0)
				ReturnedScoreO.Score = PlayerManager.Instance.MyPlayer.AbsDailyScore;
			else if (DailyWeeklyOrAllTime == 1)
				ReturnedScoreO.Score = PlayerManager.Instance.MyPlayer.AbsWeeklyScore;
			else
				ReturnedScoreO.Score = PlayerManager.Instance.MyPlayer.AbsScore;
		}
		return ReturnedScoreO;
	}

	private void GetThisPlayerScore(int DailyWeeklyOrAllTime,string PlayeRID,int LevelNO,bool IsLastOne){
		if(PlayerManager.Instance.InternetAvailability)
			LoadingWindow.Instance.ShowMenu ();
		bool TopPlayersOrLevels = (LevelNO == 0);
		if (TopPlayersOrLevels) {
			FirebaseDatabase.DefaultInstance
				.GetReference ("Scores").Child ("TopPlayersScores").Child (GetTimeInterval (DailyWeeklyOrAllTime)).Child (PlayeRID).GetValueAsync ().ContinueWith (task => {
				if (task.IsFaulted) {
					Debug.Log (task.Result.ToString ());
				} else if (task.IsCompleted) {
					DataSnapshot snapshot = task.Result;
						ScoreO ThisScore = new ScoreO ();
					JsonUtility.FromJsonOverwrite (snapshot.GetRawJsonValue (), ThisScore);
					CurrentCreatedScoresList.Add (ThisScore);
						if(IsLastOne)
						{
							List<ScoreO> CurrentCreatedScoresListOrdered = CurrentCreatedScoresList.OrderBy(ScoreO=>ScoreO.Score).ToList();
							CurrentCreatedScoresListOrdered.Reverse();
							ScoreBoardManager.Instance.ShowThisScoresList(CurrentCreatedScoresListOrdered,DailyWeeklyOrAllTime,LevelNO,true);
							for(int i=0;i<CurrentCreatedScoresListOrdered.Count;i++)
								if(CurrentCreatedScoresListOrdered[i].PlayerID==PlayerManager.Instance.MyPlayer.PlayerID)
								{
									ScoreBoardManager.Instance.SetPlayerRank(i+1);
									break;
								}
						}
				}
			});
		} else {
			FirebaseDatabase.DefaultInstance
				.GetReference("Scores").Child("LevelsScores").Child (GetTimeInterval(DailyWeeklyOrAllTime)).Child(""+LevelNO).Child(PlayeRID).GetValueAsync ().ContinueWith (task => {
					if (task.IsFaulted) {
						Debug.Log (task.Result.ToString ());
					} else if (task.IsCompleted) {
						DataSnapshot snapshot = task.Result;
						ScoreO ThisScore = new ScoreO ();
						JsonUtility.FromJsonOverwrite (snapshot.GetRawJsonValue (), ThisScore);
						CurrentCreatedScoresList.Add (ThisScore);
						if(IsLastOne){
							List<ScoreO> CurrentCreatedScoresListOrdered = CurrentCreatedScoresList.OrderBy(ScoreO=>ScoreO.Score).ToList();
							CurrentCreatedScoresListOrdered.Reverse();
							ScoreBoardManager.Instance.ShowThisScoresList(CurrentCreatedScoresListOrdered,DailyWeeklyOrAllTime,LevelNO,true);
							for(int i=0;i<CurrentCreatedScoresListOrdered.Count;i++)
								if(CurrentCreatedScoresListOrdered[i].PlayerID==PlayerManager.Instance.MyPlayer.PlayerID)
								{
									ScoreBoardManager.Instance.SetPlayerRank(i+1);
									break;
								}
						}
					}
				});
		}
	}

	public void GetTopPlayersScores(int DailyWeeklyOrAllTime){
		if(PlayerManager.Instance.InternetAvailability)
			LoadingWindow.Instance.ShowMenu ();
		ScoreBoardManager.Instance.CurrentChoosedType = 1;
		FirebaseDatabase.DefaultInstance
			.GetReference("Scores").Child("TopPlayersScores").Child (GetTimeInterval(DailyWeeklyOrAllTime)).OrderByChild("Score").LimitToLast(100).GetValueAsync ().ContinueWith (task => {
				if (task.IsFaulted) {
					Debug.Log (task.Result.ToString ());
				} else if (task.IsCompleted) {
					DataSnapshot snapshot = task.Result;
					List<ScoreO> ScoresItems = new List<ScoreO>();
					int PlayersCounter=0;
					bool RankSetted=false;
					foreach(var child in snapshot.Children){
						PlayersCounter++;
						ScoreO ThisScore = new ScoreO ();
						JsonUtility.FromJsonOverwrite (child.GetRawJsonValue (), ThisScore);
						ScoresItems.Add(ThisScore);
					}
					ScoresItems.Reverse();
					for(int i=0;i<ScoresItems.Count;i++)
						if(ScoresItems[i].PlayerID==PlayerManager.Instance.MyPlayer.PlayerID)
					{
							ScoreBoardManager.Instance.SetPlayerRank(i+1);
							RankSetted=true;
							break;
					}
					if(!RankSetted)
						ScoreBoardManager.Instance.SetPlayerRank(1000);
					ScoreBoardManager.Instance.ShowThisScoresList(ScoresItems,DailyWeeklyOrAllTime,0,false);
				}
			});
	}

	public void GetLevelHighScores(int LevelNO,int DailyWeeklyOrAllTime){
		if(PlayerManager.Instance.InternetAvailability)
			LoadingWindow.Instance.ShowMenu ();
		ScoreBoardManager.Instance.CurrentChoosedType = 0;
		FirebaseDatabase.DefaultInstance
			.GetReference("Scores").Child("LevelsScores").Child (GetTimeInterval(DailyWeeklyOrAllTime)).Child(""+LevelNO).OrderByChild("Score").LimitToLast(100).GetValueAsync ().ContinueWith (task => {
				if (task.IsFaulted) {
					Debug.Log (task.Result.ToString ());
				} else if (task.IsCompleted) {
					DataSnapshot snapshot = task.Result;
					List<ScoreO> ScoresItems = new List<ScoreO>();
					bool RankSetted=false;
					int PlayersCounter=0;
					foreach(var child in snapshot.Children){
						PlayersCounter++;
						ScoreO ThisScore = new ScoreO ();
						JsonUtility.FromJsonOverwrite (child.GetRawJsonValue (), ThisScore);
						ScoresItems.Add(ThisScore);
					}
					ScoresItems.Reverse();
					for(int i=0;i<ScoresItems.Count;i++){
						Debug.Log("ScoresItems[i].PlayerID\t"+ScoresItems[i].Score);
						if(ScoresItems[i].PlayerID==PlayerManager.Instance.MyPlayer.PlayerID)
						{
							ScoreBoardManager.Instance.SetPlayerRank(i+1);
							RankSetted=true;
							break;
						}
					}
					if(!RankSetted)
						ScoreBoardManager.Instance.SetPlayerRank(1000);
					ScoreBoardManager.Instance.ShowThisScoresList(ScoresItems,DailyWeeklyOrAllTime,LevelNO,false);
				}
			});
	}

	public void GetLevelHighScoresForCurrentLevelScores(int LevelNO){
		if(CurrentScene==0&&PlayerManager.Instance.InternetAvailability)
			LoadingWindow.Instance.ShowMenu ();
		ScoreBoardManager.Instance.CurrentChoosedType = 0;
		FirebaseDatabase.DefaultInstance
			.GetReference("Scores").Child("LevelsScores").Child ("AllTime").Child(""+LevelNO).OrderByChild("Score").LimitToLast(100).GetValueAsync ().ContinueWith (task => {
				if (task.IsFaulted) {
					Debug.Log (task.Result.ToString ());
				} else if (task.IsCompleted) {
					DataSnapshot snapshot = task.Result;
					List<float> ThisCurrentLevelScores =  new List<float>(100);
					bool RankSetted=false;
					int PlayersCounter=0;
					foreach(var child in snapshot.Children){
						PlayersCounter++;
						ScoreO ThisScore = new ScoreO ();
						JsonUtility.FromJsonOverwrite (child.GetRawJsonValue (), ThisScore);
						ThisCurrentLevelScores.Add(ThisScore.Score);
					}
					ThisCurrentLevelScores.Reverse();
					GameManager.Instance.SetScoresListAndREfresh(ThisCurrentLevelScores);
				}
			});
	}

	private void InitilizeFireBase ()
	{
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl (DataBaseUrl);
		reference = FirebaseDatabase.DefaultInstance.RootReference;
	}

	private string GetTimeInterval (int dailyWeeklyOrAllTime)
	{
		if (dailyWeeklyOrAllTime == 0)
			return "DailyScores";
		else if (dailyWeeklyOrAllTime == 1)
			return "WeeklyScores";
		else
			return "AllTime";
	}

	public void VersionCheck(){
		FirebaseDatabase.DefaultInstance
			.GetReference ("CurrentVersion").GetValueAsync ().ContinueWith (task => {
				if (task.IsFaulted) {
					Debug.Log (task.Result.ToString ());
				} else if (task.IsCompleted) {
					DataSnapshot snapshot = task.Result;
					string OnlineVersion = snapshot.GetRawJsonValue ().Replace("\"","");
					if(Application.version!=OnlineVersion)
						MainMenuManager.Instance.UpdateNow();
				}
			});
	}

	public void GetPlayerGiftsList(){
		FirebaseDatabase.DefaultInstance
			.GetReference ("Players").Child(PlayerManager.Instance.MyPlayer.PlayerID).Child("Gifts").GetValueAsync ().ContinueWith (task => {
				if (task.IsFaulted) {
					Debug.Log (task.Result.ToString ());
				} else if (task.IsCompleted) {
					DataSnapshot snapshot = task.Result;
					List<GiftO> GiftsItems = new List<GiftO>();
					foreach(var Child in snapshot.Children){
						GiftO ThisGift = new GiftO();
						JsonUtility.FromJsonOverwrite(Child.GetRawJsonValue(),ThisGift);
						ThisGift.SendOrReceived=1;
						GiftsItems.Add(ThisGift);
					}
					FriendsGiftManager.Instance.ShowThisGiftsList(GiftsItems);
				}
			});
	}

	public void SendGift(GiftO ThisGift){
		Debug.Log ("SendGift\t"+ThisGift.SID);
		reference.Child ("Players").Child (ThisGift.RID).Child("Gifts").Child(ThisGift.GiftID).SetRawJsonValueAsync (JsonUtility.ToJson (ThisGift));
	}

	public void RemoveThisGift(string GiftID){
		Debug.Log ("RemoveThisGift\t"+GiftID);
		reference.Child ("Players").Child (PlayerManager.Instance.MyPlayer.PlayerID).Child("Gifts").Child(GiftID).RemoveValueAsync();
		FriendsGiftManager.Instance.ShowReceivedGiftsFriendsList ();
	}

	public void CreateGameAndSendInvite(RoomO NewRoom){
		PlayerManager.Instance.OnlinePlayerID = 0;
		Debug.Log ("NewRoom\t"+JsonUtility.ToJson(NewRoom));
		reference.Child ("Rooms").Child(NewRoom.ID).SetRawJsonValueAsync (JsonUtility.ToJson(NewRoom));
		reference.Child ("Players").Child(NewRoom.PlayerSIDs[1]).Child ("CurrentOnlineRoom").SetValueAsync (NewRoom.ID);
		reference.Child ("Rooms").Child (NewRoom.ID).ValueChanged += RoomUpdates;
		MainMenuManager.Instance.ShowWaitingOnlinePlayer (true);
		NotifiyThisPlayer (NewRoom.PlayerSIDs[1]);
	}

	private void NotifiyThisPlayer(string FBID){
		FirebaseDatabase.DefaultInstance
			.GetReference ("Players").Child(FBID).Child("FCMToken").GetValueAsync ().ContinueWith (task => {
				if (task.IsFaulted) {
					Debug.Log (task.Result.ToString ());
				} else if (task.IsCompleted) {
					DataSnapshot snapshot = task.Result;
					StartCoroutine(SendNotifyCor(snapshot.GetRawJsonValue()));
				}
			});
	}

	private IEnumerator SendNotifyCor (string OtherPlayerToken)
	{
		WWW JsonUrl = new WWW ("http://servizio.biz/api/sendNotification/"+
			Application.productName+"/"+PlayerManager.Instance.MyPlayer.PlayerName+" Wants to challenge you!/"+OtherPlayerToken);
		yield return JsonUrl;
	}

	public void GetThisRoomO(string RoomID){
		FirebaseDatabase.DefaultInstance
			.GetReference ("Rooms").Child(RoomID).GetValueAsync ().ContinueWith (task => {
				if (task.IsFaulted) {
					Debug.Log (task.Result.ToString ());
				} else if (task.IsCompleted) {
					DataSnapshot snapshot = task.Result;
					RoomO CurrentRoom = new RoomO();
					JsonUtility.FromJsonOverwrite(snapshot.GetRawJsonValue(),CurrentRoom);
					OnlineGameManager.Instance.ShowInvitationData(CurrentRoom);
				}
			});
	}

	public void RefusedGameInvitation(string RoomID){
		reference.Child ("Players").Child(PlayerManager.Instance.MyPlayer.PlayerID).Child ("CurrentOnlineRoom").SetValueAsync ("0");
		SetRoomStatus (RoomID,"-1");
	}

	public void RefusedGameInvitationByCreator(RoomO ThisRoom){
		reference.Child ("Players").Child(ThisRoom.PlayerSIDs[1]).Child ("CurrentOnlineRoom").SetValueAsync ("0");
		SetRoomStatus (OnlineGameManager.Instance.CurrentRoom.ID,"-1");
	}

	public void SetRoomStatus(string RoomID,string NewRoomStatus){
		Debug.Log ("SetRoomStatus");
		reference.Child ("Rooms").Child(RoomID).Child("RoomStatus").SetValueAsync (NewRoomStatus);
	}

	public void AcceptGameInvitation(RoomO MyNewRoom){
		if (CurrentRoomID == "0") {
			PopMSG.Instance.ShowPopMSG ("Game canceled by creator!");
			return;
		}
		reference.Child ("Players").Child (PlayerManager.Instance.MyPlayer.PlayerID).Child ("CurrentOnlineRoom").SetValueAsync ("0");
			MyNewRoom.PlayersConnectionsStatus [1] = 1;
			MyNewRoom.RoomStatus = 1;
			reference.Child ("Rooms").Child (MyNewRoom.ID).SetRawJsonValueAsync (JsonUtility.ToJson (MyNewRoom));
			reference.Child ("Rooms").Child (MyNewRoom.ID).ValueChanged += RoomUpdates;
	}

	void RoomUpdates (object sender, ValueChangedEventArgs e){
		if (e.DatabaseError != null)
			return;		
		OnlineGameManager.Instance.RoomUpdates(e.Snapshot.GetRawJsonValue ());
	}

	public void UpdateMyScore(int NewScore){
		reference.Child ("Rooms").Child(OnlineGameManager.Instance.CurrentRoom.ID).
		Child("PlayersScores").Child(PlayerManager.Instance.OnlinePlayerID.ToString()).SetValueAsync (NewScore+"");
	}

	public void SetOnDissconnectHandle ()
	{
		RoomO CurrentRoom = new RoomO ();
		CurrentRoom = OnlineGameManager.Instance.CurrentRoom;
		reference.Child ("Rooms").Child (CurrentRoom.ID).OnDisconnect ().Cancel ();
		Debug.Log ("Currenroomid\t"+CurrentRoom.ID+"||"+PlayerManager.Instance.OnlinePlayerID);
		reference.Child ("Rooms").Child (CurrentRoom.ID).Child("PlayersConnectionsStatus").Child (""+PlayerManager.Instance.OnlinePlayerID).OnDisconnect ().SetValue ("0");
		reference.Child ("Rooms").Child (CurrentRoom.ID).Child("RoomStatus").OnDisconnect ().SetValue ("4");
	}

	public void DisConnect(bool LoadMainScene){
		reference.Child ("Rooms").Child (OnlineGameManager.Instance.CurrentRoom.ID).OnDisconnect ().Cancel ();
		reference.Child ("Rooms").Child (OnlineGameManager.Instance.CurrentRoom.ID).ValueChanged -= RoomUpdates;
		if(PlayerManager.Instance.IsCreator())
			reference.Child ("Rooms").Child (OnlineGameManager.Instance.CurrentRoom.ID).RemoveValueAsync();
		if(LoadMainScene&&CurrentScene!=0)
			SceneManager.LoadScene ("MainScene");
		if (CurrentScene == 0)
			MainMenuManager.Instance.ShowWaitingOnlinePlayer (false);
	}

	void OnDestroy(){
		if (CurrentScene != 1)
			return;
		if (OnlineGameManager.Instance.CurrentRoom.PlayersConnectionsStatus [0] == 0 || OnlineGameManager.Instance.CurrentRoom.PlayersConnectionsStatus [1] == 0) {
			try{reference.Child ("Rooms").Child (OnlineGameManager.Instance.CurrentRoom.ID).ValueChanged -= RoomUpdates;}catch(Exception e){
			}
			try{
				reference.Child ("Rooms").Child (OnlineGameManager.Instance.CurrentRoom.ID).RemoveValueAsync();}catch(Exception e){
			}
		}
	}
}