using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerO
{
	public string PlayerID,PlayerName,FCMToken;
	public float ComulativeScore,AbsScore,AbsDailyScore,AbsWeeklyScore,ComulativeDailyScore,ComulativeWeeklyScore,
	NoOfDaysOnLastAdd,NoOfDaysOnWeekOnLastAdd,YearOfLastAdd,Coins,MusicOn, SoundsOn;
	public LevelScores MyLevelScores;
	public int NotificationsOn,LevelProgress,SendGiftsCounter,ViberationOn,ReceiveGiftsCounter,DailyGiftsCounter,RateTaken
	,GamesCounter,FBCounter,FBFirstTimeShowed;
	public int[] Bombs;
	public List<FBFriendO> MyFBFriends;

	public PlayerO(){
		FBFirstTimeShowed = 0;
		RateTaken = 0;
		DailyGiftsCounter = -1;
		FCMToken = "";
		PlayerName = "Player";
		ComulativeScore = 0;
		Coins = 200;
		AbsScore = 0;
		AbsDailyScore = 0;
		AbsWeeklyScore = 0;
		ComulativeDailyScore = 0;
		ComulativeWeeklyScore = 0;
		LevelProgress = 1;
		NoOfDaysOnLastAdd = 0;
		NoOfDaysOnWeekOnLastAdd = 0;
		YearOfLastAdd = 0;
		FBCounter = 7;
		NotificationsOn = 1;
		SoundsOn = 1;
		MusicOn = 1;
		ViberationOn = 1;
		MyLevelScores = new LevelScores ();
		Bombs = new int[7]{3,3,3,0,0,0,0};
		MyFBFriends = new List<FBFriendO> ();
		ReceiveGiftsCounter = 0;
		SendGiftsCounter = 0;
	}
}