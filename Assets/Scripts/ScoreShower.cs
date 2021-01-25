using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreShower : MonoBehaviour {

	public RectTransform ScoreRect;
	public Animator ScoresAnims;
	public Image[] ScoreNoIMG;
	public ImagesArray[] RowsScoresNoIMG;
	public Sprite[] NoFrom0To10;
	public RectTransform canvasRectT;
	public Animator[] WordsAnims,EmptyRowsAnims;
	public GameObject ScoreGO, WordsGO;

	public void HideAll(){
		ScoreGO.SetActive (false);
		WordsGO.SetActive (false);
	}

	public void ShowScoreNo(string ScoreToShow,Transform MatchPos,int MatchesNO){
		if (MatchesNO < 1)
			return;
		StopAllCoroutines ();
		StartCoroutine (ShowScoreNoCor(ScoreToShow,MatchPos,MatchesNO));
	}

	private IEnumerator ShowScoreNoCor(string ScoreToShow,Transform MatchPos,int MatchesNO){
		char[] NosArray = ScoreToShow.ToCharArray ();
		for (int i = 0; i < ScoreNoIMG.Length; i++)
			ScoreNoIMG [i].gameObject.SetActive (false);
		for (int i = 0; i < NosArray.Length; i++) {
			ScoreNoIMG [i].gameObject.SetActive (true);
			ScoreNoIMG [i].sprite = NoFrom0To10 [int.Parse (NosArray [i].ToString ())];
		}
		if(MatchesNO>=5)
			WordsAnims [GetWordAnimFromMatchesNO (MatchesNO)].SetBool ("IsShow", true);
		Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, MatchPos.position);
		ScoreRect.anchoredPosition = screenPoint - canvasRectT.sizeDelta / 2f;
		if (ScoreRect.anchoredPosition.x < -300 && ScoreRect.anchoredPosition.x > -400)
			ScoreRect.anchoredPosition = new Vector2 (ScoreRect.anchoredPosition.x+150,ScoreRect.anchoredPosition.y);
		else if (ScoreRect.anchoredPosition.x < -400)
			ScoreRect.anchoredPosition = new Vector2 (ScoreRect.anchoredPosition.x+250,ScoreRect.anchoredPosition.y);
		ScoresAnims.SetBool ("IsShow", true);
		yield return new WaitForSeconds (0.5f);
		if(MatchesNO>=5)
			WordsAnims [GetWordAnimFromMatchesNO (MatchesNO)].SetBool ("IsShow", false);
		ScoresAnims.SetBool ("IsShow", false);
	}

	public void ShowEmptyRowsScores(int NoOFRows,string NoOfScore){
		StartCoroutine(ShowEmptyRowsScoresCor (NoOFRows,NoOfScore));
	}

	private IEnumerator ShowEmptyRowsScoresCor(int NoOFRows,string NoOfScore){
		char[] NosArray = NoOfScore.ToCharArray ();
		for (int i = 0; i < NoOFRows; i++) {
			yield return new WaitForSeconds (0.3f);
			for (int y = 0; y < NosArray.Length; y++) {
				RowsScoresNoIMG[i].RowScoreNoIMGS[y].gameObject.SetActive (true);
				RowsScoresNoIMG[i].RowScoreNoIMGS[y].sprite = NoFrom0To10 [int.Parse (NosArray [y].ToString ())];
			}
			EmptyRowsAnims [i].SetBool ("IsShow",true);
			RowsScoresNoIMG [i].ShowThisNo ();
			yield return new WaitForSeconds (0.15f);
			SoundsManager.Instance.PlayCombo(i);
			ShapesManager.Instance.IncreaseScoreWithoutFactorial (GameManager.Instance.LevelNO*100);
		}
	}

	private int GetWordAnimFromMatchesNO (int MatchesNO)
	{
		if(MatchesNO>10)
			SoundsManager.Instance.PlaySFX (8);
		if (MatchesNO >= 5 && MatchesNO < 7)
			return 0;
		else if (MatchesNO >= 7 && MatchesNO < 10)
			return 1;
		else if (MatchesNO >= 10 && MatchesNO < 14)
			return 2;
		else if (MatchesNO >= 14 && MatchesNO < 17)
			return 3;
		else if (MatchesNO >= 17 && MatchesNO < 21)
			return 4;
		else if (MatchesNO >= 21 && MatchesNO < 24)
			return 5;
		else if (MatchesNO >= 24 && MatchesNO < 28)
			return 6;
		else if (MatchesNO >= 28)
			return 7;
		return 0;
	}
}