using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Unity.Editor;
using System;

[Serializable]
public class ScoreO
{
	public string PlayerID,Name;
	public float Score;
	public int LevelNO,PlayerProgress;
	public ScoreO(){
		Name = "";
		Score = 0;
		LevelNO = 0;
		PlayerProgress = 0;
	}
}