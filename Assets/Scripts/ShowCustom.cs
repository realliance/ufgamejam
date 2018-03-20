using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCustom : MonoBehaviour {

	public GameObject width, height, winNum, bg, bgMid;

	public BoardAssembly boardAssembler;
	public GameManager gameManager;

	// Use this for initialization
	void Start () {
		width.SetActive(false);
		height.SetActive(false);
		winNum.SetActive(false);
        bg.SetActive(false);
        bgMid.SetActive(false);
	}

	public void SetActiveElements(int value) {
		width.SetActive(value == 1);
		height.SetActive(value == 1);
		winNum.SetActive(value == 1);
        bg.SetActive(value == 1);
        bgMid.SetActive(value == 1);
	}

	public void DropdownChanged(int value) {
		SetActiveElements(value);
        if (value == 0) {
			boardAssembler.RevertToStandard();
			gameManager.RevertToStandard();
		}
	}

}
