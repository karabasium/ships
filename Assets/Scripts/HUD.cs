using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void NextTurn()
	{
		int nextPlayer = 2;
		if (GameManager.instance.currentPlayerSide == 2)
		{
			nextPlayer = 1;
		}
		GameManager.instance.currentPlayerSide = nextPlayer;
		ResetPlayerShipsActions(nextPlayer);
		GameManager.instance.GetComponent<Weather>().SetWeather();
		GameManager.instance.SelectUnit( GameManager.instance.GetPlayerUnits(nextPlayer)[0] );		
	}

	void ResetPlayerShipsActions(int playerSide )
	{
		foreach (GameObject shipObj in GameManager.instance.ships)
		{
			Unit unit = shipObj.GetComponent<Unit>();
			if (unit.side == playerSide)
			{
				unit.NextTurnSetup();
			}
		}
		GameManager.instance.GetSelectedUnit().isSelected = false;
		GameManager.instance.ResetMoveHighlight();
		GameManager.instance.ResetUnderFireHighlight();
	}

	public void NextShip()
	{
		Debug.Log("NextShip(): Button clicked");
		GameManager.instance.NextShip();
	}
}
