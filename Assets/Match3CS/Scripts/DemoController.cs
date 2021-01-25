//
// DemoController.cs
//
// Author:
//       Cosmos and paradox <cosmos.n.paradox@gmail.com>
//
// Copyright (c) 2016 Cosmos and paradox

using UnityEngine;
using System.Collections;
using Match3CS;

public class DemoController : MonoBehaviour
{

	public Candy[] candyHint;
	public Candy[] candyArray;
	public float delayTime;

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Q)) {
			Hint (false);
			StartCoroutine (AnimateLanding (0, delayTime));
		}

		if (Input.GetKeyDown (KeyCode.W)) {
			Hint (false);
			StartCoroutine (AnimateShake (0, delayTime));
		}

		if (Input.GetKeyDown (KeyCode.E)) {
			Hint (false);
			StartCoroutine (AnimateDestroy (0, delayTime));
		}

		if (Input.GetKeyDown (KeyCode.R)) {
			Hint (false);
			StartCoroutine (AnimateGlow (0, delayTime));
		}

		if (Input.GetKeyDown (KeyCode.T)) {
			Hint (true);
		}
	}

	private void Hint (bool action)
	{
		for (int i = 0; candyHint.Length > i; i++) {
			candyHint [i].Hint (action);
		}
	}

	IEnumerator AnimateLanding (int CandyID, float waitTime)
	{
		yield return new WaitForSeconds (waitTime);
		candyArray [CandyID].Landing ();
		CandyID++;
		if (candyArray.Length > CandyID) {
			StartCoroutine (AnimateLanding (CandyID, waitTime));	
		}
	}

	IEnumerator AnimateShake (int CandyID, float waitTime)
	{
		yield return new WaitForSeconds (waitTime);
		candyArray [CandyID].Shake ();
		CandyID++;
		if (candyArray.Length > CandyID) {
			StartCoroutine (AnimateShake (CandyID, waitTime));	
		}
	}

	IEnumerator AnimateDestroy (int CandyID, float waitTime)
	{
		yield return new WaitForSeconds (waitTime);
		candyArray [CandyID].Destroy ();
		CandyID++;
		if (candyArray.Length > CandyID) {
			StartCoroutine (AnimateDestroy (CandyID, waitTime));	
		}
	}

	IEnumerator AnimateGlow (int CandyID, float waitTime)
	{
		yield return new WaitForSeconds (waitTime);
		candyArray [CandyID].Glow ();
		CandyID++;
		if (candyArray.Length > CandyID) {
			StartCoroutine (AnimateGlow (CandyID, waitTime));	
		}
	}
		
}