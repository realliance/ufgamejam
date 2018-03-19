using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardAssembly : MonoBehaviour {

	public GameObject hole, leg, parent, table;

	public RPGCamera cameraScript;

	uint row, column, winNum;

	GameObject[,] holes;

	public float firstColumnOffset, columnYoffset, rightLegOffset = 0f;

	public string widthTextPrefix = "Width: ";

	public string heightTextPrefix = "Height: ";

	public Text widthText, heightText;

	void updateText() {
		widthText.text = widthTextPrefix + column;
		heightText.text = heightTextPrefix + row;
	}

	void assembleBoard() {
		foreach (Transform child in parent.transform) {
			GameObject.Destroy(child.gameObject);
		}
		holes = new GameObject[column, row];
		parent.transform.position = new Vector3(-(float)column / 2, -(float)row / 2, 0);
		GameObject leftLeg = (GameObject)Instantiate(leg);
		leftLeg.transform.parent = parent.transform;
		leftLeg.transform.localPosition = Vector3.zero;
		for (int c = 0; c <= column - 1; c++) {
			GameObject columnObj = new GameObject("Column" + c);
			columnObj.transform.parent = parent.transform;
			columnObj.transform.localPosition = new Vector3(firstColumnOffset + c, columnYoffset * row, 0);
			for (int r = 0; r <= row - 1; r++) {
				GameObject rowObj = (GameObject)Instantiate(hole, Vector3.zero, Quaternion.identity);
				rowObj.transform.parent = columnObj.transform;
				rowObj.transform.localPosition = new Vector3(0, -r, 0);
				holes[c, (row - 1) - r] = rowObj;
			}
		}
		GameObject rightLeg = (GameObject)Instantiate(leg);
		rightLeg.transform.parent = parent.transform;
		rightLeg.transform.localPosition = new Vector3(firstColumnOffset + column + rightLegOffset, 0 , 0);
		table.transform.position = new Vector3(table.transform.position.x, -((row * 0.5f) + 1.7f), table.transform.position.z);
		table.transform.localScale = new Vector3(2.5f * column, 1, 2.5f * column);
		updateText();
	}

	// Use this for initialization
	void Start () {
		row = 6;
		column = 7;
		winNum = 4;
		assembleBoard();
	}

	public void AddColumn() {
		column++;
		assembleBoard();
	}

	public void RemoveColumn() {
		if (column != 1) {
			column--;
			assembleBoard();
		}
	}

	public void AddRow() {
		row++;
		assembleBoard();
	}

	public void RemoveRow() {
		if (row != 1) {
			row--;
			assembleBoard();
		}
	}

	public void RevertToStandard() {
		row = 6;
		column = 7;
		assembleBoard();
	}

	public uint GetColumns() {
		return column;
	}

	public uint GetRows() {
		return row;
	}

	public GameObject[,] GetHoles() {
		return holes;
	}
}
