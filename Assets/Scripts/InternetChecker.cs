using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternetChecker : MonoBehaviour {

	[HideInInspector]
	private const int RequestRate = 10;

	private void CheckInternetNow ()
	{
		if (Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork &&
			Application.internetReachability != NetworkReachability.ReachableViaCarrierDataNetwork)
			PlayerManager.Instance.InternetAvailability = false;
		else
			PlayerManager.Instance.InternetAvailability = true;
	}

	void Start(){
		StartChecking ();
	}

	public void StartChecking()
	{
		InvokeRepeating ("CheckInternetNow",0,RequestRate);
	}

}
