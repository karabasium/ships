﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public GameObject tileObj;
	private int fieldSizeX;
	private int fieldSizeY;
	private List<MyTile> highlightedMoveTiles = new List<MyTile>();
	private List<MyTile> highlightedUnderFireTiles = new List<MyTile>();
	public List<GameObject> ships = new List<GameObject>();
	private Color highlightMoveColor = new Color(1.0f, 0.5f, 1.0f, 1.0f);
	private Color shipUnderFireHighlight = new Color(0.95f, 0.45f, 0.35f, 1.0f);
	public Player player_1;
	public Player player_2;
	public int currentPlayerSide;
	private Texture2D mouseCursorAim;
	private GameState gameState = GameState.FIRE;
	private bool someShipIsSelected = false;

	private enum GameState
	{
		MOVEMENT,
		FIRE
	}



	// Use this for initialization
	void Start () {

		tileObj = Resources.Load("Prefabs/Tile") as GameObject;
		Debug.Log(tileObj);
		Renderer r = tileObj.GetComponent<Renderer>();
		SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
		Color initTileColor = sr.color;

		player_1 = new Player( 1 );
		player_2 = new Player( 2 );
		currentPlayerSide = player_1.side;

		float width = r.bounds.size[0];
		float height = r.bounds.size[1];
		Vector2 topRightCorner = new Vector2(1, 1);

		Vector2 edgeVector = Camera.main.ViewportToWorldPoint( topRightCorner );
		float screenWidth = edgeVector.x * 2;
		float screenHeight = edgeVector.y * 2;
		float screenZeroX = - edgeVector.x;
		float screenZeroY = edgeVector.y;

		fieldSizeX = (int) (screenWidth / width);
		fieldSizeY = (int) (screenHeight / height);

		for (int x = 0; x < fieldSizeX; x++)
		{
			for (int y = 0; y < fieldSizeY; y++)
			{
				tileObj = Resources.Load("Prefabs/Tile") as GameObject;
				GameObject t = Instantiate(tileObj, new Vector3((screenZeroX + (x + 1) * width - width/2), (screenZeroY - (y+1)*height+height/2), 0), Quaternion.identity);
				t.name = string.Concat("tile_", (fieldSizeX * y + (x + 1)).ToString());
			}
		}
		AddShip(3, 3, "brig", player_1);
		AddShip(3, 7, "brig2", player_1);
		AddShip(4, 8, "brig3", player_2);
		AddShip(6, 8, "brig4", player_2);

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

			RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
			if (hit.collider != null)
			{
				Debug.Log("click on " + hit.collider.gameObject.name);
				MyTile t = hit.collider.gameObject.GetComponent<MyTile>();
				GameObject ship = null;
				Debug.Log(t.name);
				if (t.transform.Find("ship") != null)
				{
					ship = t.transform.Find("ship").gameObject;
				}
				if (ship != null)
				{
					Unit shipUnit = ship.GetComponent<Unit>();
					if (shipUnit.side == currentPlayerSide)
					{
						ResetMoveHighlight();
						string tileName = hit.collider.gameObject.name;
						int[] xy = GetXYbyTileName(tileName);
						HighlightArea(xy[0], xy[1], 4, "move");
						HighlightArea(xy[0], xy[1], 3, "fire");
						shipUnit.isSelected = true;
					}
					else
					{
						Debug.Log("You can't select an enemy ship");
						if (gameState == GameState.FIRE && someShipIsSelected)
						{
							ship.GetComponent<Unit>().dealDamage(4);
						}
					}
				}
				else
				{
					if (highlightedMoveTiles.Contains(t))
					{
						if (t.transform.Find("ship") == null)
						{//move ship
							GameObject selectedShip = GetSelectedShip();
							MyTile currentShipTileParent = selectedShip.transform.parent.GetComponent<MyTile>();						
							selectedShip.transform.parent = t.transform;
							selectedShip.transform.localPosition = new Vector2(0, 0);
							selectedShip.GetComponent<Unit>().movementCompleted = true;
							t.GetComponent<MyTile>().AddShipToTile(selectedShip);
							ResetMoveHighlight();
							ResetUnderFireHighlight();
							if (!selectedShip.GetComponent<Unit>().fireCompleted)
							{
								int[] xy = GetXYbyTileName(t.gameObject.name);								
								HighlightArea(xy[0], xy[1], 3, "fire");
							}
						}
						else
						{
							Debug.Log("Tile is already occupied");
						}
					}
					else
					{
						Debug.Log("tile is not highlighted");
						ResetMoveHighlight();
					}
				}
			}
		}
	}

	GameObject GetSelectedShip()
	{
		foreach (GameObject s in ships)
		{
			if (s.GetComponent<Unit>().isSelected)
			{
				return s;
			}
		}
		Debug.Log("no ship is selected");
		return null;
	}

	void Awake()
	{
		MakeSingleton();
		//AddShip(10, 10, "brig");
	}

	int[] GetXYbyTileName( string s)
	{
		int[] xy = new int[] {0,0};
		int position = s.IndexOf("_");
		int n = Int32.Parse(s.Substring(position + 1));
		xy[0] = n % fieldSizeX;
		xy[1] = n / fieldSizeX+1;

		return xy;
	}

	GameObject GetTileByXY(int x, int y)
	{
		return GameObject.Find(string.Concat("tile_", (fieldSizeX * (y-1) + x).ToString()));
	}

	void AddShip(int x, int y, string name, Player player )
	{
		GameObject shipObj = Resources.Load("Prefabs/ship") as GameObject;
		GameObject s = Instantiate(shipObj, new Vector3(0, 0, 0), Quaternion.identity);
		s.name = "ship";
		GetTileByXY(x, y).GetComponent<MyTile>().AddShipToTile(s);
		ships.Add(s);
		s.GetComponent<Unit>().side = player.side;
		s.GetComponent<Unit>().shipName = name;
	}

	void HighlightTile( GameObject tile, string type)
	{
		if (type == "move")
		{
			SpriteRenderer t_sr = tile.GetComponent<SpriteRenderer>();
			t_sr.color = highlightMoveColor;
			highlightedMoveTiles.Add(tile.GetComponent<MyTile>());
		}
		else if (type == "fire")
		{
			tile.transform.Find("UnderFire").GetComponent<SpriteRenderer>().enabled = true;
			if (tile.transform.Find("ship") != null)
			{
				if (tile.transform.Find("ship").GetComponent<Unit>().side != currentPlayerSide)
				{
					tile.transform.Find("ship").GetComponent<SpriteRenderer>().color = shipUnderFireHighlight;
				}
			}
			highlightedUnderFireTiles.Add(tile.GetComponent<MyTile>());
		}
	}

	void ResetUnderFireHighlight()
	{
		foreach (MyTile t in highlightedUnderFireTiles)
		{
			t.transform.Find("UnderFire").GetComponent<SpriteRenderer>().enabled = false;
			if (t.transform.Find("ship") != null)
			{
				t.transform.Find("ship").GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			}
		}
		highlightedUnderFireTiles.Clear();
	}

	void ResetMoveHighlight()
	{
		foreach( MyTile t in highlightedMoveTiles)
		{
			t.ResetMoveHighlight();
		}
		highlightedMoveTiles.Clear();
		foreach(GameObject s in ships)
		{
			s.GetComponent<Unit>().isSelected = false;
		}
		someShipIsSelected = false;
	}

	void HighlightArea( int x, int y, int radius, string type)
	{
		for (int rel_x = -radius; rel_x <= radius; rel_x++)
		{
			for (int rel_y = -radius; rel_y <= radius; rel_y++)
			{
				if (Math.Abs(rel_x) == Math.Abs(rel_y) || rel_x == 0 || rel_y == 0)
				{
					if (x + rel_x <= fieldSizeX && x + rel_x >= 1 && y + rel_y <= fieldSizeY && y + rel_y >= 1)
					{
						HighlightTile(GetTileByXY(x + rel_x, y + rel_y), type);
					}
				}
			}
		}
	}

	void MakeSingleton()
	{
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}
}
