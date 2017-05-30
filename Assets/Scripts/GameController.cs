using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Player{
	public Image panel;
	public Text text;
	public Button button;
}

[System.Serializable]
public struct PlayerColor{
	public Color panelColor;
	public Color textColor;
}

public class GameController : MonoBehaviour {
	public Text[] buttonList;
	public Text gameOverText;
	public GameObject gameOverPanel;
	public GameObject restartButton;
	public GameObject startInfo;
	public Player playerX;
	public Player playerO;
	public PlayerColor activePlayerColor;
	public PlayerColor inactivePlayerColor;

	private string playerSide;
	private int moveCount;

	void SetGameControllerReferenceOnButtons(){
		for (int i = 0; i < buttonList.Length; i++){
			buttonList[i].GetComponentInParent<GridSpace>().SetGameControllerReference(this);
		}
	}

	void Awake(){
		SetGameControllerReferenceOnButtons ();
		gameOverPanel.SetActive (false);
		moveCount = 0;
		restartButton.SetActive (false);
	}

	public string GetPlayerSide(){
		return playerSide;
	}

	public void EndTurn(){
		moveCount++;

		if (buttonList [0].text == playerSide && buttonList [1].text == playerSide && buttonList [2].text == playerSide) {
			GameOver (playerSide);
//			Debug.Log ("1");
		} else if (buttonList [3].text == playerSide && buttonList [4].text == playerSide && buttonList [5].text == playerSide) {
			GameOver (playerSide);
//			Debug.Log ("2");
		} else if (buttonList [6].text == playerSide && buttonList [7].text == playerSide && buttonList [8].text == playerSide) {
			GameOver (playerSide);
//			Debug.Log ("3");
		} else if (buttonList [0].text == playerSide && buttonList [3].text == playerSide && buttonList [6].text == playerSide) {
			GameOver (playerSide);
//			Debug.Log ("4");
		} else if (buttonList [1].text == playerSide && buttonList [4].text == playerSide && buttonList [7].text == playerSide) {
			GameOver (playerSide);
//			Debug.Log ("5");
		} else if (buttonList [2].text == playerSide && buttonList [5].text == playerSide && buttonList [8].text == playerSide) {
			GameOver (playerSide);
//			Debug.Log ("6");
		} else if (buttonList [0].text == playerSide && buttonList [4].text == playerSide && buttonList [8].text == playerSide) {
			GameOver (playerSide);
//			Debug.Log ("7");
		} else if (buttonList [2].text == playerSide && buttonList [4].text == playerSide && buttonList [6].text == playerSide) {
			GameOver (playerSide);
//			Debug.Log ("8");
		} else if (moveCount >= 9) {
			GameOver ("draw");
		} else {
			ChangeSides ();
		}

	}

	void GameOver(string winningPlayer){
		SetBoardInteractable (false);
		if (winningPlayer == "draw") {
			SetGameOverText ("It's a Draw");
			SetPlayerColorsInactive ();
		} else {
			SetGameOverText (winningPlayer + " Wins!");
		}
		restartButton.SetActive (true);
	}

	void ChangeSides(){
		playerSide = (playerSide == "X") ? "O" : "X";
		if (playerSide == "X") {
			SetPlayerColors (playerX, playerO);
		} 
		else {
			SetPlayerColors (playerO, playerX);
		}
	}

	void SetGameOverText(string value){
		gameOverPanel.SetActive (true);
		gameOverText.text = value;
	}

	void StartGmae(){
		SetBoardInteractable (true);
		SetPlayerButtons (false);
		startInfo.SetActive (false);
	}

	public void RestartGame(){
		moveCount = 0;
		gameOverPanel.SetActive (false);
		restartButton.SetActive (false);
		SetPlayerButtons (true);
		SetPlayerColorsInactive ();
		startInfo.SetActive (true);

		for (int i = 0; i < buttonList.Length; i++) {
			buttonList [i].text = "";
		}
	}

	void SetBoardInteractable(bool toggle){
		for (int i = 0; i < buttonList.Length; i++) {
			buttonList [i].GetComponentInParent<Button> ().interactable = toggle;
		}
	}

	void SetPlayerColors(Player newPlayer, Player oldPlayer){
		newPlayer.panel.color = activePlayerColor.panelColor;
		newPlayer.text.color = activePlayerColor.textColor;
		oldPlayer.panel.color = inactivePlayerColor.panelColor;
		oldPlayer.text.color = inactivePlayerColor.textColor;
	}

	void SetPlayerColorsInactive(){
		playerX.panel.color = inactivePlayerColor.panelColor;
		playerX.text.color = inactivePlayerColor.textColor;
		playerO.panel.color = inactivePlayerColor.panelColor;
		playerO.text.color = inactivePlayerColor.textColor;
	}

	public void SetStartingSide(string startingSide){
		playerSide = startingSide;
		if (playerSide == "X") {
			SetPlayerColors (playerX, playerO);
		} 
		else {
			SetPlayerColors (playerO, playerX);
		}

		StartGmae ();
	}

	void SetPlayerButtons(bool toggle){
		playerX.button.interactable = toggle;
		playerO.button.interactable = toggle;
	}
}
