using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LevelScores
{
	public float[] LevelDailyScore, LevelWeeklyScore, LevelHighScore;
	public int[] LevelsStarsNO;

	public LevelScores(){
		LevelDailyScore = new float[Constants.NoOfLevels];
		LevelWeeklyScore = new float[Constants.NoOfLevels];
		LevelHighScore = new float[Constants.NoOfLevels];
		LevelsStarsNO = new int[Constants.NoOfLevels];
	}
}
