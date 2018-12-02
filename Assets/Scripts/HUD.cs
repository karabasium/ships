using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour {
	private bool ready = false;
	// Use this for initialization
	void Start () {
		ready = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void NextTurn()
	{
		foreach (Unit u in GameManager.instance.GetPlayerUnits(GameManager.instance.currentPlayerSide))
		{
			if (!u.movementCompleted && !u.fireCompleted && u.transform.parent.gameObject.GetComponent<MyTile>().isHeal)
			{
				u.idleTurnsCount++;
				u.HealHP(1);
			}
			else
			{
				u.idleTurnsCount = 0;
			}
		}
		int nextPlayer = 2;
		if (GameManager.instance.currentPlayerSide == 2)
		{
			nextPlayer = 1;
		}
		GameManager.instance.currentPlayerSide = nextPlayer;
		ResetPlayerShipsActions(nextPlayer);
		GameManager.instance.ResetAllShipsHighlights();
		GameManager.instance.ResetAllTilesHighlights();
		GameManager.instance.currentWeather.SetWeather();
		GameManager.instance.previouslySelectedShips.Clear();
		GameManager.instance.previouslySelectedShips.Add(GameManager.instance.GetPlayerUnits(nextPlayer)[0]);
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


	public bool isReady()
	{
		return ready;
	}
}
