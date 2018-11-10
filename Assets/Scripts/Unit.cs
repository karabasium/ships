﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
	public int side;
	public int hp = 3;
	public bool isSelected = false;
	public string shipName;
	public enum State

	{
		MOVEMENT,
		FIRE,
		END_TURN
	}
	public State state;

	// Use this for initialization
	void Start () {
		state = State.MOVEMENT;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void dealDamage(int dmg)
	{
		hp -= dmg;
		if (hp <= 0)
		{
			GameManager.instance.ships.Remove(gameObject);
			Destroy(gameObject);
			Debug.Log("unit destroyed");
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
	}


}
