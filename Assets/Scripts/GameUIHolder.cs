using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameUIHolder : MonoBehaviour
{
	public Text ScoreTxt,TimerTxt,ReviveTxt,RankNOTxt,LevelNOTxt;
	public GameObject[] Bombs,TutorialGO;
	public Text[] BombsNOTxt;
	public Animator LoseUIWindowAnim,ReviveWindowAnim,PauseMenuAnim;
	public RectTransform ScoreRect,CoinsRect;
	public Button ReviveWatchVideoBTN,UsingCoinsBTN;
}
