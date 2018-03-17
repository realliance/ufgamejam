using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public string winTextPrefix = "# to Win: ";
	public Text winText;

	private Game game;
	public BoardAssembly boardAssembler;

	uint winNum;

	public Dropdown playerOne, playerTwo;

	void updateText() {
		winText.text = winTextPrefix + winNum;
	}

	// Use this for initialization
	void Start () {
		winNum = 4;
		updateText();
	}

	public void Play() {
		AILogic ai = new AILogic();
		PlayerType[] players = new PlayerType[2];
		players[0] = playerOne.value == 0 ? PlayerType.Human : PlayerType.AI;
		players[0] = playerTwo.value == 0 ? PlayerType.Human : PlayerType.AI;

		game = new Game(ai, boardAssembler.GetColumns(), boardAssembler.GetRows(), winNum, players);
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
}
