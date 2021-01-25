using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnlineGameManager : MenuFather {

	public static OnlineGameManager Instance;
	public InvitationHolder MyInvitationHolderScript;
	public RoomO CurrentRoom;

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

	protected override void Start(){
		CurrentRoom = new RoomO ();
		TempRoom = new RoomO ();
		MyAnim = GetComponent<Animator> ();
		SceneManager.sceneLoaded += OnSceneFinishLoading;
	}

	private int CurrentSceneNO;
	void OnSceneFinishLoading (Scene arg0, LoadSceneMode arg1)
	{
		CurrentSceneNO = arg0.buildIndex;
	}

	public void RoomUpdates(string NewRoomJSON){
		Debug.Log ("NewRoomJSON\t"+NewRoomJSON);
		JsonUtility.FromJsonOverwrite (NewRoomJSON,CurrentRoom);
		switch(CurrentRoom.RoomStatus){
		case -1:
			FireBaseManager.Instance.DisConnect (true);
			break;
		case 1:
			if (CurrentSceneNO != 1) {
				LevelsManager.Instance.ChoosedLevelNO = CurrentRoom.LevelNO;
				LevelsManager.Instance.IsOnlineGame = true;
				LevelsManager.Instance.IsBonusLevel = false;
				if (PlayerManager.Instance.IsCreator ())
					FireBaseManager.Instance.SetRoomStatus (CurrentRoom.ID,"2");
				LevelsManager.Instance.PlayBTN ();
			}
			break;
		case 2:
			FireBaseManager.Instance.SetOnDissconnectHandle ();
			GameManager.Instance.SetPlayersData (CurrentRoom);
			break;
		case 3:
			GameManager.Instance.SetScores (CurrentRoom);
			break;
		case 4:
			if (ShapesManager.Instance.GameIsOver)
				GameManager.Instance.ShowWaitingWindowUntilOtherPlayerFinishs (true);
			if (OneOfPlayersIsOut ())
				FireBaseManager.Instance.SetRoomStatus (CurrentRoom.ID,"5");
			break;
		case 5:
			ShowTheWinner();
			break;
		}
	}

	private bool OneOfPlayersIsOut(){
		return CurrentRoom.PlayersConnectionsStatus [0] == 0 || CurrentRoom.PlayersConnectionsStatus [1] == 0;
	}

	void ShowTheWinner ()
	{
		GameManager.Instance.ShowWaitingWindowUntilOtherPlayerFinishs (false);
		FireBaseManager.Instance.DisConnect (false);
		GameManager.Instance.GameIsOverCorFun (IamTheWinner());
	}

	private bool IamTheWinner(){
		if (PlayerManager.Instance.IsCreator () && (CurrentRoom.PlayersScores [0] > CurrentRoom.PlayersScores [1]))
			return true;
		else if (!PlayerManager.Instance.IsCreator () && (CurrentRoom.PlayersScores [0] < CurrentRoom.PlayersScores [1]))
			return true;
		else
			return false;
	}

	public void GameIsOver(){
		FireBaseManager.Instance.SetRoomStatus (CurrentRoom.ID,""+(CurrentRoom.RoomStatus+1));
	}

	public void SendInvite(FBFriendO MyFriend){
		RoomO NewRoom = new RoomO ();
		NewRoom.ID = RandomIDGenerator.Instance.GetRandomID ();
		NewRoom.LevelNO = LevelsManager.Instance.ChoosedLevelNO;
		NewRoom.PlayersConnectionsStatus [0] = 1;
		NewRoom.PlayerSIDs [0] = PlayerManager.Instance.MyPlayer.PlayerID;
		NewRoom.PlayersNames [0] = PlayerManager.Instance.MyPlayer.PlayerName;
		NewRoom.PlayerSIDs [1] = MyFriend.id;
		NewRoom.PlayersNames [1] = MyFriend.name;
		FireBaseManager.Instance.CreateGameAndSendInvite (NewRoom);
	}

	public void InvitationReceived(string RoomID){
		Debug.Log ("RoomID\t"+RoomID);
		FireBaseManager.Instance.GetThisRoomO (RoomID);
	}

	private RoomO TempRoom;
	public void ShowInvitationData(RoomO ThisRoom){
		if(PlayerManager.Instance.MyPlayer.ViberationOn==1)
			Handheld.Vibrate ();
		Debug.Log (ThisRoom.ID);
		TempRoom = ThisRoom;
		MyAnim.SetBool ("IsShow",true);
		MyInvitationHolderScript.ChallengeTxt.text = "Wants to challenge you on level " + ThisRoom.LevelNO;
		MyInvitationHolderScript.Name.text = ThisRoom.PlayersNames [0];
		FBManager.Instance.LoadThisPhoto (ThisRoom.PlayerSIDs[0],MyInvitationHolderScript.PlayerIMG);
	}

	public void BTNSFunction(){
		GameObject CurrentBTNGO = EventSystem.current.currentSelectedGameObject;SoundsManager.Instance.PlaySFX (0);
		string BTNName = CurrentBTNGO.name;
		Debug.Log ("BTNName\t" + BTNName);
		switch(BTNName){
		case "RefusedBTN":
			RefuseBTN ();
			break;
		case "AcceptBTN":
			PlayerManager.Instance.OnlinePlayerID = 1;
			MyAnim.SetBool ("IsShow",false);
			FireBaseManager.Instance.AcceptGameInvitation (TempRoom);
			break;
		}
	}

	public override void HideMenu(){
		base.HideMenu ();
		RefuseBTN ();
	}

	public void RefuseBTN ()
	{
		FireBaseManager.Instance.RefusedGameInvitation (TempRoom.ID);
		MyAnim.SetBool ("IsShow",false);
	}

	public void CancelInvitation(){
		FireBaseManager.Instance.RefusedGameInvitationByCreator (CurrentRoom);
	}
}