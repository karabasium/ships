using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public GameObject tileObj;
	private int fieldSizeX;
	private int fieldSizeY;
	List<GameObject> ships = new List<GameObject>();
	Color highlightMoveColor = new Color(1.0f, 0.5f, 1.0f, 1.0f);

	// Use this for initialization
	void Start () {

		tileObj = Resources.Load("Prefabs/Tile") as GameObject;
		Debug.Log(tileObj);
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
				tileObj = Resources.Load("Prefabs/Tile") as GameObject;
				GameObject t = Instantiate(tileObj, new Vector3((screenZeroX + (x + 1) * width - width/2), (screenZeroY - (y+1)*height+height/2), 0), Quaternion.identity);
				t.name = string.Concat("tile_", (fieldSizeX * y + (x + 1)).ToString());
			}
		}
		AddShip(10, 10, "brig");
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
				string tileName = hit.collider.gameObject.name;
				int[] xy = GetXYbyTileName(tileName);
				Debug.Log(xy);
				HighlightMoveArea(xy[0], xy[1], 6);
			}
		}
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

	void AddShip(int x, int y, string name )
	{
		GameObject shipObj = Resources.Load("Prefabs/ship") as GameObject;
		GameObject s = Instantiate(shipObj, new Vector3(0, 0, 0), Quaternion.identity);
		s.name = name;
		GameObject t = GetTileByXY(x, y);	
		s.transform.parent = t.transform;
		s.transform.localPosition = new Vector2(0, 0);
		HighlightTile(t);
		MyTile tl = t.GetComponent<MyTile>();
		tl.AddShipToTile(s);
		ships.Add(s);
	}

	void HighlightTile( GameObject tile)
	{
		SpriteRenderer t_sr = tile.GetComponent<SpriteRenderer>();
		t_sr.color = highlightMoveColor;
	}

	void HighlightMoveArea( int x, int y, int radius)
	{
		for (int rel_x = -radius; rel_x <= radius; rel_x++)
		{
			for (int rel_y = -radius; rel_y <= radius; rel_y++)
			{
				if (Math.Abs(rel_x) == Math.Abs(rel_y) || rel_x == 0 || rel_y == 0)
				{
					if (x + rel_x <= fieldSizeX && x + rel_x >= 1 && y + rel_y <= fieldSizeY && y + rel_y >= 1)
					{
						HighlightTile(GetTileByXY(x + rel_x, y + rel_y));
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
