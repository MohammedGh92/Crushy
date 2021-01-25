using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColoredShape : MonoBehaviour {

	private SpriteRenderer MySprite;
	private int CurrentSelectedShapeNo;

	IEnumerator Start(){
		GetComponent<Shape> ().Type = name;
		MySprite = GetComponent<SpriteRenderer> ();
		CurrentSelectedShapeNo = 0;
		yield return new WaitForSeconds (0.15f);
		name += "Colored";
		InvokeRepeating ("ChangeMyShape",0,1);
	}

	private void ChangeMyShape(){
		if (ShapesManager.Instance.GameIsOver) {
			CancelInvoke ();
			return;
		}
		CurrentSelectedShapeNo++;
		if (CurrentSelectedShapeNo >= 3)
			CurrentSelectedShapeNo = 0;
		MySprite.sprite = ShapesManager.Instance.SquaresSprites [CurrentSelectedShapeNo];
		name = ShapesManager.Instance.SquaresSprites [CurrentSelectedShapeNo].name+"(Clone)Colored";
	}
}