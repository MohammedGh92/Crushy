using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.SceneManagement;

public class AdsManager : MonoBehaviour
{
	public static AdsManager Instance;
	public string AndroidAppID;
	public string AndroidBannerID, AndroidInterstintialID, AndroidVideoID;

	public string IOSAppID;
	public string IOSBannerID, IOSInterstintialID, IOSVideoID;

	private BannerView bannerView;
	private InterstitialAd interstitialAd;
	public RewardBasedVideoAd rewardBasedVideo;
	private string CurrentScene;

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

	public void Start ()
	{
		#if UNITY_ANDROID
		string appId = AndroidAppID;
		#elif UNITY_IPHONE
		string appId = IOSAppID;
		#else
		string appId = "unexpected_platform";
		#endif
		MobileAds.Initialize (appId);
		RequestBanner ();
		RequestRewardBasedVideo ();
		SceneManager.sceneLoaded += OnSceneFinishLoading;
		rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
		rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
		rewardBasedVideo.OnAdFailedToLoad += HandleOnAdFailedToLoad;
		rewardBasedVideo.OnAdLoaded += HandleOnAdLoaded;
	}

	public bool RewardVideoAdIsLoaded;
	void HandleOnAdLoaded (object sender, System.EventArgs e)
	{
		RewardVideoAdIsLoaded = true;
	}

	void HandleOnAdFailedToLoad (object sender, AdFailedToLoadEventArgs e)
	{
		RewardVideoAdIsLoaded = false;
		Debug.Log ("HandleRewardBasedVideoClosed");
		RequestRewardBasedVideo ();
	}

	void HandleRewardBasedVideoClosed (object sender, System.EventArgs e)
	{
		RewardVideoAdIsLoaded = false;
		Debug.Log ("HandleRewardBasedVideoClosed");
		RequestRewardBasedVideo ();
	}

	void OnSceneFinishLoading (Scene arg0, LoadSceneMode arg1)
	{
		CurrentScene = arg0.name;
		if (CurrentScene == "GamePlay") {
			RequestInterstitial ();
			RequestRewardBasedVideo ();
		} else
			RequestRewardBasedVideo ();
		if (GameManager.Instance.IsTotorialLevel)
			bannerView.Hide ();
		else
		bannerView.Show();
	}

	/// <summary>
	/// Banner
	/// </summary>
	private void RequestBanner ()
	{
		#if UNITY_ANDROID
		string adUnitId = AndroidBannerID;
		#elif UNITY_IPHONE
			string adUnitId = IOSBannerID;
		#endif
		bannerView = new BannerView (adUnitId, AdSize.Banner, AdPosition.Bottom);
		AdRequest request = new AdRequest.Builder ().Build ();
		bannerView.LoadAd (request);
		bannerView.Show ();
	}

	/// <summary>
	/// Big AD
	/// </summary>
	private void RequestInterstitial ()
	{
		#if UNITY_ANDROID
		string adUnitId = AndroidInterstintialID;
		#elif UNITY_IPHONE
		string adUnitId = IOSInterstintialID;
		#endif

		// Initialize an InterstitialAd.
		interstitialAd = new InterstitialAd (adUnitId);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder ().Build ();
		// Load the interstitial with the request.
		interstitialAd.LoadAd (request);
	}

	public void ShowBigAd ()
		{
		if (GameManager.Instance.IsTotorialLevel)
			return;
		PlayerManager.Instance.BigAdCounter++;
		if (PlayerManager.Instance.BigAdCounter >= 3)
			PlayerManager.Instance.BigAdCounter = 0;
		if (PlayerManager.Instance.BigAdCounter == 1 && interstitialAd.IsLoaded ()) {
		interstitialAd.Show ();Debug.Log ("Show Big Ad");
		}
	}


	/// <summary>
	/// Video
	/// </summary>
	private void RequestRewardBasedVideo ()
	{
		rewardBasedVideo = RewardBasedVideoAd.Instance;
		#if UNITY_ANDROID
		string adUnitId = AndroidVideoID;
		#elif UNITY_IPHONE
			string adUnitId = IOSVideoID;
		#endif
		AdRequest request = new AdRequest.Builder ().Build ();
		rewardBasedVideo.LoadAd (request, adUnitId);
	}

	void HandleRewardBasedVideoRewarded (object sender, Reward e)
	{
		RewardVideoAdIsLoaded = false;
		RequestRewardBasedVideo ();
		Debug.Log ("HandleRewardBasedVideoRewarded");
		if (CurrentScene == "GamePlay")
			GameManager.Instance.RevivePlayerNow ();
		else
			FortuneWheelManager.Instance.TurnWheel ();
	}

	public void ShowVideo ()
	{
		if (RewardVideoAdIsLoaded) {
			RewardVideoAdIsLoaded = false;
			rewardBasedVideo.Show ();
			RequestRewardBasedVideo ();
		}
	}
}