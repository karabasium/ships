﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTile : MonoBehaviour {
	public GameObject tileObj;
	public string st;
	private Texture2D mouseCursorAim;
	private Texture2D mouseCursorMove;
	private bool cursorChanged;
	public bool isHeal = false;

	// Use this for initialization
	void Start()
	{
		//gameObject.transform.Find("UnderFire").GetComponent<SpriteRenderer>().enabled = false;
		//cursorChanged = false;
	}

	void Awake()
	{
		gameObject.transform.Find("UnderFire").GetComponent<SpriteRenderer>().enabled = false;
		cursorChanged = false;
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void AddShipToTile(GameObject s)
	{
		s.transform.parent = gameObject.transform;
		s.transform.localPosition = new Vector2(0, 0);
		//Debug.Log("ship added to tile " + this.name);

	}

	public void ResetMoveHighlight()
	{
		SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
		if (isHeal)
		{
			sr.color = GameManager.instance.healTileColor;
		}
		else
		{
			sr.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
	}

	public void ResetHealHighlight()
	{
		SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
		sr.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		isHeal = false;
	}

	public void ResetHighlightsExceptHeal()
	{
		SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
		if (!isHeal)
		{			
			sr.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}
		else
		{
			sr.color = GameManager.instance.healTileColor;
		}
	}

	void OnMouseOver()
	{
		if (this.gameObject.transform.Find("ship") != null)
		{
			Unit ship = this.gameObject.transform.Find("ship").gameObject.GetComponent<Unit>();
			if (ship != null)
			{
				Unit selectedShip = GameManager.instance.GetSelectedUnit();
				if (ship.isUnderFire && selectedShip!=null)
				{
					mouseCursorAim = Resources.Load("Textures/aim") as Texture2D;
					Cursor.SetCursor(mouseCursorAim, new Vector2(mouseCursorAim.width / 2, mouseCursorAim.height / 2), CursorMode.Auto);
					cursorChanged = true;
				}
			}
		}
		else
		{

		}
	}

	void OnMouseExit()
	{
		if (cursorChanged)
		{
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
			cursorChanged = false;
		}
	}
}
