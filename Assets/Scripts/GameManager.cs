using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public string winTextPrefix = "# to Win: ";
	public Text winText;

	public GameObject widthObj, heightObj, optionDropdown, playButton;

	private Game game;
	public BoardAssembly boardAssembler;

	uint winNum;

	public Dropdown playerOne, playerTwo;

	bool turnDone = false;

	public GameObject blueToken, orangeToken;
	GameObject token;
	uint currentTokenPosition;

	public Text turnText;
	public string bluesTurn = "Blues Turn";
	public Color32 blue;
	public string orangesTurn = "Oranges Turn";
	public Color32 orange;

	void updateText() {
		winText.text = winTextPrefix + winNum;
	}

	// Use this for initialization
	void Start () {
		winNum = 4;
		updateText();
	}

	public void Update() {
		if (token != null) {
			if (Input.GetButtonDown("Horizontal")) {
				if (Input.GetAxis("Horizontal") > 0) {
					currentTokenPosition++;
					if (boardAssembler.GetColumns() == currentTokenPosition) {
						currentTokenPosition = 0;
					}
				} else {
					if (currentTokenPosition == 0) {
						currentTokenPosition = boardAssembler.GetColumns();
					}
					currentTokenPosition--;
				}
				token.transform.parent = boardAssembler.GetHoles()[currentTokenPosition,0].transform.parent.transform;
				token.transform.localPosition = new Vector3(0, token.transform.localPosition.y, token.transform.localPosition.z);
			}
			if (Input.GetButtonDown("Submit")) {
				game.Move(currentTokenPosition);
			}
		}
	}

	public void OnHumansTurnStart(uint player) {
		turnText.text = player == 0 ? bluesTurn : orangesTurn;
		turnText.color = player == 0 ? blue : orange;
		token = (GameObject)Instantiate(player == 0 ? blueToken : orangeToken, Vector3.zero, Quaternion.identity);
		token.transform.parent = boardAssembler.GetHoles()[0, boardAssembler.GetRows() - 1].transform.parent.transform;
		token.transform.localPosition = new Vector3(0, boardAssembler.GetHoles()[0,boardAssembler.GetRows() - 1].transform.localPosition.y + boardAssembler.columnYoffset, boardAssembler.GetHoles()[0,boardAssembler.GetRows() - 1].transform.localPosition.z);
		currentTokenPosition = 0;
	}

	public void OnMove(uint column, uint row, uint player) {
		token.transform.parent = boardAssembler.GetHoles()[column, row].transform;
		token.transform.localPosition = Vector3.zero;
		token = null;
	}

	public void OnGameWin(uint winningPlayer) {
		turnText.text = (winningPlayer == 0 ? "Blue" : "Orange") + " Wins!";
		turnText.color = winningPlayer == 0 ? blue : orange;
	}

	public void OnGameTie() {
		turnText.text = "Tie!";
		turnText.color = Color.black;
	}

	public void ToggleUI(bool visible) {
		playerOne.gameObject.SetActive(visible);	
		playerTwo.gameObject.SetActive(visible);
		winText.gameObject.SetActive(visible);
		widthObj.SetActive(visible);
		heightObj.SetActive(visible);
		optionDropdown.SetActive(visible);
		playButton.SetActive(visible);
	}

	public void Play() {
		AiLogic ai = new AiLogic();
		PlayerType[] players = new PlayerType[2];
		players[0] = playerOne.value == 0 ? PlayerType.Human : PlayerType.AI;
		players[0] = playerTwo.value == 0 ? PlayerType.Human : PlayerType.AI;


		Callbacks callbacks = new Callbacks();
		callbacks.humanTurnStartCallback = OnHumansTurnStart;
		callbacks.gameVictoryCallback = OnGameWin;
		callbacks.moveCallback = OnMove;
		callbacks.gameTieCallback = OnGameTie;
		game = new Game(ai, callbacks, boardAssembler.GetColumns(), boardAssembler.GetRows(), winNum, players);
		ToggleUI(false);
	}

	public void AddNum() {
		winNum++;
		updateText();
	}

	public void RemoveNum() {
		if (winNum != 2) {
			winNum--;
			updateText();
		}
	}

	public void RevertToStandard() {
		winNum = 4;
		updateText();
	}
}
