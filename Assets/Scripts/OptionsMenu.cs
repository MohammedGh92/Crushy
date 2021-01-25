using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsMenu : MenuFather {

	public static OptionsMenu Instance;
	public GameObject ContainerGO,FBBTNGO;
	public Image[] OptionsBTNS;

	void Awake(){
		Instance=this;
	}

	protected override void Start(){
		base.Start ();
		RefreshOptionsIcons();
	}

	public void RefreshOptionsIcons(){
		SetIMGAlpha (OptionsBTNS[0],PlayerManager.Instance.MyPlayer.MusicOn>0,false);
		SetIMGAlpha (OptionsBTNS[1],PlayerManager.Instance.MyPlayer.SoundsOn>0,false);
		SetIMGAlpha (OptionsBTNS[2],PlayerManager.Instance.MyPlayer.NotificationsOn==1,false);
		SetIMGAlpha (OptionsBTNS[3],PlayerManager.Instance.MyPlayer.ViberationOn==1,false);
	}

	private void SetIMGAlpha(Image ThisIMG,bool HighOrLow,bool BTNActivation){
		if (HighOrLow)
			ThisIMG.color = Color.white;
		else
			ThisIMG.color = new Color (1,1,1,0.5f);
		if(BTNActivation)
			ThisIMG.GetComponent<Button> ().interactable = HighOrLow;
	}

	public void BTNSFunction(){
		GameObject CurrentBTNGO = EventSystem.current.currentSelectedGameObject;SoundsManager.Instance.PlaySFX (0);
		string BTNName = CurrentBTNGO.name;
		switch(BTNName){
		case "MusicBTN":
			SoundsManager.Instance.ToggleMusicOnOrOff ();
			break;
		case "SoundBTN":
			SoundsManager.Instance.ToggleSoundsOnOrOff ();
			break;
		case "NotificationsBTN":
			PlayerManager.Instance.ToggleNotificationOnOrOff ();
			break;
		case "ViberationBTN":
			PlayerManager.Instance.ToggleViberationOnOrOff ();
			break;
		case "FBLoginBTN":
			FBManager.Instance.FBLogInBTNClick ();
			break;
		case "AddFreindBTN":
			#if UNITY_ANDROID
			NativeShare.Share("Try Crushy and challenge your friends now: https://play.google.com/store/apps/details?id="+Application.identifier, "", "https://play.google.com/store/apps/details?id="+Application.identifier, "", "text/plain", true, "");
			#elif UNITY_IOS
			NativeShare.Share("Try Kotshena and challenge your friends now:"+Constants.AppStoreLink, "", Constants.AppStoreLink, "", "text/plain", true, "");
			#endif
			break;
		case "GameInfoBTN":
			Application.OpenURL ("https://play.google.com/store/apps/details?id="+Application.identifier);
			break;
		case "CloseBTN":
			HideMenu ();
			break;
		}
		RefreshOptionsIcons ();
	}
}