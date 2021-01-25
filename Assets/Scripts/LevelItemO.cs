using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LevelItemO
{
	public int LevelID,StarsNO,IsUnLocked;
	public LevelItemO(){
		LevelID = 0;
		StarsNO = 0;
		IsUnLocked = 0;
	}
}