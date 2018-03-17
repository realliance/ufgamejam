using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardAssembly : MonoBehaviour {


	public GameObject hole, leg, parent;

	uint row, column, winNum;

	public float firstColumnOffset, columnYoffset, rightLegOffset = 0f;

	void assembleBoard() {
		GameObject leftLeg = (GameObject)Instantiate(leg);
		leftLeg.transform.parent = parent.transform;
		leftLeg.transform.localPosition = Vector3.zero;
		for (int c = 0; c <= column - 1; c++) {
			GameObject columnObj = new GameObject("Column" + c);
			columnObj.transform.parent = parent.transform;
			columnObj.transform.localPosition = new Vector3(firstColumnOffset + c, columnYoffset, 0);
			for (int r = 0; r <= row - 1; r++) {
				GameObject rowObj = (GameObject)Instantiate(hole, Vector3.zero, Quaternion.identity);
				rowObj.transform.parent = columnObj.transform;
				rowObj.transform.localPosition = new Vector3(0, -r, 0);
			}
		}
		GameObject rightLeg = (GameObject)Instantiate(leg);
		rightLeg.transform.parent = parent.transform;
		rightLeg.transform.localPosition = new Vector3(firstColumnOffset + column + rightLegOffset, 0 , 0);
	}

	// Use this for initialization
	void Start () {
		row = 6;
		column = 7;
		winNum = 4;
		assembleBoard();
	}
}
