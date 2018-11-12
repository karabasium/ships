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
		int nextPlayer = 2;
		if (GameManager.instance.currentPlayerSide == 2)
		{
			nextPlayer = 1;
		}
		GameManager.instance.currentPlayerSide = nextPlayer;
		ResetPlayerShipsActions(nextPlayer);
	}

	void ResetPlayerShipsActions(int playerSide )
	{
		foreach (GameObject shipObj in GameManager.instance.ships)
		{
			Unit unit = shipObj.GetComponent<Unit>();
			if (unit.side == playerSide)
			{
				unit.movementCompleted = false;
				unit.fireCompleted = false;
			}
		}
		GameManager.instance.GetSelectedUnit().isSelected = false;
		GameManager.instance.ResetMoveHighlight();
		GameManager.instance.ResetUnderFireHighlight();
	}
}
