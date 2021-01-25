using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomIDGenerator : MonoBehaviour{

	public static RandomIDGenerator Instance;
	const string glyphs= "abcdefghijklmnopqrstuvwxyz0123456789";

	void Awake(){
		Instance = this;
	}

	public string GetRandomID(){
		string myString = "";
		int charAmount = UnityEngine.Random.Range(7, 11); //set those to the minimum and maximum length of your string
		for(int i=0; i<charAmount; i++)
			myString += glyphs[UnityEngine.Random.Range(0, glyphs.Length)];
		return myString;
	}
}