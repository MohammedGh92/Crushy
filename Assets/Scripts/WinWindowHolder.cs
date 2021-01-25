using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WinWindowHolder:MonoBehaviour
{
	public Text ScoreTxt,CoinsTxt;
	public GameObject NextBTNGO,BonusBTNGO,RestartBTNGO,YouWinWindow,FireWorksGO,HighScoreGO;
	public Animator WinSpriteWindowAnim,WinUIWindowAnim;
	public GameObject[] StarsGO;
}
