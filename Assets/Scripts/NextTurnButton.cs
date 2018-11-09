using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextTurnButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnButtonClick()
	{
		if (GameManager.instance.currentPlayerSide == 1)
		{
			GameManager.instance.currentPlayerSide = 2;
			Debug.Log("Current player is 2");
		}
		else
		{
			GameManager.instance.currentPlayerSide = 1;
			Debug.Log("Current player is 1");
		}
	}
}
