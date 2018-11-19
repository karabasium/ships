using System;
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
	private Color highlightMoveColor;
	private Color shipUnderFireHighlight;
	public Player player_1;
	public Player player_2;
	public int currentPlayerSide;
	private Texture2D mouseCursorAim;
	public float HitProbability;



	// Use this for initialization
	void Start () {

	
	}
	


	// Update is called once per frame
	void Update () {
		if (GetSelectedUnit().fireCompleted && GetSelectedUnit().movementCompleted)
		{
			NextShip();
		}
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

			RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
			if (hit.collider != null)
			{
				//Debug.Log("click on " + hit.collider.gameObject.name);
				MyTile t = hit.collider.gameObject.GetComponent<MyTile>();
				GameObject ship = null;
				//Debug.Log(t.name);
				if (t.transform.Find("ship") != null)
				{
					ship = t.transform.Find("ship").gameObject;
				}
				if (ship != null)
				{
					Unit shipUnit = ship.GetComponent<Unit>();
					if (shipUnit.side == currentPlayerSide)
					{
						SelectUnit(shipUnit);
					}
					else
					{
						if ( ship.GetComponent<Unit>().isUnderFire)
						{
							ship.GetComponent<Unit>().dealDamage(1);
							GetSelectedUnit().Fire();
							if (GetSelectedUnit().fireCompleted)
							{
								ResetUnderFireHighlight();
							}
						}
					}
				}
				else
				{
					if (highlightedMoveTiles.Contains(t))
					{
						if (t.transform.Find("ship") == null)
						{//move ship
							Unit selectedShip = GetSelectedUnit();
							if (!selectedShip.GetComponent<Unit>().movementCompleted)
							{ 
								MyTile currentShipTileParent = selectedShip.transform.parent.GetComponent<MyTile>();
								selectedShip.transform.parent = t.transform;
								selectedShip.transform.localPosition = new Vector2(0, 0);
								selectedShip.GetComponent<Unit>().movementCompleted = true;
								t.GetComponent<MyTile>().AddShipToTile(selectedShip.gameObject);
								ResetMoveHighlight();
								ResetUnderFireHighlight();
								selectedShip.GetComponent<Unit>().movementCompleted = true;
								if (!selectedShip.GetComponent<Unit>().fireCompleted)
								{
									HighlightArea(hit.collider.gameObject, "fire");
								}
							}
						}
						else
						{
							//Debug.Log("Tile is already occupied");
						}
					}
					else
					{
						//Debug.Log("tile is not highlighted");
						ResetMoveHighlight();
					}
				}
			}
		}
	}

	public Unit GetSelectedUnit()
	{
		foreach (GameObject s in ships)
		{
			Unit unit = s.GetComponent<Unit>();
			if (unit.isSelected)
			{
				return unit;
			}
		}
		// Debug.Log("no ship is selected");
		return null;
	}

	void Awake()
	{
		MakeSingleton();

		tileObj = Resources.Load("Prefabs/Tile") as GameObject;
		//Debug.Log(tileObj);
		Renderer r = tileObj.GetComponent<Renderer>();
		SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
		Color initTileColor = sr.color;

		player_1 = new Player(1);
		player_2 = new Player(2);
		currentPlayerSide = player_1.side;
		highlightMoveColor = new Color(0.75f, 0.95f, 1.0f, 1.0f);
		shipUnderFireHighlight = new Color(0.95f, 0.45f, 0.35f, 1.0f);

		float width = r.bounds.size[0];
		float height = r.bounds.size[1];
		Vector2 topRightCorner = new Vector2(1, 1);

		Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
		float screenWidth = edgeVector.x * 2;
		float screenHeight = edgeVector.y * 2;
		float screenZeroX = -edgeVector.x;
		float screenZeroY = edgeVector.y;

		fieldSizeX = (int)(screenWidth / width);
		fieldSizeY = (int)(screenHeight / height);

		for (int x = 0; x < fieldSizeX; x++)
		{
			for (int y = 0; y < fieldSizeY; y++)
			{
				tileObj = Resources.Load("Prefabs/Tile") as GameObject;
				GameObject t = Instantiate(tileObj, new Vector3((screenZeroX + (x + 1) * width - width / 2), (screenZeroY - (y + 1) * height + height / 2), 0), Quaternion.identity);
				t.name = string.Concat("tile_", (fieldSizeX * y + (x + 1)).ToString());
			}
		}
		AddShip(8, 5, "brig", "brig", player_1);
		AddShip(3, 7, "brig2", "ship_of_the_line_2deck", player_1);
		AddShip(4, 8, "brig3", "brig", player_2);
		AddShip(6, 8, "brig4", "brig", player_2);
		AddShip(4, 7, "galera1", "galera", player_2);		
		GetComponent<Weather>().SetWeather();
		SelectUnit(GetPlayerUnits(1)[0]);

		HitProbability = 0.25f;
		Weather w = GetComponent<Weather>();
		Debug.Log("DISTANCE = " + w.DistanceToCurrentWind( -1, 1).ToString());
	}



	public void SelectUnit(Unit u)
	{
		u.MakeSelected();
		ResetMoveHighlight();
		ResetUnderFireHighlight();
		if (!u.movementCompleted)
		{
			HighlightArea(u.transform.parent.gameObject, "move");
			//Debug.Log("move highilighted");
		}
		if (!u.fireCompleted)
		{
			HighlightArea(u.transform.parent.gameObject, "fire");
			//Debug.Log("Fire highlighted");
		}
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

	void AddShip(int x, int y, string name, string ship_class, Player player )
	{
		GameObject shipObj = Resources.Load("Prefabs/ship") as GameObject;
		GameObject s = Instantiate(shipObj, new Vector3(0, 0, 0), Quaternion.identity);
		s.name = "ship";
		s.GetComponent<Unit>().SetupShip(ship_class, player.side, name);		
		GetTileByXY(x, y).GetComponent<MyTile>().AddShipToTile(s);
		ships.Add(s);
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
			//Debug.Log("under fire tile enabled");
			bool needHighlight = true;
			if (tile.transform.Find("ship") != null)
			{
				if (tile.transform.Find("ship").GetComponent<Unit>().side != currentPlayerSide)
				{
					tile.transform.Find("ship").GetComponent<Unit>().SetColor( shipUnderFireHighlight ); ;
					tile.transform.Find("ship").GetComponent<Unit>().isUnderFire = true;
				}
				else
				{
					needHighlight = false;
				}
			}
			if (needHighlight)
			{
				tile.transform.Find("UnderFire").GetComponent<SpriteRenderer>().enabled = true;
				highlightedUnderFireTiles.Add(tile.GetComponent<MyTile>());
			}
		}
	}

	public void ResetUnderFireHighlight()
	{
		foreach (MyTile t in highlightedUnderFireTiles)
		{
			t.transform.Find("UnderFire").GetComponent<SpriteRenderer>().enabled = false;
			if (t.transform.Find("ship") != null)
			{
				t.transform.Find("ship").GetComponent<Unit>().SetColor(new Color(1.0f, 1.0f, 1.0f, 1.0f));
				t.transform.Find("ship").GetComponent<Unit>().isUnderFire = false;
			}
		}
		highlightedUnderFireTiles.Clear();
	}

	public void ResetMoveHighlight()
	{
		foreach( MyTile t in highlightedMoveTiles)
		{
			t.ResetMoveHighlight();
		}
		highlightedMoveTiles.Clear();
	}

	void HighlightArea(GameObject t, string type)
	{
		int[] xy = GetXYbyTileName(t.gameObject.name);
		int x = xy[0];
		int y = xy[1];
		Weather w = GetComponent<Weather>();
		int radius = 0;
		Unit u = t.transform.Find("ship").GetComponent<Unit>();
		if (u != null)
		{
			if (type == "move")
			{
				if (w.currentWeather == Weather.weather_type.WIND)
				{
					radius = u.movementRange;
				}
				else
				{
					if (w.currentWeather == Weather.weather_type.CALM)
					{
						radius = u.calmMovementRange;
					}
				}
			}
			else
			{
				radius = u.fireRange;
			}
			for (int rel_x = -radius; rel_x <= radius; rel_x++)
			{
				for (int rel_y = -radius; rel_y <= radius; rel_y++)
				{
					if (Math.Abs(rel_x) == Math.Abs(rel_y) || rel_x == 0 || rel_y == 0)
					{
						if (x + rel_x <= fieldSizeX && x + rel_x >= 1 && y + rel_y <= fieldSizeY && y + rel_y >= 1)
						{
							if (type == "move")
							{
								if (GetComponent<Weather>().currentWeather == Weather.weather_type.WIND)
								{
									int rad = Math.Max(Math.Abs(rel_x), Math.Abs(rel_y));
									if (rad <= radius - w.DistanceToCurrentWind(rel_x, rel_y))
									{
										HighlightTile(GetTileByXY(x + rel_x, y + rel_y), type);
									}
								}
								else
								{
									if (GetComponent<Weather>().currentWeather == Weather.weather_type.CALM)
									{
										HighlightTile(GetTileByXY(x + rel_x, y + rel_y), type);
									}
								}
							}
							else
							{
								HighlightTile(GetTileByXY(x + rel_x, y + rel_y), type);
							}
						}
					}
				}
			}
		}
	}

	public List<Unit> GetPlayerUnits(int side)
	{
		List<Unit> playerUnits = new List<Unit>();
		foreach (GameObject s in ships)
		{
			if (s.GetComponent<Unit>().side == side)
			{
				playerUnits.Add(s.GetComponent<Unit>());
			}
		}
		return playerUnits;
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

	public void StormMovesShips()
	{
		foreach (Unit u in GetPlayerUnits(currentPlayerSide))
		{
			int[] xy = GetXYbyTileName(u.gameObject.transform.parent.name);
			int x = xy[0];
			int y = xy[1];
			int[] windDir = GetComponent<Weather>().curWind;
			MyTile tileToDrift=null;
			for (int radius = u.stormDrift; radius > 0; radius--)
			{
				tileToDrift = GetTileByXY(x + radius*windDir[0], y + (-1) * radius * windDir[0]).GetComponent<MyTile>();
				if (tileToDrift != null)
				{
					break;
				}
			}
			if (tileToDrift == null)
			{
				tileToDrift = GetTileByXY(x, y).GetComponent<MyTile>();
			}
			u.transform.parent = tileToDrift.gameObject.transform;
			u.transform.localPosition = new Vector2(0, 0);
			tileToDrift.AddShipToTile(u.gameObject);
			u.movementCompleted = true;
			ResetMoveHighlight();
			ResetUnderFireHighlight();
			if (!u.fireCompleted)
			{
				HighlightArea(tileToDrift.gameObject, "fire");
			}
		}
	}

	public void NextShip()
	{
		Unit selectedShip = GetSelectedUnit();
		foreach (Unit u in GetPlayerUnits(GameManager.instance.currentPlayerSide))
		{
			if (u != selectedShip && (!u.movementCompleted || !u.fireCompleted))
			{
				SelectUnit(u);
				//Debug.Log("NextShip(): " + u.shipName + " is selected");
				break;
			}
		}
	}
}
