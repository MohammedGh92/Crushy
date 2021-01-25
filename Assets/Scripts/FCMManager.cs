using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FCMManager : MonoBehaviour {

	public IEnumerator Start() {
		if (Application.platform == RuntimePlatform.WindowsEditor)
			yield break;Debug.Log ("Start");
		yield return new WaitForSeconds (1);
		Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
	}

	public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
		if (!PlayerManager.Instance.HaveToken ()) {
			PlayerManager.Instance.MyPlayer.FCMToken = token.Token;
			PlayerManager.Instance.SavePlayerData ();
		}
	}

	void OnDestroy(){
		if (Application.platform == RuntimePlatform.WindowsEditor)
			return;
		try{
		Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
		}catch{
		}
	}
}