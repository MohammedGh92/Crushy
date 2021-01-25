using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class FortuneWheelManager : MenuFather
{
	public static FortuneWheelManager Instance;
    private bool _isStarted;
    private float[] _sectorsAngles;
    private float _finalAngle;
    private float _startAngle = 0;
    private float _currentLerpRotationTime;
    public Button TurnButton;
    public GameObject Circle; 			// Rotatable Object with rewards
	public Text NoVideoAvailableTxt;

	void Awake(){
		Instance = this;
	}

	public void ShowSpinner(bool ShowIt){
		IsShowed = ShowIt;
		if (ShowIt)
			InvokeRepeating ("CheckIfHaveVideo", 0, 1);
		else
			CancelInvoke ();
				MyAnim.SetBool ("IsShow",ShowIt);
	}

	private void CheckIfHaveVideo(){
		if (_isStarted) {
			ActivateSpinBTN (false);
			return;
		}
				ActivateSpinBTN (AdsManager.Instance.RewardVideoAdIsLoaded);
	}

	public void ActivateSpinBTN(bool Activate){
		TurnButton.interactable = Activate;
		if (Activate)
			NoVideoAvailableTxt.color = Color.clear;
		else
			NoVideoAvailableTxt.color = Color.white;
	}

	public void ShowVideoThenTurnTheWheel(){
		SoundsManager.Instance.PlaySFX (0);
		AdsManager.Instance.ShowVideo ();
	}

    public void TurnWheel ()
    {
		if (_isStarted)
			return;
    	// Player has enough money to turn the wheel
	    _currentLerpRotationTime = 0f;
	
	    // Fill the necessary angles (for example if you want to have 12 sectors you need to fill the angles with 30 degrees step)
	    _sectorsAngles = new float[] { 30, 60, 90, 120, 150, 180, 210, 240, 270, 300, 330, 360 };
	
	    int fullCircles = 5;
	    float randomFinalAngle = _sectorsAngles [UnityEngine.Random.Range (0, _sectorsAngles.Length)];
	
	    // Here we set up how many circles our wheel should rotate before stop
	    _finalAngle = -(fullCircles * 360 + randomFinalAngle);
	    _isStarted = true;
    }

    private void GiveAwardByAngle ()
    {
    	// Here you can set up rewards for every sector of wheel
    	switch ((int)_startAngle) {
		case 0:
			GiftsShowerManager.Instance.GiveABombGift (0);
    	    break;
    	case -330:
			GiftsShowerManager.Instance.GiveACoinsGift (1);
    	    break;
    	case -300:
			GiftsShowerManager.Instance.GiveABombGift (1);
    	    break;
    	case -270:
			GiftsShowerManager.Instance.GiveACoinsGift (0);
    	    break;
    	case -240:
			GiftsShowerManager.Instance.GiveABombGift (2);
			break;
    	case -210:
			GiftsShowerManager.Instance.GiveACoinsGift (0);
    	    break;
    	case -180:
			GiftsShowerManager.Instance.GiveABombGift (0);
    	    break;
    	case -150:
			GiftsShowerManager.Instance.GiveACoinsGift (2);
    	    break;
    	case -120:
			GiftsShowerManager.Instance.GiveABombGift (2);
    	    break;
    	case -90:
			GiftsShowerManager.Instance.GiveACoinsGift (1);
			break;
    	case -60:
			GiftsShowerManager.Instance.GiveABombGift (1);
			break;
    	case -30:
			GiftsShowerManager.Instance.GiveACoinsGift (1);
    	    break;
    	default:
			GiftsShowerManager.Instance.GiveABombGift (0);
			break;
        }
    }

    void Update ()
    {
        // Make turn button non interactable if user has not enough money for the turn
    	if (!_isStarted)
    	    return;

    	float maxLerpRotationTime = 4f;
    
    	// increment timer once per frame
    	_currentLerpRotationTime += Time.deltaTime;
    	if (_currentLerpRotationTime > maxLerpRotationTime || Circle.transform.eulerAngles.z == _finalAngle) {
    	    _currentLerpRotationTime = maxLerpRotationTime;
    	    _isStarted = false;
    	    _startAngle = _finalAngle % 360;
    	    GiveAwardByAngle ();
    	}
    
    	// Calculate current position using linear interpolation
    	float t = _currentLerpRotationTime / maxLerpRotationTime;
    
    	// This formulae allows to speed up at start and speed down at the end of rotation.
    	// Try to change this values to customize the speed
    	t = t * t * t * (t * (6f * t - 15f) + 10f);
    
    	float angle = Mathf.Lerp (_startAngle, _finalAngle, t);
    	Circle.transform.eulerAngles = new Vector3 (0, 0, angle);
    }
}