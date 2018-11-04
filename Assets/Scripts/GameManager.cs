using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public GameObject tileObj;
	private int fieldSizeX;
	private int fieldSizeY;
	Color highlightMoveColor = new Color(1.0f, 0.5f, 1.0f, 1f);

	// Use this for initialization
	void Start () {
		
		tileObj = Resources.Load("Prefabs/Tile") as GameObject;
		Renderer r = tileObj.GetComponent<Renderer>();
		SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
		Color initTileColor = sr.color;



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
			 	GameObject t = Instantiate(tileObj, new Vector3((screenZeroX + (x + 1) * width - width/2), (screenZeroY - (y+1)*height+height/2), 0), Quaternion.identity);
				t.name = string.Concat("tile_", (fieldSizeX * y + (x + 1)).ToString());
			}
		}
		HighlightMoveArea(25, 15, 6);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Awake()
	{
		MakeSingleton();
	}

	GameObject GetTileByXY(int x, int y)
	{
		return GameObject.Find(string.Concat("tile_", (fieldSizeX * (y-1) + x).ToString()));
	}

	void HighlightTile( GameObject tile)
	{
		SpriteRenderer t_sr = tile.GetComponent<SpriteRenderer>();
		t_sr.color = highlightMoveColor;
	}

	void HighlightMoveArea( int x, int y, int radius)
	{
		int initX = x - radius;
		int initY = y - radius;
		int maxX = x + radius;
		int maxY = y + radius;
		if (initX <= 0) { initX = 1; }		
		if (initY <= 0) { initY = 1; }
		if (maxX > fieldSizeX) { maxX = fieldSizeX; }
		if (maxY > fieldSizeY) { maxY = fieldSizeY; }

		for (int rel_x = -radius; rel_x <= radius; rel_x++)
		{
			for (int rel_y = -radius; rel_y <= radius; rel_y++)
			{
				Debug.Log("rel_x = " + rel_x + ", rel_y = " + rel_y);
				if (Math.Abs(rel_x) == Math.Abs(rel_y) || rel_x == 0 || rel_y == 0)
				{
					Debug.Log("Bingo!");
					Debug.Log("(x + rel_x = " + (x + rel_x) + ", y + rel_y = " + (y + rel_y));
					if (x + rel_x <= fieldSizeX && x + rel_x >= 1 && y + rel_y <= fieldSizeY && y + rel_y >= 1)
					{
						HighlightTile(GetTileByXY(x + rel_x, y + rel_y));
					}
				}
			}
		}

		/*	for (int X = initX; X <= maxX; X++)
		{
			for (int Y = initY; Y <= maxY; Y++)
			{
				Debug.Log("X = " + X + ", Y = " + Y);
				if (Math.Abs(relX) == Math.Abs(relY) || X==1 || Y == 1)
				{
					Debug.Log("Bingo!");
					HighlightTile(GetTileByXY(X, Y));
				}
				relY++;
			}
			relX++;
			relY = 1;
		}*/
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
