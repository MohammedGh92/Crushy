using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[Serializable]
public class RoomO
{
	public string ID;
	public int LevelNO,RoomStatus;
	public int[] PlayersConnectionsStatus;
	public float[] PlayersScores;
	public string[] PlayersNames,PlayerSIDs;
	public RoomO(){
		PlayersNames = new string[2];
		PlayerSIDs = new string[2];
		PlayersScores = new float[2];
		PlayersConnectionsStatus = new int[2];
	}
}