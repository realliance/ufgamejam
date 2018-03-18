using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCustom : MonoBehaviour {

	public GameObject width, height, winNum;

	public BoardAssembly boardAssembler;
	public GameManager gameManager;

	// Use this for initialization
	void Start () {
		width.SetActive(false);
		height.SetActive(false);
		winNum.SetActive(false);
	}

	public void DropdownChanged(int value) {
		width.SetActive(value == 1);
		height.SetActive(value == 1);
		winNum.SetActive(value == 1);
		if (value == 0) {
			boardAssembler.RevertToStandard();
			gameManager.RevertToStandard();
		}
	}

}
