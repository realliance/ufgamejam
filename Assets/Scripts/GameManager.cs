using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public string winTextPrefix = "# to Win: ";
	public Text winText;

	private Game game;

	int winNum;

	void updateText() {
		winText.text = winTextPrefix + winNum;
	}

	// Use this for initialization
	void Start () {
		winNum = 4;
		updateText();
	}
}
