using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Custom class to accomodate useful stuff for our shapes array
/// </summary>
public class ShapesArray
{

	private GameObject[,] shapes = new GameObject[Constants.Rows, Constants.Columns];

	/// <summary>
	/// Indexer
	/// </summary>
	/// <param name="row"></param>
	/// <param name="column"></param>
	/// <returns></returns>
	public GameObject this [int row, int column] {
		get {
			try {
				return shapes [row, column];
			} catch (Exception ex) {
                
				throw ex;
			}
		}
		set {
			shapes [row, column] = value;
		}
	}

	/// <summary>
	/// Swaps the position of two items, also keeping a backup
	/// </summary>
	/// <param name="g1"></param>
	/// <param name="g2"></param>
	public void Swap (GameObject g1, GameObject g2)
	{
		//hold a backup in case no match is produced
		backupG1 = g1;
		backupG2 = g2;

		var g1Shape = g1.GetComponent<Shape> ();
		var g2Shape = g2.GetComponent<Shape> ();

		//get array indexes
		int g1Row = g1Shape.Row;
		int g1Column = g1Shape.Column;
		int g2Row = g2Shape.Row;
		int g2Column = g2Shape.Column;

		//swap them in the array
		var temp = shapes [g1Row, g1Column];
		shapes [g1Row, g1Column] = shapes [g2Row, g2Column];
		shapes [g2Row, g2Column] = temp;

		//swap their respective properties
		Shape.SwapColumnRow (g1Shape, g2Shape);

	}

	/// <summary>
	/// Undoes the swap
	/// </summary>
	public void UndoSwap ()
	{
		if (backupG1 == null || backupG2 == null)
			throw new Exception ("Backup is null");

		Swap (backupG1, backupG2);
	}

	private GameObject backupG1;
	private GameObject backupG2;


   

	/// <summary>
	/// Returns the matches found for a list of GameObjects
	/// MatchesInfo class is not used as this method is called on subsequent collapses/checks, 
	/// not the one inflicted by user's drag
	/// </summary>
	/// <param name="gos"></param>
	/// <returns></returns>
	public IEnumerable<GameObject> GetMatches (IEnumerable<GameObject> gos)
	{
		List<GameObject> matches = new List<GameObject> ();
		foreach (var go in gos) {
			matches.AddRange (GetMatches (go).MatchedCandy);
		}
		return matches.Distinct ();
	}

	private List<GameObject> MatchesGO;

	/// <summary>
	/// Returns the matches found for a single GameObject
	/// </summary>
	/// <param name="go"></param>
	/// <returns></returns>
	public MatchesInfo GetMatches (GameObject go)
	{
		MatchesGO = new List<GameObject> ();
		MatchesInfo matchesInfo = new MatchesInfo ();
		if (go.name.Contains ("bomb")) {
			var ColorMatches = GetColorMatches (go);
			matchesInfo.AddObjectRange (ColorMatches);
		} else {
			var horizontalMatches = GetMatchesHorizontally (go);
			matchesInfo.AddObjectRange (horizontalMatches);
			var verticalMatches = GetMatchesVertically (go);
			matchesInfo.AddObjectRange (verticalMatches);
			matchesInfo.MatchedCandy.Distinct ();
			for (int i = 0; i < MatchesGO.Count; i++) {
				var horizontalMatches1 = GetMatchesHorizontally (MatchesGO [i]);
				matchesInfo.AddObjectRange (horizontalMatches1);
				var verticalMatches1 = GetMatchesVertically (MatchesGO [i]);
				matchesInfo.AddObjectRange (verticalMatches1);
			}
		}
		return matchesInfo;
	}

	public MatchesInfo DestroyFirst7RowsForRevive()
	{
		MatchesGO = new List<GameObject> ();
		MatchesInfo matchesInfo = new MatchesInfo ();
		var ColorMatches = GetFirst7RowsForDestroy ();
		matchesInfo.AddObjectRange (ColorMatches);
		return matchesInfo;
	}

	private bool ContainsDestroyRowColumnBonus (IEnumerable<GameObject> matches)
	{
		if (matches.Count () >= Constants.MinimumMatches) {
			foreach (var go in matches) {
				if (BonusTypeUtilities.ContainsDestroyWholeRowColumn
                    (go.GetComponent<Shape> ().Bonus))
					return true;
			}
		}

		return false;
	}

	private IEnumerable<GameObject> GetEntireRow (GameObject go)
	{
		List<GameObject> matches = new List<GameObject> ();
		int row = go.GetComponent<Shape> ().Row;
		for (int column = 0; column < Constants.Columns; column++) {
			matches.Add (shapes [row, column]);
		}
		return matches;
	}

	private IEnumerable<GameObject> GetEntireColumn (GameObject go)
	{
		List<GameObject> matches = new List<GameObject> ();
		int column = go.GetComponent<Shape> ().Column;
		for (int row = 0; row < Constants.Rows; row++) {
			matches.Add (shapes [row, column]);
		}
		return matches;
	}

	private IEnumerable<GameObject> GetMatchesHorizontally (GameObject go)
	{
		List<GameObject> matches = new List<GameObject> ();
		matches.Add (go);
		var shape = go.GetComponent<Shape> ();
		string ShapeColor = GetColorOfThisItem (go);
		//check left
		if (shape.Column != 0)
			for (int column = shape.Column - 1; column >= 0; column--) {
				if (shapes [shape.Row, column] == null)
					break;
				if (shapes [shape.Row, column].name.Contains("Colored")||shapes [shape.Row, column].name.Contains (ShapeColor)||shapes [shape.Row, column].name.Contains ("black")) {
					{
							matches.Add (shapes [shape.Row, column]);
					}
				} else
					break;
			}

		//check right
		if (shape.Column != Constants.Columns - 1)
			for (int column = shape.Column + 1; column < Constants.Columns; column++) {
				if (shapes [shape.Row, column] == null)
					break;
				if (shapes [shape.Row, column].name.Contains("Colored")||shapes [shape.Row, column].name.Contains (ShapeColor)||shapes [shape.Row, column].name.Contains ("black")) {
					{
						matches.Add (shapes [shape.Row, column]);}
				} else
					break;
			}

		//we want more than three matches

		if (matches.Count < Constants.MinimumMatches)
			matches.Clear ();
		matches.Distinct ();
		bool Exsists = false;
		for (int i = 0; i < matches.Count; i++) {
			Exsists = false;
			for (int y = 0; y < MatchesGO.Count; y++)
				if (matches [i] == MatchesGO [y]) {
					Exsists = true;
					break;
				}
			if (Exsists)
				continue;
			MatchesGO.Add (matches [i]);
		}
		return matches;
	}

	/// <summary>
	/// Searches horizontally for matches
	/// </summary>
	/// <param name="go"></param>
	/// <returns></returns>
	private IEnumerable<GameObject> GetColorMatches (GameObject go)
	{
		List<GameObject> matches = new List<GameObject> ();
		int CurrentRowNO = ShapesManager.Instance.CurrentRowNo;
		var shape = go.GetComponent<Shape> ();
		string shapeType = GetColorOfThisItem (go);
		if (!go.name.Contains ("bombSelection"))
			matches.Add (go);
		//check ThisColor
		if (shapeType.Contains ("black")) {
			for (int column = shape.Column - 1; column <= shape.Column + 1; column++) {
				if (column < 0 || column > 7)
					continue;
				if (column > Constants.Columns)
					continue;
				for (int row = shape.Row - 1; row <= shape.Row + 1; row++) {
					try{if (row >= Constants.Rows)
						continue;
					if (row < 0 || (row == shape.Row && column == shape.Column))
						continue;
					if (shapes [row, column] == null)
							continue;}catch{
						continue;
					}
					matches.Add (shapes [row, column]);
				}
			}
			try {
				if (CanAddThisOne (shapes [shape.Row, shape.Column + 2]))
					matches.Add (shapes [shape.Row, shape.Column + 2]);
			} catch {
			}

			if (shape.Column == 0 || shape.Column == 7) {//Black Bomb On Edge
				if (shape.Column == 0) {//Black Bomb in the first column
					try {
						if (CanAddThisOne (shapes [shape.Row + 1, shape.Column + 2]))
							matches.Add (shapes [shape.Row + 1, shape.Column + 2]);
					} catch {
					}
					try {
						if (CanAddThisOne (shapes [shape.Row - 1, shape.Column + 2]))
							matches.Add (shapes [shape.Row - 1, shape.Column + 2]);
					} catch {
					}
					try {
						if (CanAddThisOne (shapes [shape.Row, shape.Column + 3]))
							matches.Add (shapes [shape.Row, shape.Column + 3]);
					} catch {
					}
				} else {//Black Bomb column in the last column
					try {
						if (CanAddThisOne (shapes [shape.Row + 1, shape.Column - 2]))
							matches.Add (shapes [shape.Row + 1, shape.Column - 2]);
					} catch {
					}
					try {
						if (CanAddThisOne (shapes [shape.Row - 1, shape.Column + 2]))
							matches.Add (shapes [shape.Row - 1, shape.Column + 2]);
					} catch {
					}
					try {
						if (CanAddThisOne (shapes [shape.Row, shape.Column - 3]))
							matches.Add (shapes [shape.Row, shape.Column - 3]);
					} catch {
					}

				}
			} 
			//Add Row +2,-2 && comlumn +2,-2
			try {
				if (CanAddThisOne (shapes [shape.Row, shape.Column + 2]))
					matches.Add (shapes [shape.Row, shape.Column + 2]);
			} catch {
			}
			try {
				if (CanAddThisOne (shapes [shape.Row, shape.Column - 2]))
					matches.Add (shapes [shape.Row, shape.Column - 2]);
			} catch {
			}
			try {
				if (CanAddThisOne (shapes [shape.Row + 2, shape.Column]))
					matches.Add (shapes [shape.Row + 2, shape.Column]);
			} catch {
			}
			try {
				if (CanAddThisOne (shapes [shape.Row - 2, shape.Column]))
					matches.Add (shapes [shape.Row - 2, shape.Column]);
			} catch {
			}
		} else {
			for (int column = 0; column < Constants.Columns; column++) {
				for (int row = Constants.Rows - CurrentRowNO; row < Constants.Rows; row++) {
					if (shapes [row, column] == null)
						continue;
					if (shapes [row, column].name.Contains (shapeType))
						matches.Add (shapes [row, column]);
				}
			}
		}
		matches.Distinct ();
		return matches;
	}

	private IEnumerable<GameObject> GetFirst7RowsForDestroy ()
	{
		List<GameObject> matches = new List<GameObject> ();
		int CurrentRowNO = ShapesManager.Instance.CurrentRowNo;
		//check ThisColor
			for (int column = 0; column < Constants.Columns; column++) {
				for (int row = Constants.Rows - CurrentRowNO+7; row < Constants.Rows; row++) {
					if (shapes [row, column] == null)
						continue;
					if (CanAddThisOne (shapes [row, column]))
						matches.Add (shapes [row, column]);
				}
			}
		matches.Distinct ();
		return matches;
	}

	bool CanAddThisOne (GameObject gameObject)
	{
		try {
			Shape SquareShape = gameObject.GetComponent<Shape> ();
			if (SquareShape.Column < 0 || SquareShape.Column > 7)
				return false;
			if (SquareShape.Column > Constants.Columns)
				return false;
			if (SquareShape.Row >= Constants.Rows)
				return false;
			if (SquareShape.Row < 0)
				return false;
			if (shapes [SquareShape.Row, SquareShape.Column] == null)
				return false;
		} catch {
			return false;
		}
		return true;
	}


	string GetColorOfThisItem (GameObject go)
	{
		if (go.name.Contains ("blue"))
			return  "blue";
		else if (go.name.Contains ("green"))
			return  "green";
		else if (go.name.Contains ("red"))
			return  "red";
		else if (go.name.Contains ("pink"))
			return  "pink";
		else if (go.name.Contains ("gold"))
			return  "gold";
		else if (go.name.Contains ("teal"))
			return  "teal";
		else if (go.name.Contains ("white"))
			return  "white";
		else if (go.name.Contains ("black"))
			return  "black";
		return  "red";
	}

	/// <summary>
	/// Searches vertically for matches
	/// </summary>
	/// <param name="go"></param>
	/// <returns></returns>
	private IEnumerable<GameObject> GetMatchesVertically (GameObject go)
	{
		List<GameObject> matches = new List<GameObject> ();
		matches.Add (go);
		var shape = go.GetComponent<Shape> ();
		string ShapeColor = GetColorOfThisItem (go);
		//check bottom
		if (shape.Row != 0)
			for (int row = shape.Row - 1; row >= 0; row--) {
				if (shapes [row, shape.Column] != null &&(
					shapes [row, shape.Column].name.Contains("Colored")||shapes [row, shape.Column].name.Contains (ShapeColor)||shapes [row, shape.Column].name.Contains ("black"))) {
					matches.Add (shapes [row, shape.Column]);
				} else
					break;
			}

		//check top
		if (shape.Row != Constants.Rows - 1)
			for (int row = shape.Row + 1; row < Constants.Rows; row++) {
				if (shapes [row, shape.Column] != null &&(
					shapes [row, shape.Column].name.Contains("Colored")||shapes [row, shape.Column].name.Contains (ShapeColor)||shapes [row, shape.Column].name.Contains ("black"))) {
					matches.Add (shapes [row, shape.Column]);
				} else
					break;
			}


		if (matches.Count < Constants.MinimumMatches)
			matches.Clear ();
		matches.Distinct ();
		bool Exsists = false;
		for (int i = 0; i < matches.Count; i++) {
			Exsists = false;
			for (int y = 0; y < MatchesGO.Count; y++)
				if (matches [i] == MatchesGO [y]) {
					Exsists = true;
					break;
				}
			if (Exsists)
				continue;
			MatchesGO.Add (matches [i]);
		}
		return matches;
	}

	/// <summary>
	/// Removes (sets as null) an item from the array
	/// </summary>
	/// <param name="item"></param>
	public void Remove (GameObject item)
	{
		item.GetComponent<Shape> ().DestroyNow ();
		shapes [item.GetComponent<Shape> ().Row, item.GetComponent<Shape> ().Column] = null;

	}

	/// <summary>
	/// Collapses the array on the specific columns, after checking for empty items on them
	/// </summary>
	/// <param name="columns"></param>
	/// <returns>Info about the GameObjects that were moved</returns>
	public AlteredCandyInfo Collapse (IEnumerable<int> columns)
	{
		AlteredCandyInfo collapseInfo = new AlteredCandyInfo ();
		int CurrentRowNO = ShapesManager.Instance.CurrentRowNo;

		///search in every column
		foreach (var column in columns) {
			//begin from bottom row
			for (int row = Constants.Rows - CurrentRowNO; row < Constants.Rows - 1; row++) {
				//if you find a null item
				if (shapes [row, column] == null) {
					//start searching for the first non-null
					for (int row2 = row + 1; row2 < Constants.Rows; row2++) {
						//if you find one, bring it down (i.e. replace it with the null you found)
						if (shapes [row2, column] != null) {
							shapes [row, column] = shapes [row2, column];
							shapes [row2, column] = null;

							//calculate the biggest distance
							if (row2 - row > collapseInfo.MaxDistance)
								collapseInfo.MaxDistance = row2 - row;

							//assign new row and column (name does not change)
							shapes [row, column].GetComponent<Shape> ().Row = row;
							shapes [row, column].GetComponent<Shape> ().Column = column;

							collapseInfo.AddCandy (shapes [row, column]);
							break;
						}
					}
				}
			}
		}
		int row1 = Constants.Rows - CurrentRowNO;
		//Loop For All Columns
		for (int i = 0; i < Constants.Columns; i++) {
			//Check for null column
			if (shapes [row1, i] == null) {
				for (int column2 = i + 1; column2 < Constants.Columns; column2++) {
					if (shapes [row1, column2] != null) {
						for (int row2 = row1; row2 < Constants.Rows; row2++) {
							if (shapes [row2, column2] == null)
								continue;
							shapes [row2, i] = shapes [row2, column2];
							shapes [row2, column2] = null;
							if (column2 - i > collapseInfo.MaxDistance)
								collapseInfo.MaxDistance = column2 - i;
							shapes [row2, i].GetComponent<Shape> ().Row = row2;
							shapes [row2, i].GetComponent<Shape> ().Column = i;
							collapseInfo.AddCandy (shapes [row2, i]);
						}
						break;
					}
				}
			}
		}
		return collapseInfo;
	}

	/// <summary>
	/// Searches the specific column and returns info about null items
	/// </summary>
	/// <param name="column"></param>
	/// <returns></returns>
	public IEnumerable<ShapeInfo> GetEmptyItemsOnColumn (int column)
	{
		List<ShapeInfo> emptyItems = new List<ShapeInfo> ();
		for (int row = 0; row < Constants.Rows; row++) {
			if (shapes [row, column] == null)
				emptyItems.Add (new ShapeInfo () { Row = row, Column = column });
		}
		return emptyItems;
	}

	public int CurrentEmptyRows(){
		int ReturnValue=0;
		for(int row=0;row<13;row++)
		{
			for (int column = 0; column < Constants.Columns; column++) {
				try{
					if (CanAddThisOne (shapes [row, column]))
					break;
				}catch{
				}
				if (column == 6)
					ReturnValue += 1;
			}
		}
		return ReturnValue;
	}

	public bool ThereIsMatches ()
	{
		for (int row = 0; row < 15; row++) {
			for (int column = 0; column < Constants.Columns; column++) {
				try{if (CanAddThisOne (shapes [row, column])) {
					var SquareInfo = GetMatches(shapes [row, column]);
					var TotalMatches = SquareInfo.MatchedCandy.Distinct();
					if (TotalMatches.Count () >= Constants.MinimumMatches && TotalMatches.Count () > 2 ||
					    shapes [row, column].name.Contains ("bomb"))
						return true;
					}}catch{
				}
				}
			}
		return false;
	}
}