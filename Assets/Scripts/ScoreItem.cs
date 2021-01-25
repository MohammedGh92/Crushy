using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArabicSupport;

public class ScoreItem : ImportedItem
{
	public RawImage ScoreIMG;
	public GameObject FBIMG,PlayerProgress;
	public Text RankTxt,PlayerNameTxt,ScoreTxt,PlayerProgressTxt;
	public Image BackIMG;

	public void SetScoreItemDetails(ScoreO ThisScoreItem){
		if (PlayerManager.Instance.MyPlayer.PlayerID == ThisScoreItem.PlayerID)
			BackIMG.color = new Color (0.86f, 0.86f, 0.86f, 1);
		else
			BackIMG.color = Color.white;
		Activated = this;
		RankTxt.text = (transform.GetSiblingIndex () + 1)+"";
		PlayerNameTxt.text = ArabicFixer.Fix(ThisScoreItem.Name);
		ScoreTxt.text = ThisScoreItem.Score.ToString("00");
		FBIMG.SetActive(ThisScoreItem.PlayerID.Length > 13);
		PlayerProgress.SetActive (ThisScoreItem.PlayerProgress>0);
		if (ThisScoreItem.PlayerProgress > 0)
			PlayerProgressTxt.text = "L"+ThisScoreItem.PlayerProgress;
		FBManager.Instance.LoadThisPhoto (ThisScoreItem.PlayerID,ScoreIMG);
	}
}