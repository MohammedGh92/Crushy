using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundsManager : MonoBehaviour {

	public static SoundsManager Instance;
	public AudioClip[] BackGroundMusic,SFX,Combos;
	public AudioSource MusicAudioSource,SFXAudioSource;
	[HideInInspector]
	public float BackGroundMusicVolume,SFXVolume;

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

	void Start(){
		SceneManager.sceneLoaded += OnSceneLoaded;
		BackGroundMusicVolume = PlayerManager.Instance.MyPlayer.MusicOn;
		SFXVolume = PlayerManager.Instance.MyPlayer.SoundsOn;
		PlayBackGroundMusicSound (0);
	}

	void OnSceneLoaded (Scene arg0, LoadSceneMode arg1)
	{
		PlayBackGroundMusic (arg0.name);
	}

	void PlayBackGroundMusic (string SceneName)
	{
		switch(SceneName){
		case "MainScene":
			PlayBackGroundMusicSound (0);
			break;
		case "GamePlay":
			if(LevelsManager.Instance.IsBonusLevel)
				PlayBackGroundMusicSound (1);
			else
				PlayBackGroundMusicSound (Random.Range(2,5));
			break;
		}
	}

	public void PlayBackGroundMusicSound(int ClipID){
		MusicAudioSource.volume = BackGroundMusicVolume;
		MusicAudioSource.loop = ClipID <= 4;
		MusicAudioSource.Stop ();
		MusicAudioSource.clip = BackGroundMusic [ClipID];
		MusicAudioSource.Play ();
	}

	public void PlaySFX(int SFXID){
		/*
		 * 0=>	Click
		 * 1-3=>Match
		 * 4=>	Bomb
		 * 5-7=>Stars
		 * */
		SFXAudioSource.PlayOneShot (SFX[SFXID],SFXVolume);
	}

	public void PlayCombo(int SFXID){
		/*
		 * 0=>	Click
		 * 1-3=>Match
		 * 4=>	Bomb
		 * 5-7=>Stars
		 * */
		if(SFXVolume>0)
			SFXAudioSource.PlayOneShot (Combos[SFXID],0.65f);
	}

	public void ToggleSoundsOnOrOff(){
		if (SFXVolume==0)
			SFXVolume = 1;
		else
			SFXVolume = 0;
		SFXAudioSource.volume = SFXVolume;
		PlayerManager.Instance.MyPlayer.SoundsOn = SFXVolume;
		PlayerManager.Instance.SavePlayerData ();
	}

	public void ToggleMusicOnOrOff(){
		if (BackGroundMusicVolume == 0)
			BackGroundMusicVolume = Constants.BackGroundMusicVolume;
		else
			BackGroundMusicVolume = 0;
		MusicAudioSource.volume = BackGroundMusicVolume;
		PlayerManager.Instance.MyPlayer.MusicOn = BackGroundMusicVolume;
		PlayerManager.Instance.SavePlayerData ();
	}
}