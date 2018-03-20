using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public string winTextPrefix = "# to Win: ";
	public Text winText, errorText;

	public GameObject widthObj, heightObj, optionDropdown, playButton, playAgainButton;

	private Game game;
	public BoardAssembly boardAssembler;
	public ShowCustom showCustom;

	uint winNum;

	public Dropdown playerOne, playerTwo;

	bool turnDone = false;

	public float fallingGravity = 9.81f;
	public float waitUntilFall = 7f;

	public GameObject blueToken, orangeToken;
	GameObject token;
	uint currentTokenPosition;

	public Text turnText;
	public string bluesTurn = "Blues Turn";
	public Color32 blue;
	public string orangesTurn = "Oranges Turn";
	public Color32 orange;

	List<Rigidbody> tokenRigidbodies = new List<Rigidbody>();

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
				if (Input.GetAxisRaw("Horizontal") > 0) {
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


	float velocity = 0;
	float startingY = 0;
	bool fall = false;
	GameObject newTokenRef;

	void FixedUpdate() {
		if (fall) {
			newTokenRef.transform.localPosition = new Vector3(0, Mathf.Lerp(startingY, 0, velocity), 0);
			velocity += (0.5f * fallingGravity);
			if (newTokenRef.transform.localPosition.y == 0) {
				newTokenRef.transform.localPosition = Vector3.zero;
				velocity = 0;
				fall = false;
				newTokenRef = null;
			}
		}
	}

	public void OnMove(uint column, uint row, uint player) {
		tokenRigidbodies.Add(token.GetComponent<Rigidbody>());
		newTokenRef = token;
		token = null;

		newTokenRef.transform.parent = boardAssembler.GetHoles()[column, row].transform;
		startingY = newTokenRef.transform.localPosition.y;

		fall = true;

	}

	public void OnGameWin(uint winningPlayer) {
		turnText.text = (winningPlayer == 0 ? "Blue" : "Orange") + " Wins!";
		turnText.color = winningPlayer == 0 ? blue : orange;
	}

	public void OnGameTie() {
		turnText.text = "Tie!";
		turnText.color = Color.black;
	}

	public IEnumerator DelayUntilFall() {
		yield return new WaitForSeconds(waitUntilFall);
		foreach(Rigidbody r in tokenRigidbodies) {
			r.isKinematic = false;
			yield return new WaitForSeconds(0.05f);
		}
	}

	public void OnGameDone(EndState endState, uint? winningPlayer) {
		StartCoroutine(DelayUntilFall());
		playAgainButton.SetActive(true);
	}

	public void ToggleUI(bool visible) {
		playerOne.gameObject.SetActive(visible);	
		playerTwo.gameObject.SetActive(visible);
		winText.gameObject.SetActive(visible);
		widthObj.SetActive(visible);
		heightObj.SetActive(visible);
		optionDropdown.SetActive(visible);
		playButton.SetActive(visible);
		errorText.gameObject.SetActive(visible);
	}

	public void Play() {
		AiLogic ai = new AiLogic();
		PlayerType[] players = new PlayerType[2];
		players[0] = playerOne.value == 0 ? PlayerType.Human : PlayerType.AI;
		players[1] = playerTwo.value == 0 ? PlayerType.Human : PlayerType.AI;


		Callbacks callbacks = new Callbacks();
		callbacks.humanTurnStartCallback = OnHumansTurnStart;
		callbacks.gameVictoryCallback = OnGameWin;
		callbacks.moveCallback = OnMove;
		callbacks.gameTieCallback = OnGameTie;
		callbacks.gameFinishedCallback = OnGameDone;
		try {
			game = new Game(ai, callbacks, boardAssembler.GetColumns(), boardAssembler.GetRows(), winNum, players);
			showCustom.SetActiveElements(0);
			ToggleUI(false);
		} catch {
			errorText.gameObject.SetActive(true);
			errorText.text = "Board is Not Legal (Check to make sure it is playable!)";
		}
	}

	public void RestartGame() {
		SceneManager.LoadScene(0);
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
