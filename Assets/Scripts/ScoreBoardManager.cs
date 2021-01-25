using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScoreBoardManager : MenuFather {

	public static ScoreBoardManager Instance;
	public GameObject ContainerGO;
	public ScoreItemsList ScoresList;
	public Text ScoresTitleTxt,MyPlayerRank;
	private int CurrentDailyWeeklyOrAllTime,CurrentChoosedLevel;
	public int CurrentChoosedType;

	void Awake(){
		Instance=this;
	}

	public void SetPlayerRank(int Rank){
		if (Rank <= 100)
			MyPlayerRank.text = "My Rank #" + Rank.ToString ("00");
		else
			MyPlayerRank.text = "My Rank > 100";
	}

	public void BTNSFunction(){
		GameObject CurrentBTNGO =EventSystem.current.currentSelectedGameObject;SoundsManager.Instance.PlaySFX (0);
		string BTNName = CurrentBTNGO.name;
		switch (BTNName) {
		case "DailyBTN":
			FriendsScoreBoardBTN (0);
			break;
		case "WeeklyBTN":
			FriendsScoreBoardBTN (1);
			break;
		case "AllTimeBTN":
			FriendsScoreBoardBTN (2);
			break;
		case "CloseBTN":
			CurrentChoosedLevel = 0;
			CurrentDailyWeeklyOrAllTime = 0;
			HideMenu ();
			break;
		}
	}

	void FriendsScoreBoardBTN (int ID)
	{
		if (CurrentDailyWeeklyOrAllTime == ID)
			return;
		if (CurrentChoosedType == 0)
			ShowScoresListForThisLevel (CurrentChoosedLevel, ID);
		else if (CurrentChoosedType == 1)
			ShowScoresListForTopPlayers (ID);
		else 
			ShowScoresListForPlayerFriendsNOW (PlayerManager.Instance.MyPlayer.MyFBFriends,ID,CurrentChoosedLevel);
	}

	public void ShowScoresListForThisLevel(int LevelNO,int DailyWeeklyOrAllTime){
		ScoresList.DisActivateAllItems ();
		CurrentChoosedLevel = LevelNO;
		CurrentDailyWeeklyOrAllTime = DailyWeeklyOrAllTime;
		FireBaseManager.Instance.GetLevelHighScores (LevelNO,DailyWeeklyOrAllTime);
	}

	public void ShowScoresListForTopPlayers(int DailyWeeklyOrAllTime){
		ScoresList.DisActivateAllItems ();
		CurrentDailyWeeklyOrAllTime = DailyWeeklyOrAllTime;
		FireBaseManager.Instance.GetTopPlayersScores (DailyWeeklyOrAllTime);
	}

	public void ShowScoresListForPlayerFriends(int DailyWeeklyOrAllTime,int LevelNO){
		CurrentDailyWeeklyOrAllTime = DailyWeeklyOrAllTime;
		CurrentChoosedLevel = LevelNO;
		FBManager.Instance.CurrentMenuFriendsShow = 2;
		FBManager.Instance.UpdateMyFBFriendsList ();
	}

	public void ShowScoresListForPlayerFriendsNOW(List<FBFriendO> FriendSIDList,int DailyWeeklyOrAllTime,int LevelNO){
		ScoresList.DisActivateAllItems ();
		CurrentDailyWeeklyOrAllTime = DailyWeeklyOrAllTime;
		FireBaseManager.Instance.GetPlayerFriendsScores (FriendSIDList,DailyWeeklyOrAllTime,LevelNO);
	}

	public void ShowScoresListForPlayerFriendsNOW(){
		ScoresList.DisActivateAllItems ();
		FireBaseManager.Instance.GetPlayerFriendsScores (PlayerManager.Instance.MyPlayer.MyFBFriends,CurrentDailyWeeklyOrAllTime,CurrentChoosedLevel);
	}

	public void ShowThisScoresList(List<ScoreO> ScoresItems,int dailyWeeklyOrAllTime,int LevelNO,bool Friends){
		LoadingWindow.Instance.HideMenu ();
		if (Friends) {
			if (LevelNO == 0)
				ScoresTitleTxt.text = "Top " + GetTimeInterval (dailyWeeklyOrAllTime) + " Friends";
			else
				ScoresTitleTxt.text = GetTimeInterval (dailyWeeklyOrAllTime) + " Friends Scores";
		} else {
			if (LevelNO == 0)
				ScoresTitleTxt.text = GetTimeInterval (dailyWeeklyOrAllTime) + " Top Players";
			else
				ScoresTitleTxt.text = GetTimeInterval (dailyWeeklyOrAllTime);
		}
		ShowMenu ();
		int MyScoreRectChildInt = -1;
		for (int i = 0; i < ScoresItems.Count; i++) {
			ScoresList.AddItem (ScoresItems [i]);
			if (ScoresItems [i].PlayerID == PlayerManager.Instance.MyPlayer.PlayerID)
				MyScoreRectChildInt = i;
		}
		if(MyScoreRectChildInt!=-1)
			StartCoroutine (SnapToCor(ScoresList.MyItemsGO[MyScoreRectChildInt].GetComponent<RectTransform>()));
	}

	public IEnumerator SnapToCor(RectTransform target)
	{
		yield return new WaitForSeconds (0.1f);
		Canvas.ForceUpdateCanvases();
		ScoresList.ContentRect.anchoredPosition =
			(Vector2)ScoresList.transform.InverseTransformPoint(ScoresList.ContentRect.position)
			- (Vector2)ScoresList.transform.InverseTransformPoint(target.position);
		ScoresList.ContentRect.anchoredPosition = new Vector2 (0,ScoresList.ContentRect.anchoredPosition.y-400);
	}

	private string GetTimeInterval (int dailyWeeklyOrAllTime)
	{
		if (dailyWeeklyOrAllTime == 0)
			return "Daily";
		else if (dailyWeeklyOrAllTime == 1)
			return "Weekly";
		else
			return "All Time";
	}

}