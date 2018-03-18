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
				if (game.Move(currentTokenPosition)) {
					for(int i = 0; i < boardAssembler.GetRows(); i++) {
						if (game.GetBoard()[currentTokenPosition, (boardAssembler.GetRows() - 1) - i] != null) {
							token.transform.parent = boardAssembler.GetHoles()[currentTokenPosition, (boardAssembler.GetRows() - 1) - i].transform;
							token.transform.localPosition = Vector3.zero;
							break;
						}
					}
					token = null;
					turnDone = true;
				}
			}
		}
	}

	public IEnumerator StartTurns() {
		while(game.GetGameState() == GameState.InProgress) {
			turnText.text = game.GetCurrentPlayer() == 0 ? bluesTurn : orangesTurn;
			turnText.color = game.GetCurrentPlayer() == 0 ? blue : orange;
			token = (GameObject)Instantiate(game.GetCurrentPlayer() == 0 ? blueToken : orangeToken, Vector3.zero, Quaternion.identity);
			token.transform.parent = boardAssembler.GetHoles()[0, boardAssembler.GetRows() - 1].transform.parent.transform;
			token.transform.localPosition = new Vector3(0, boardAssembler.GetHoles()[0,boardAssembler.GetRows() - 1].transform.localPosition.y + boardAssembler.columnYoffset, boardAssembler.GetHoles()[0,boardAssembler.GetRows() - 1].transform.localPosition.z);
			currentTokenPosition = 0;
			yield return new WaitUntil(() => turnDone);
			turnDone = false;
		}
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

		game = new Game(ai, boardAssembler.GetColumns(), boardAssembler.GetRows(), winNum, players);
		ToggleUI(false);
	
		StartCoroutine(StartTurns());
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
