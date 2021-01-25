using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

public class ShapesManager : MonoBehaviour
{
	public Text ScoreTxt,CoinsTxt;

	public static ShapesManager Instance;
    public ShapesArray shapes;
	[HideInInspector]
	public int score,Coins;

	public Vector2 BottomRight = new Vector2(-2.4f, 0f);
    public readonly Vector2 CandySize = new Vector2(0.66f, 0.66f);

	private Vector2[] SpawnPositions;
	public GameObject SquareFatherGO,ColoredSquare,SqaureExplo,BombExplo;
	public GameObject[] CandyPrefabs,BombsPrefabs,BonusCandyPrefabs;
	public GameObject[] ExplosionPrefabs,SelectionBombs;

    private IEnumerator CheckPotentialMatchesCoroutine;
    private IEnumerator AnimatePotentialMatchesCoroutine;

    IEnumerable<GameObject> potentialMatches;
	[HideInInspector]
	public int CurrentRowNo;
	public SoundsManager soundManager;
	[HideInInspector]
	public bool GameIsOver;
	private int ColorsNumber,BombRandomRange,BlackBombRandomRange,RowsGroupNo,CurrentSelectedGroupNo=-1,
	ColoredSquareRandomRange;
	private List<int> BonusSquaresGroupNo;
	public Sprite[] SquaresSprites;
	public ScoreShower ScoreShowerScript;

    void Awake()
    {
		Instance = this;
    }

	private int RandomBombCreateNO;
    // Use this for initialization
	IEnumerator Start()
    {
		RandomBombCreateNO = Random.Range (7,91);
		SetBonusSquaresGroupNo ();
		if (LevelsManager.Instance.IsBonusLevel)
			BottomRight = new Vector2 (BottomRight.x,0.2f);
		yield return new WaitForSeconds (0.1f);
		ColorsNumber = GameManager.Instance.ColorsNumber;
		BombRandomRange = GameManager.Instance.BombRandomRange;
		BlackBombRandomRange = GameManager.Instance.BlackBombRandomRange;
		ColoredSquareRandomRange = GameManager.Instance.ColoredSquareRandomRange;
		RowsGroupNo = GameManager.Instance.RowsGroupNo;
		yield return new WaitForSeconds (0.1f);
		InitializeVariables();
		InitializeTypesOnPrefabShapesAndBonuses();
		InitializeCandyAndSpawnPositions();
    }

	private void SetBonusSquaresGroupNo ()
	{
		if (GameManager.Instance.IsTotorialLevel) {
			BonusSquaresGroupNo = new List<int> (31);
			for (int i = 0; i < 45; i++)
				BonusSquaresGroupNo.Add (2);
			BonusSquaresGroupNo.Add (1);
		} else {
			BonusSquaresGroupNo = new List<int> (10);
			for (int i = 5; i < 16; i++)
				BonusSquaresGroupNo.Add (i);
			BonusSquaresGroupNo.Add (10);
		}
	}

    /// <summary>
    /// Initialize shapes
    /// </summary>
    private void InitializeTypesOnPrefabShapesAndBonuses()
    {
        //just assign the name of the prefab
        foreach (var item in CandyPrefabs)
        {
            item.GetComponent<Shape>().Type = item.name;
        }
		//just assign the name of the Bomb prefab
		foreach (var item in BombsPrefabs)
		{
			item.GetComponent<Shape>().Type = item.name;
		}
		ColoredSquare.GetComponent<Shape>().Type = ColoredSquare.name;
    }
		

	public void InitializeCandyAndSpawnPositions()
    {
        if (shapes != null)
            DestroyAllCandy();
        shapes = new ShapesArray();
        SpawnPositions = new Vector2[Constants.Columns];
		GameManager.Instance.CreateNewRowsGroup ();
    }

	public void CreateRowsGroup ()
	{
		int CreatedRowNo = 0;
		SoundsManager.Instance.PlaySFX (12);
		if (GameManager.Instance.IsTotorialLevel) {//Is Tutorial
			for (int row = Constants.Rows - CurrentRowNo - 1; CreatedRowNo < RowsGroupNo; row--) {
				for (int column = 0; column < Constants.Columns; column++) {
					GameObject newCandy = GetThisCandy (GetThisCandyID());
					InstantiateAndPlaceNewCandy (row, column, newCandy);
				}
				CreatedRowNo++;
				CurrentRowNo++;
			}
		}
		else if (GameManager.Instance.IsBonusLevel) {//Is Bonus
			for (int row = Constants.Rows - CurrentRowNo - 1; CreatedRowNo < RowsGroupNo; row--) {
				for (int column = 0; column < Constants.Columns; column++) {
					GameObject newCandy = GetThisCandy (GetThisCandyID());
					InstantiateAndPlaceNewCandy (row, column, newCandy);
				}
				CreatedRowNo++;
				CurrentRowNo++;
			}
		} else {//Not Bonus
			for (int row = Constants.Rows - CurrentRowNo - 1; CreatedRowNo < RowsGroupNo; row--) {
				for (int column = 0; column < Constants.Columns; column++) {
					GameObject newCandy = GetRandomCandy ();
					InstantiateAndPlaceNewCandy (row, column, newCandy);
				}
				CreatedRowNo++;
				CurrentRowNo++;
			}
		}
		SetupSpawnPositions ();
	}

	private int CurrentSelectedColor;
	int GetThisCandyID ()
	{
		if (CurrentSelectedGroupNo == -1) {
			int RandomItemFromGroupsNo = Random.Range (0, BonusSquaresGroupNo.Count);
			CurrentSelectedGroupNo = BonusSquaresGroupNo [RandomItemFromGroupsNo];
			BonusSquaresGroupNo.RemoveAt (RandomItemFromGroupsNo);
			if (CurrentSelectedColor == 0)
				CurrentSelectedColor = Random.Range (1, 3);
			else if (CurrentSelectedColor == 1) {
				CurrentSelectedColor = Random.Range (1, 3);
				if (CurrentSelectedColor == 1)
					CurrentSelectedColor = 0;
			}
			else
				CurrentSelectedColor = Random.Range (0,2);
		}
		CurrentSelectedGroupNo--;
		return CurrentSelectedColor;
	}

    private void InstantiateAndPlaceNewCandy(int row, int column, GameObject newCandy)
    {
        GameObject go = Instantiate(newCandy,
			BottomRight + new Vector2(column * CandySize.x, row * CandySize.y), Quaternion.identity,SquareFatherGO.transform)
            as GameObject;

        //assign the specific properties
        go.GetComponent<Shape>().Assign(newCandy.GetComponent<Shape>().Type, row, column);
        shapes[row, column] = go;
    }

    private void SetupSpawnPositions()
    {
        //create the spawn positions for the new shapes (will pop from the 'ceiling')
        for (int column = 0; column < Constants.Columns; column++)
        {
            SpawnPositions[column] = BottomRight
                + new Vector2(column * CandySize.x, Constants.Rows * CandySize.y);
        }
    }

    /// <summary>
    /// Destroy all candy gameobjects
    /// </summary>
    private void DestroyAllCandy()
	{
		
        for (int row = 0; row < Constants.Rows; row++)
        {
            for (int column = 0; column < Constants.Columns; column++)
            {
                Destroy(shapes[row, column]);
            }
        }
    }

	public bool CanMatch=true;
    // Update is called once per frame
    void Update()
    {
		if (Input.GetMouseButtonUp(0))
            {
			if (IsTutPhase ()) {
				GameManager.Instance.ShowNextSimpleTut ();
				return;
			}
			if (!CanMatch)
				return;
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            //we have a hit
			if (hit.collider != null&&hit.collider.tag=="Square")
                   FindMatchesAndCollapse(hit);
            }
    }

	private bool IsTutPhase(){
		return GameManager.Instance.tutorialClickPhase <=6&&GameManager.Instance.tutorialClickPhase>=2;
	}

    /// <summary>
    /// Modifies sorting layers for better appearance when dragging/animating
    /// </summary>
    /// <param name="hitGo"></param>
    /// <param name="hitGo2"></param>
    private void FixSortingLayer(GameObject hitGo, GameObject hitGo2)
    {
        SpriteRenderer sp1 = hitGo.GetComponent<SpriteRenderer>();
        SpriteRenderer sp2 = hitGo2.GetComponent<SpriteRenderer>();
        if (sp1.sortingOrder <= sp2.sortingOrder)
        {
            sp1.sortingOrder = 1;
            sp2.sortingOrder = 0;
        }
    }
	GameObject hitGo2;
	public void FindMatchesAndCollapse(RaycastHit2D hit2)
    {
		CanMatch = false;
		if ((!GameManager.Instance.IsTotorialLevel && Time.timeScale == 0) || GameIsOver) {
			CanMatch = true;
			return;
		}
		if (GameManager.Instance.IsTotorialLevel && !CanMatchPhase ()) {
			CanMatch = true;
			return;
		}
		hitGo2 = hit2.collider.gameObject;
        var hitGo2matchesInfo = shapes.GetMatches(hitGo2);
		var totalMatches = hitGo2matchesInfo.MatchedCandy.Distinct();
		if (totalMatches.Count() >= Constants.MinimumMatches&&totalMatches.Count()>2||
			hitGo2.name.Contains("bomb"))
        {
			if (!hitGo2.name.Contains("bomb")&&totalMatches.Count () >= 20)
			{
				if(CanKingCrush())
					GameManager.Instance.KingCrushHere();
			}
			if (GameManager.Instance.IsTotorialLevel && GameManager.Instance.tutorialClickPhase == 0)
				GameManager.Instance.ShowBombTut ();
			if(hitGo2.name.Contains("bomb"))
				SoundsManager.Instance.PlaySFX (4);
			else
				SoundsManager.Instance.PlaySFX ((int)(Random.Range(1,4)));
			if (hitGo2.name.Contains ("bomb"))
				IncreaseScoreWithoutFactorial (totalMatches.Count () * Constants.Match3Score);
			else {
				if(totalMatches.Count()<10)
					IncreaseScore ((int)((totalMatches.Count () * Constants.Match3Score) * ColorsBonus (hitGo2)));
				else
					IncreaseScore (totalMatches.Count () * Constants.Match3Score);
			}
            foreach (var item in totalMatches)
            {
                shapes.Remove(item);
				GameObject ExploGO;
				if(item.name.Contains("bomb"))
					ExploGO = Instantiate (BombExplo,item.transform.position,Quaternion.identity);
				else
					ExploGO = Instantiate (SqaureExplo,item.transform.position,Quaternion.identity);
				Destroy (ExploGO,0.7f);
            }
            //get the columns that we had a collapse
            var columns = totalMatches.Select(go => go.GetComponent<Shape>().Column).Distinct();
            //the order the 2 methods below get called is important!!!
            //collapse the ones gone
            var collapsedCandyInfo = shapes.Collapse(columns);
            var newCandyInfo = CreateNewCandyInSpecificColumns(columns);
            int maxDistance = Mathf.Max(collapsedCandyInfo.MaxDistance, newCandyInfo.MaxDistance);
			AddComboBonus (totalMatches.Count());
            MoveAndAnimate(collapsedCandyInfo.AlteredCandy, maxDistance);
			if (CurrentRowNo == Constants.Rows && !shapes.ThereIsMatches())
				GameManager.Instance.GameIsOver (true);
		}else
			CanMatch = true;
		GameManager.Instance.RefreshYourRank ();
    }

	bool CanKingCrush ()
	{
		for (int i = 0; i < SquareFatherGO.transform.childCount; i++) {
			GameObject ChildGO = SquareFatherGO.transform.GetChild (i).gameObject;
			if (ChildGO.name.Contains ("MagicBle"))
				return false;
		}
		return true;
	}

	bool CanMatchPhase ()
	{
		return (GameManager.Instance.tutorialClickPhase == 0 || GameManager.Instance.tutorialClickPhase > 4);
	}

	bool CanMatchBombPhase ()
	{
		return (GameManager.Instance.tutorialClickPhase == 1 || GameManager.Instance.tutorialClickPhase > 4);
	}
		
	private float ColorsBonus(GameObject HitGO2){
		if (HitGO2.name.Contains ("pink"))
			return 1.2f;
		else if (HitGO2.name.Contains ("gold"))
			return 1.5f;
		else if (HitGO2.name.Contains ("teal"))
			return 1.8f;
		else if (HitGO2.name.Contains ("white"))
			return 2f;
		else
			return 1;
	}

	public void FindMatchesAndCollapse(int BombID)
	{
		if ((!GameManager.Instance.IsTotorialLevel && Time.timeScale==0)||GameIsOver)
			return;
		if (GameManager.Instance.IsTotorialLevel && !CanMatchBombPhase())
			return;
		var hitGo2 = SelectionBombs[BombID];
		var hitGo2matchesInfo = shapes.GetMatches(hitGo2);
		var totalMatches = hitGo2matchesInfo.MatchedCandy.Distinct();	
		if (totalMatches.Count() >= Constants.MinimumMatches&&totalMatches.Count()>2||
			hitGo2.name.Contains("bomb"))
		{
			if(hitGo2.name.Contains("bomb"))
				SoundsManager.Instance.PlaySFX (4);
			else
				SoundsManager.Instance.PlaySFX ((int)(Random.Range(1,4)));
			if (hitGo2.name.Contains ("bomb")) {
				IncreaseScoreWithoutFactorial(totalMatches.Count() * Constants.Match3Score);
				if (GameManager.Instance.tutorialClickPhase == 1)
					StartCoroutine (ShowNextTutCor());
			}
			else
				IncreaseScore(totalMatches.Count() * Constants.Match3Score);
			foreach (var item in totalMatches)
			{
				shapes.Remove(item);
				GameObject SquareExploGO = Instantiate (SqaureExplo,item.transform.position,Quaternion.identity);
				Destroy (SquareExploGO,0.7f);
			}
			//get the columns that we had a collapse
			var columns = totalMatches.Select(go => go.GetComponent<Shape>().Column).Distinct();
			//the order the 2 methods below get called is important!!!
			//collapse the ones gone
			var collapsedCandyInfo = shapes.Collapse(columns);
			var newCandyInfo = CreateNewCandyInSpecificColumns(columns);
			int maxDistance = Mathf.Max(collapsedCandyInfo.MaxDistance, newCandyInfo.MaxDistance);
			AddComboBonus (totalMatches.Count());
			MoveAndAnimate(collapsedCandyInfo.AlteredCandy, maxDistance);
			if (CurrentRowNo == Constants.Rows && !shapes.ThereIsMatches())
				GameManager.Instance.GameIsOver (true);
		}
		GameManager.Instance.RefreshYourRank ();
	}

	private IEnumerator ShowNextTutCor(){
		yield return new WaitForSeconds (1);
		GameManager.Instance.tutorialClickPhase = 2;
		GameManager.Instance.ShowNextSimpleTut ();
	}

	private bool HaveBombs(){
		for (int i = 0; i < GameManager.Instance.BombsForCurrentLevel.Length; i++)
			if (GameManager.Instance.BombsForCurrentLevel [i] >= 1)
				return true;
		return false;
	}

	public void DestroyFirst7RowsForRevive()
	{
		var hitGo2matchesInfo = shapes.DestroyFirst7RowsForRevive();
		var totalMatches = hitGo2matchesInfo.MatchedCandy.Distinct();	
			foreach (var item in totalMatches)
			{
				shapes.Remove(item);
			}
			//get the columns that we had a collapse
			var columns = totalMatches.Select(go => go.GetComponent<Shape>().Column).Distinct();
			//the order the 2 methods below get called is important!!!
			//collapse the ones gone
			var collapsedCandyInfo = shapes.Collapse(columns);
			var newCandyInfo = CreateNewCandyInSpecificColumns(columns);
			int maxDistance = Mathf.Max(collapsedCandyInfo.MaxDistance, newCandyInfo.MaxDistance);
			AddComboBonus (totalMatches.Count());
			MoveAndAnimate(collapsedCandyInfo.AlteredCandy, maxDistance);
			if (CurrentRowNo == Constants.Rows && !shapes.ThereIsMatches())
				GameManager.Instance.GameIsOver (true);
	}

	void AddComboBonus (int MatchesCount)
	{
		if (MatchesCount < 30)
			return;
		int BonusMultiplier = MatchesCount - 20;
		int TensCounter = 0;
		while(BonusMultiplier>=10){
			BonusMultiplier -= 10;
			TensCounter += 1;
		}
		IncreaseScoreWithoutFactorial (1000*TensCounter);
	}

    /// <summary>
    /// Spawns new candy in columns that have missing ones
    /// </summary>
    /// <param name="columnsWithMissingCandy"></param>
    /// <returns>Info about new candies created</returns>
    private AlteredCandyInfo CreateNewCandyInSpecificColumns(IEnumerable<int> columnsWithMissingCandy)
	{
        AlteredCandyInfo newCandyInfo = new AlteredCandyInfo();
		return newCandyInfo;
    }

    /// <summary>
    /// Animates gameobjects to their new position
    /// </summary>
    /// <param name="movedGameObjects"></param>
    private void MoveAndAnimate(IEnumerable<GameObject> movedGameObjects, int distance)
    {
		StartCoroutine (MoveAndAnimateCor(movedGameObjects,distance));
      
    }

	private IEnumerator MoveAndAnimateCor(IEnumerable<GameObject> movedGameObjects, int distance){
		yield return new WaitForSeconds (Constants.AnimationDuration);
		foreach (var item in movedGameObjects)
		{
			item.transform.positionTo(Constants.MoveAnimationMinDuration * distance, BottomRight +
				new Vector2(item.GetComponent<Shape>().Column * CandySize.x, item.GetComponent<Shape>().Row * CandySize.y));
		}
		yield return new WaitForSeconds (0.1f);
		CanMatch = true;
		GameManager.Instance.MyAlarmZone.AlarmOn = 0;
	}

    /// <summary>
    /// Get a random candy
    /// </summary>
    /// <returns></returns>
	private int SquaresCounter;
    private GameObject GetRandomCandy()
    {
		SquaresCounter++;
		int ColoredSquareRandomNo = Random.Range (0,ColoredSquareRandomRange);
		if (ColoredSquareRandomNo == 0&& GameManager.Instance.LevelNO>=5)
			return ColoredSquare;
		int BombRandomNo = Random.Range (0,BombRandomRange);
		if (BombRandomNo == 0||SquaresCounter==RandomBombCreateNO){//Return Bomb
			if (SquaresCounter == RandomBombCreateNO) {
				SquaresCounter = 0;
				RandomBombCreateNO = Random.Range (7, 91);
			}
			int BlackBombRandom = Random.Range (0,BlackBombRandomRange);
			if (BlackBombRandom == 0)
				return BombsPrefabs [0];
			else
				return BombsPrefabs[Random.Range(0, ColorsNumber+1)];
		}
		else
			return CandyPrefabs[Random.Range(0, 6+ColorsNumber)];
    }

	private GameObject GetThisCandy(int CandyID)
	{
		/*
		 * 0	Blue
		 * 1	Green
		 * 2	Red
		 * */
		return BonusCandyPrefabs[CandyID];
	}

    private void InitializeVariables()
    {
        score = 0;
        ShowScore();
    }

	public void IncreaseScore(int amount)
    {
		score += GetLevelBonus(GetRealAcount (amount));
		Coins += (int)((score / 100) * Constants.ScoreToCoins);
		ShowScore(GetLevelBonus(GetRealAcount(amount)),amount/10);
    }

	public void IncreaseScoreWithoutFactorial(int amount)
	{
		score += GetLevelBonus(amount);
		Coins += (int)((score / 100) * Constants.ScoreToCoins);
		ShowScore(GetLevelBonus(amount),-1);
	}

	int GetLevelBonus (int score)
	{
		return score + ((int)(score * 0.1f * GameManager.Instance.LevelNO));
	}

	int GetRealAcount (int amount)
	{
		if (amount > 30)
			return 30 + Factorial (amount/10);
		else
			return 30;
	}

	int Factorial(int i)
	{
		if (i <= 4)
			return i*10;
		return (i*10) + Factorial(i - 1);
	}

	private void ShowScore()
	{
		try{if (LevelsManager.Instance.IsOnlineGame) {
				FireBaseManager.Instance.UpdateMyScore (score);
		}else{
			ScoreTxt.text = "" + score.ToString("00");
			CoinsTxt.text = "" + Coins.ToString("00");
		}
	}catch{
			ScoreTxt.text = "" + score.ToString("00");
			CoinsTxt.text = "" + Coins.ToString ("0");
		}
	}

	private void ShowScore(int PlusAmount,int MatchesNO)
    {
		try{
			ScoreShowerScript.ShowScoreNo (PlusAmount.ToString("00"),hitGo2.transform,MatchesNO);
		}catch{
		}
		try{if (LevelsManager.Instance.IsOnlineGame) {
			FireBaseManager.Instance.UpdateMyScore (score);
		}else{
			ScoreTxt.text = "" + score.ToString("00");
			CoinsTxt.text = "" + (int)((score/100)*Constants.ScoreToCoins);
		}
	}catch{
			ScoreTxt.text = "" + score.ToString("00");
			CoinsTxt.text = "" + (int)((score/100)*Constants.ScoreToCoins);
		}
    }

	public void AddEmptyRowsBonus(){
		GameManager.Instance.NoOfEmptyRows = shapes.CurrentEmptyRows ();

	}

    /// <summary>
    /// Get a random explosion
    /// </summary>
    /// <returns></returns>
    private GameObject GetRandomExplosion()
    {
        return ExplosionPrefabs[Random.Range(0, ExplosionPrefabs.Length)];
    }

	public void ToBlue(){
		for (int i = 0; i < SquareFatherGO.transform.childCount; i++) {
			GameObject ChildGO = SquareFatherGO.transform.GetChild (i).gameObject;
			if (ChildGO.name.Contains ("bomb"))
				continue;
			ChildGO.GetComponent<Shape> ().ToBlue ();
		}
	}
}