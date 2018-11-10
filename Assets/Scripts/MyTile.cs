﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTile : MonoBehaviour {
	public GameObject tileObj;
	public string st;
	private Texture2D mouseCursorAim;
	private bool cursorChanged = false;

	// Use this for initialization
	void Start()
	{
		gameObject.transform.Find("UnderFire").GetComponent<SpriteRenderer>().enabled = false;
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void AddShipToTile(GameObject s)
	{
		s.transform.parent = gameObject.transform;
		s.transform.localPosition = new Vector2(0, 0);
		Debug.Log("ship added to tile " + this.name);

	}

	public void ResetMoveHighlight()
	{
		SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
		sr.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	}

	void OnMouseOver()
	{
		if (this.gameObject.transform.Find("ship") != null)
		{
			GameObject ship = this.gameObject.transform.Find("ship").gameObject;
			if (ship != null)
			{
				if (ship.GetComponent<Unit>().side != GameManager.instance.currentPlayerSide)
				{
					mouseCursorAim = Resources.Load("Textures/aim") as Texture2D;
					Cursor.SetCursor(mouseCursorAim, new Vector2(mouseCursorAim.width / 2, mouseCursorAim.height / 2), CursorMode.Auto);
					cursorChanged = true;
				}
			}
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
