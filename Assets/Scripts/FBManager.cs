using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;
using System.IO;
using System;
using SimpleJSON;

public class FBManager : MonoBehaviour
{
	public static FBManager Instance;
	public int CurrentMenuFriendsShow;

	void Awake ()
	{
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

	public void FBLogInBTNClick ()
	{
		if (!FB.IsInitialized)
			FB.Init (InitCallback, OnHideUnity);
		else
			InitCallback ();
	}

	private void InitCallback ()
	{
		if (FB.IsInitialized) {
			if (FB.IsLoggedIn)
				AuthCallback (null);
			else
				LogInFun ();
		}
	}

	private void LogInFun ()
	{
		var perms = new List<string> (){ "public_profile", "email"};
		FB.LogInWithReadPermissions (perms, AuthCallback);
	}

	private void AuthCallback (ILoginResult result)
	{
		if (FB.IsLoggedIn)
			FetchFBProfile ();
	}

	private void FetchFBProfile ()
	{
		FB.API ("/me?fields=id,name", HttpMethod.GET, FetchProfileCallback, new Dictionary<string,string> (){ });
	}

	Dictionary<string, object> FBUserDetails,FriendFBUserDetails;

	private void FetchProfileCallback (IGraphResult result)
	{
		FBUserDetails = (Dictionary<string,object>)result.ResultDictionary;
		PlayerManager.Instance.MyPlayer.PlayerName = FBUserDetails ["name"].ToString ();
		PlayerManager.Instance.MyPlayer.PlayerID = FBUserDetails ["id"].ToString ();
		PlayerManager.Instance.SavePlayerData ();
		MainMenuManager.Instance.SetNameAndFPPhoto ();
		FireBaseManager.Instance.SetMyPlayerOnlineData ();
		OptionsMenu.Instance.RefreshOptionsIcons ();
		ConnectFB.Instance.HideMenu();
		GiftsShowerManager.Instance.GiveACoinsGift (1);
	}

	public void LoadThisPhoto (string FBID, RawImage ImgToLoadOn)
	{
		if (FBID.Length <= 13)
			return;
		WWW PhotoLink = new WWW ("https://graph.facebook.com/" + FBID + "/picture?type=normal");
		if (PhotoLink.url.Length < 5)
			return;
		StartCoroutine (LoadThisPhotoCorRawimg (PhotoLink.url, ImgToLoadOn));
	}

	private IEnumerator LoadThisPhotoCorRawimg (string PhotoLink, RawImage ImgToLoadOn)
	{
		if (!LoadSavedIMG (PhotoLink, ImgToLoadOn)) {
			WWW www = new WWW (PhotoLink);
			yield return www;
			if (www.texture.width < 30)
				yield break;  
		ImgToLoadOn.texture = www.texture;
			File.WriteAllBytes (Application.dataPath + "/" + GetFBIDFromThisLink(PhotoLink), www.texture.EncodeToPNG ());
			yield return new WaitForEndOfFrame ();
			if (LoadSavedIMG (PhotoLink, ImgToLoadOn))
				yield break;
		}
	
	}

	private bool LoadSavedIMG (string PhotoLink, RawImage ImgToLoadOn)
	{
		Texture2D tex = new Texture2D (2, 2);
		try {
			tex.LoadImage (File.ReadAllBytes (Application.dataPath + "/" + GetFBIDFromThisLink (PhotoLink)));
		} catch {
			return false;
		}
		ImgToLoadOn.texture = tex;
		return true;
	}

	string GetFBIDFromThisLink (string photoLink)
	{
		string[] ReturnFBID = photoLink.Split(new string[] { "/"}, StringSplitOptions.None);
		return ReturnFBID[3];
	}

	public void AddFriend ()
	{
		Debug.Log ("AddFriend");
	}

	public void UpdateMyFBFriendsList(){
		Debug.Log ("UpdateMyFBFriendsList");
		if (!FB.IsInitialized)
			FB.Init (GetFriendsListCallBack, OnHideUnity);
		else
			GetFriendsListCallBack ();
	}

	private void GetFriendsListCallBack ()
	{
		Debug.Log ("GetFriendsListCallBack");
		if (FB.IsInitialized) {
			if (FB.IsLoggedIn)
				GetFriendsAuthCallback (null);
			else
				GetFriendsLogInFun ();
		}
	}

	private void GetFriendsAuthCallback (ILoginResult result)
	{
		Debug.Log ("GetFriendsAuthCallback");
		if (FB.IsLoggedIn)
			FetchGetFriends ();
	}

	private void GetFriendsLogInFun ()
	{
		var perms = new List<string> (){ "public_profile", "email" };
		FB.LogInWithReadPermissions (perms, GetFriendsAuthCallback);
	}

	private void FetchGetFriends ()
	{
		Debug.Log ("FetchGetFriends ");
		FB.API ("/me?fields=friends.fields(name,id)", HttpMethod.GET, FetchFriendsCallback);
	}

	private void FetchFriendsCallback (IGraphResult result)
	{
		Debug.Log ("FetchFriendsCallback");
		FBFriendsO MyFBFreinds = new FBFriendsO ();
		JsonUtility.FromJsonOverwrite (result.RawResult,MyFBFreinds);
		List<FBFriendO> NewFBFriends = new List<FBFriendO> ();
		for (int i = 0; i < MyFBFreinds.friends.data.Count; i++)
			NewFBFriends.Add (MyFBFreinds.friends.data[i]);
	}

	private void OnHideUnity (bool isGameShown)
	{
	}
}