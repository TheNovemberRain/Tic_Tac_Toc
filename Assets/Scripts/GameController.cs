using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Text;
using System.Threading;

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

[System.Serializable]
struct ChessBoardPosition
{
	public int line;
	public int list;
}

[System.Serializable]
struct Tree
{
	public int[,] chessBoard;
	public ChessBoardPosition position;
	public int weight;
}

public class GameController : MonoBehaviour {
	public Text[] buttonList;
	public Text gameOverText;
	public GameObject gameOverPanel;
	public GameObject restartButton;
	public GameObject startInfo;
	public Button[] button;
	public Player playerX;
	public Player playerO;
	public PlayerColor activePlayerColor;
	public PlayerColor inactivePlayerColor;

	private string playerSide;
	private int moveCount;
	private int[,] chessBoard = new int[3, 3] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };

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
			moveCount++;
			if (Algrothm ()) {
				GameOver (playerSide);
			} else {
				
				if (moveCount >= 9) {
					GameOver ("draw");
				} else {
					ChangeSides ();
				}

			}
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
		if (playerSide == "O") {
			buttonList [4].text = "X";
			button [4].interactable = false;
			moveCount++;
		}
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

	//下面是AI算法
	public bool Algrothm()
	{
		ChessBoardPosition first = new ChessBoardPosition();    //第一次分析的当前棋位
		ChessBoardPosition second = new ChessBoardPosition();   //第二次分析的当前棋位
		Tree[] tree = new Tree[9];  //分析树
		int node;   //分析树的当前节点下标
		int[,] secondBoard = new int[3,3]; //用于第二次分析的棋盘
		bool win;   //第一次分析的结果，赢的标记

		CopyChessBoard ();
		node = 0;
		win = false;

		if (button [4].interactable == false) {
			//第一次分析，寻找可用的棋位
			for (first.line = 0; first.line < 3; first.line++) {
				for (first.list = 0; first.list < 3; first.list++) {
					if (chessBoard [first.line, first.list] == 0) {
						//把当前棋位入树，记录当前棋位
						tree [node].position.line = first.line;
						tree [node].position.list = first.list;
						tree [node].weight = 0;  //初始化权重

						tree [node].chessBoard = new int[3, 3] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };    //初始化第一次分析的棋盘
						Array.Copy (chessBoard, tree [node].chessBoard, chessBoard.Length);    //拷贝当前棋盘状况
						tree [node].chessBoard [first.line, first.list] = 1;  //尝试下棋，后面分析这一步的情况
						if (IsWins (tree [node].chessBoard, 1) == 1) {
							//已找到最好情况，直接下棋
							buttonList [first.line * 3 + first.list].text = playerSide; 
							button [first.line * 3 + first.list].interactable = false;
							win = true;
							break;
						}//如果赢了就不需要再继续进行计算了，因为没有比这更好的情况了

						//第二次分析，寻找可用的棋位
						for (second.line = 0; second.line < 3; second.line++) {
							for (second.list = 0; second.list < 3; second.list++) {
								if (tree [node].chessBoard [second.line, second.list] == 0) {
									Array.Copy (tree [node].chessBoard, secondBoard, tree [node].chessBoard.Length);   //拷贝第一次分析得到的棋盘到第二次分析的棋盘
									secondBoard [second.line, second.list] = 2;  //模拟人下棋，后面分析情况

									tree [node].weight -= IsWins (secondBoard, 2);    //如果人赢，则不利，因此要用减号
								}
							}
						}
						node++; //分析树的节点下标+1
					}
				}

				tree [node].weight = 15; //树尾的标记

				if (win == true)
					break;  //找到最优解了，无需再分析
			}

			if (win == false) {
				//开始寻找最优解
				node = 0;   //设好情况为第一个节点
				for (int i = 0; tree [i].weight < 15; i++) {
					if (tree [node].weight < tree [i].weight){
						node = i;
					}
					else if (tree [node].weight <= tree [i].weight && ((tree [i].position.line * 3 + tree [i].position.list) == 0 || (tree [i].position.line * 3 + tree [i].position.list) == 2 || (tree [i].position.line * 3 + tree [i].position.list) == 6 || (tree [i].position.line * 3 + tree [i].position.list) == 8)){
						node = i;
					}
				}

//				Debug.Log ((tree [node].position.line * 3 + tree [node].position.list));
//				Debug.Log (tree [node].position.line);
//				Debug.Log (tree [node].position.list);
				buttonList [tree [node].position.line * 3 + tree [node].position.list].text = playerSide; //把棋放到最优解的位置
				button [tree [node].position.line * 3 + tree [node].position.list].interactable = false;

				return false;
			} else {
				return true;
			}

		} else {
			//中间必须要放棋子
			buttonList [4].text = playerSide;
			button [4].interactable = false;
			return false;
		}
	}

	void CopyChessBoard(){
		int line;
		int list;
		int n;

		for (line = 0, n = 0; line < 3; line++) {
			for (list = 0; list < 3; list++) {
				if (buttonList [n].text == ((playerSide == "X") ? "O" : "X")) {
					chessBoard [line, list] = 2;
				} else if (buttonList [n].text == ((playerSide == "X") ? "X" : "O")) {
					chessBoard [line, list] = 1;
				} else {
					chessBoard [line, list] = 0;
				}
				n++;
			}
		}
	}

	int IsWins(int[,] chessBoard, int playerSide)
	{
		if (chessBoard[0, 0] == playerSide && chessBoard[0, 1] == playerSide && chessBoard[0, 2] == playerSide)
		{
			return 1;
		}
		else if (chessBoard[1, 0] == playerSide && chessBoard[1, 1] == playerSide && chessBoard[1, 2] == playerSide)
		{
			return 1;
		}
		else if (chessBoard[2, 0] == playerSide && chessBoard[2, 1] == playerSide && chessBoard[2, 2] == playerSide)
		{
			return 1;
		}
		else if (chessBoard[0, 0] == playerSide && chessBoard[1, 1] == playerSide && chessBoard[2, 2] == playerSide)
		{
			return 1;
		}
		else if (chessBoard[0, 2] == playerSide && chessBoard[1, 1] == playerSide && chessBoard[2, 0] == playerSide)
		{
			return 1;
		}
		else if (chessBoard[0, 0] == playerSide && chessBoard[1, 0] == playerSide && chessBoard[2, 0] == playerSide)
		{
			return 1;
		}
		else if (chessBoard[0, 1] == playerSide && chessBoard[1, 1] == playerSide && chessBoard[2, 1] == playerSide)
		{
			return 1;
		}
		else if (chessBoard[0, 2] == playerSide && chessBoard[1, 2] == playerSide && chessBoard[2, 2] == playerSide)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}
}
