using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnlineUIHolder : MonoBehaviour
{
	public Text[] PlayersTxts;
	public RawImage[] PlayersIMG;
	public GameObject ContainerGO,WaitingOtherPlayerToFinishGO;
	public Animator WaitingOtherPlayerToFinishGOAnim;
}