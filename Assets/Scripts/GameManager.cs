using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public GameObject tileObj;
	public float fieldSizeX;
	public float fieldSizeY;
	public float fieldZeroX;
	public float fieldZeroY;
	private List<MyTile> highlightedMoveTiles = new List<MyTile>();
	private List<MyTile> highlightedUnderFireTiles = new List<MyTile>();
	public List<MyTile> healTiles = new List<MyTile>();
	public List<GameObject> tiles = new List<GameObject>();
	public List<GameObject> ships = new List<GameObject>();
	public Color highlightMoveColor;
	private Color shipUnderFireHighlight;
	public Color friendlyShipHighlight;
	public Color healTileColor;
	public Color friendlyShipTileHighlight;
	public Player player_1;
	public Player player_2;
	public int currentPlayerSide;
	private Texture2D mouseCursorAim;
	public float HitProbability;
	public List<Unit> previouslySelectedShips = new List<Unit>();
	private bool gameOver = false;
	public Weather currentWeather;



	// Use this for initialization
	void Start () {
		Debug.Log("Game manager start");
		//GetComponent<Weather>().SetWeather();
		//previouslySelectedShips.Add(GetPlayerUnits(1)[0]);
		//SelectUnit(GetPlayerUnits(1)[0]);
	}

	
	void WinCondition()
	{
		if (gameOver)
		{
			return;
		}
		string winner = "unknown";
		if (GetPlayerUnits(player_1.side).Count == 0 || GetPlayerUnits(player_2.side).Count == 0)
		{
			if (GetPlayerUnits(player_1.side).Count == 0)
			{
				winner = "Player 2";
			}
			else
			{
				winner = "Player 1";
			}
			Debug.Log(winner + " wins!");
			GameObject resultScreenObj = GameObject.Find("ResultScreen");
			resultScreenObj.GetComponent<Canvas>().enabled = true;
			GameObject.Find("BattleResultText").GetComponent<Text>().text = winner + " wins!";
			gameOver = true;
		}
	}

	// Update is called once per frame
	void Update () {
		WinCondition();
		if (gameOver)
		{
			foreach (GameObject o in tiles)
			{
				Destroy(o);

			}
			GameObject.Find("BottomHUDCanvas").GetComponent<Canvas>().enabled = false;
			previouslySelectedShips = new List<Unit>();
			ships = new List<GameObject>();
			highlightedMoveTiles = new List<MyTile>();
			highlightedUnderFireTiles = new List<MyTile>();
			healTiles = new List<MyTile>();
			tiles = new List<GameObject>();
			return;			
		}
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
						previouslySelectedShips.Clear();
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
								currentShipTileParent.ResetHighlightsExceptHeal();
								selectedShip.transform.parent = t.transform;
								selectedShip.transform.localPosition = new Vector2(0, 0);
								selectedShip.GetComponent<Unit>().movementCompleted = true;
								t.GetComponent<MyTile>().AddShipToTile(selectedShip.gameObject);
								ResetMoveHighlight();
								ResetUnderFireHighlight();
								t.GetComponent<SpriteRenderer>().color = friendlyShipTileHighlight;
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

	public void Init()
	{
		gameOver = false;
		if (GameObject.Find("ResultScreen"))
		{
			GameObject.Find("ResultScreen").GetComponent<Canvas>().enabled = false;
		}
		if (GameObject.Find("BottomHUDCanvas"))
		{
			GameObject.Find("BottomHUDCanvas").GetComponent<Canvas>().enabled = true;
		}

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
		friendlyShipHighlight = new Color(0.38f, 1.0f, 0.55f, 1.0f);
		healTileColor = new Color(0.69f, 0.93f, 0.67f, 1.0f);
		friendlyShipTileHighlight = new Color(0.55f, 0.55f, 0.55f, 1.0f);

		float width = r.bounds.size[0];
		float height = r.bounds.size[1];
		Vector2 topRightCorner = new Vector2(1, 1);

		Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
		float screenWidth = edgeVector.x * 2;
		float screenHeight = edgeVector.y * 2;
		float screenZeroX = -edgeVector.x;
		float screenZeroY = edgeVector.y;

		Debug.Log("screenZeroX" + screenZeroX.ToString());
		Debug.Log("screenZeroY" + screenZeroY.ToString());


		fieldSizeX = 2 * (screenWidth / width);
		fieldSizeY = 2 * (screenHeight / height);
		fieldZeroX = screenZeroX;
		fieldZeroY = screenZeroY;
		Debug.Log("fieldSizeX = " + fieldSizeX.ToString());
		Debug.Log("fieldSizeY = " + fieldSizeY.ToString());

		for (int x = 0; x < (int)fieldSizeX; x++)
		{
			for (int y = 0; y < (int)fieldSizeY; y++)
			{
				tileObj = Resources.Load("Prefabs/Tile") as GameObject;
				GameObject t = Instantiate(tileObj, new Vector3((screenZeroX + (x + 1) * width - width / 2), (screenZeroY - (y + 1) * height + height / 2), 0), Quaternion.identity);
				t.name = string.Concat("tile_", ((int)fieldSizeX * y + (x + 1)).ToString());
				t.transform.parent = GameObject.Find("field").transform;
				tiles.Add(t);
			}
		}
		//AddShip(8, 5, "brig", "ironclad", player_1);
		//AddShip(2, 2, "brig", "fort", player_1);
		//AddShip(3, 4, "a", "steamboat", player_1);
		//AddShip(6, 5, "meduse", "monitor", player_1);
		//AddShip(3, 7, "brig2", "steam_fregate", player_1);
		//AddShip(4, 8, "brig3", "steam_corvette", player_2);
		//AddShip(12, 14, "fort", "fort_line2", player_2);
		//AddShip(4, 7, "galera1", "galera", player_2);
		//AddShip(7, 3, "ship", "ship_of_the_line_3deck", player_2);
		IDictionary<string, int> shipsDict = new Dictionary<string, int>();

		shipsDict.Add("ship_of_the_line_3deck", 2);
		shipsDict.Add("ship_of_the_line_2deck", 4);
		shipsDict.Add("fregate", 3);
		shipsDict.Add("tender", 2);
		shipsDict.Add("brig", 5);
		shipsDict.Add("galleon", 3);
		shipsDict.Add("steam_fregate", 1);
		shipsDict.Add("ironclad", 1);
		shipsDict.Add("galera", 4);
		shipsDict.Add("steam_corvette", 1);
		shipsDict.Add("monitor", 1);
		shipsDict.Add("steamboat", 1);

		IDictionary<string, int> fortsDict = new Dictionary<string, int>();

		fortsDict.Add("fort", 3);
		fortsDict.Add("fort_line2", 3);

		int x_min = 2;
		int x_max = (int)fieldSizeX / 2;
		int y_min = 2;
		int y_max = (int)fieldSizeY - 2;
		List<GameObject> fortTiles = new List<GameObject>();

		int fortX = UnityEngine.Random.Range(x_min, x_max);
		int fortY = UnityEngine.Random.Range(y_min, y_max);

		fortX = 3;
		fortY = 3;
		AddShip(fortX, fortY, "fort", "fort", player_1);
		fortTiles.AddRange(GetTilesAround(GetTileByXY(fortX, fortY), 1));

		fortX = (int)fieldSizeX / 2 - 2;
		fortY = 3;
		AddShip(fortX, fortY, "fort", "fort_line2", player_1);
		fortTiles.AddRange(GetTilesAround(GetTileByXY(fortX, fortY), 1));

		fortX = 3;
		fortY = (int)fieldSizeY / 2;
		AddShip(fortX, fortY, "fort", "fort", player_1);
		fortTiles.AddRange(GetTilesAround(GetTileByXY(fortX, fortY), 1));

		fortX = (int)fieldSizeX / 2 - 2;
		fortY = (int)fieldSizeY / 2;
		AddShip(fortX, fortY, "fort", "fort_line2", player_1);
		fortTiles.AddRange(GetTilesAround(GetTileByXY(fortX, fortY), 1));


		fortX = 3;
		fortY = (int)fieldSizeY - 2;
		AddShip(fortX, fortY, "fort", "fort", player_1);
		fortTiles.AddRange(GetTilesAround(GetTileByXY(fortX, fortY), 1));

		fortX = (int)fieldSizeX / 2 - 2;
		fortY = (int)fieldSizeY - 2;
		AddShip(fortX, fortY, "fort", "fort_line2", player_1);
		fortTiles.AddRange(GetTilesAround(GetTileByXY(fortX, fortY), 1));



		fortTiles.AddRange(GetTilesAround(GetTileByXY(fortX, fortY), 1));
		int fortTiles_i = 0;
		foreach (KeyValuePair<string, int> item in shipsDict)
		{
			for (int i = 0; i < item.Value; i++)
			{
				AddShip(fortTiles[fortTiles_i].GetComponent<MyTile>(), "s", item.Key, player_1);
				fortTiles_i++;
			}
		}



		fortX = UnityEngine.Random.Range(x_min, x_max);
		fortY = UnityEngine.Random.Range(y_min, y_max);
		fortTiles = new List<GameObject>();

		AddShip((int)fieldSizeX / 2 + 2, 3, "fort", "fort", player_2);
		AddShip((int)fieldSizeX - 2, 3, "fort", "fort_line2", player_2);
		AddShip((int)fieldSizeX / 2 + 2, (int)fieldSizeY / 2, "fort", "fort", player_2);
		AddShip((int)fieldSizeX - 2, (int)fieldSizeY / 2, "fort", "fort_line2", player_2);
		AddShip((int)fieldSizeX / 2 + 2, (int)fieldSizeY - 2, "fort", "fort_line2", player_2);
		AddShip((int)fieldSizeX - 2, (int)fieldSizeY - 2, "fort", "fort_line2", player_2);

		//fortTiles.AddRange(GetTilesAround(GetTileByXY(fortX, fortY), 1));
		//AddShip(fortTiles[0].GetComponent<MyTile>(), "brig", "galera", player_2);


		HitProbability = 1.0f;
		currentWeather = new Weather();
		currentWeather.Init();

		currentWeather.SetWeather();
		Debug.Log(GetPlayerUnits(1)[0].name);
		previouslySelectedShips.Add(GetPlayerUnits(1)[0]);
		SelectUnit(GetPlayerUnits(1)[0]);
	}

	void ShipsInitialArrangement()
	{
		IDictionary<string, int> shipsDict = new Dictionary<string, int>();

		shipsDict.Add("ship_of_the_line_3deck", 1);
		shipsDict.Add("ship_of_the_line_2deck", 1);
		shipsDict.Add("fregate", 1);
		shipsDict.Add("tender", 1);
		shipsDict.Add("brig", 1);
		shipsDict.Add("galleon", 1);
		shipsDict.Add("steam_fregate", 1);
		shipsDict.Add("ironclad", 1);
		shipsDict.Add("galera", 4);
		shipsDict.Add("steam_corvette", 1);
		shipsDict.Add("monitor", 1);
		shipsDict.Add("steamboat", 1);

		IDictionary<string, int> fortsDict = new Dictionary<string, int>();

		fortsDict.Add("fort", 3);
		fortsDict.Add("fort_line2", 3);

		int x_min = 2;
		int x_max = (int)fieldSizeX / 2;
		int y_min = 2;
		int y_max = (int)fieldSizeY - 2;
		List<GameObject> fortTiles = new List<GameObject>();


		foreach (KeyValuePair<string, int> item in fortsDict)
		{
			int fortX = UnityEngine.Random.Range(x_min, x_max);
			int fortY = UnityEngine.Random.Range(y_min, y_max);
			for (int i=0; i<item.Value; i++)
			{
				//while (GetTileByXY(fortX, fortY).GetComponent<MyTile>().isHeal)
				//{
					fortX = UnityEngine.Random.Range(x_min, x_max);
					fortY = UnityEngine.Random.Range(y_min, y_max);
				//}
				AddShip(fortX, fortY, "fort", item.Key, player_1);
				fortTiles.AddRange(GetTilesAround(GetTileByXY(fortX, fortY), 1));
			}
		}
		Debug.Log("forts completed");

		int fortTiles_i = 0;
		foreach (KeyValuePair<string, int> item in shipsDict)
		{
			for (int i = 0; i < item.Value; i++)
			{
				AddShip(fortTiles[fortTiles_i].GetComponent<MyTile>(), "s", item.Key, player_1);
				fortTiles_i++;
			}
		}

		x_min = (int)fieldSizeX / 2 + 2;
		x_max = (int)fieldSizeX - 2;
		y_min = 2;
		y_max = (int)fieldSizeY - 2;
		fortTiles = new List<GameObject>();


		foreach (KeyValuePair<string, int> item in fortsDict)
		{
			int fortX = UnityEngine.Random.Range(x_min, x_max); ;
			int fortY = UnityEngine.Random.Range(y_min, y_max);
			for (int i = 0; i < item.Value; i++)
			{
				while (!GetTileByXY(fortX, fortY).GetComponent<MyTile>().isHeal)
				{
					fortX = UnityEngine.Random.Range(x_min, x_max);
					fortY = UnityEngine.Random.Range(y_min, y_max);
				}
				AddShip(fortX, fortY, "fort", item.Key, player_2);
				fortTiles.AddRange(GetTilesAround(GetTileByXY(fortX, fortY), 1));
			}
		}
		Debug.Log("forts completed2");
		fortTiles_i = 0;
		foreach (KeyValuePair<string, int> item in shipsDict)
		{
			for (int i = 0; i < item.Value; i++)
			{
				AddShip(fortTiles[fortTiles_i].GetComponent<MyTile>(), "s", item.Key, player_2);
				fortTiles_i++;
			}
		}
	}

	void Awake()
	{
		MakeSingleton();
		Init();
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
		HighlightFriendlyShips();
	}

	int[] GetXYbyTileName( string s)
	{
		int[] xy = new int[] {0,0};
		int position = s.IndexOf("_");
		int n = Int32.Parse(s.Substring(position + 1));
		xy[0] = n % (int)fieldSizeX;
		xy[1] = n / (int)fieldSizeX+1;

		return xy;
	}

	GameObject GetTileByXY(int x, int y)
	{
		return GameObject.Find(string.Concat("tile_", ((int)fieldSizeX * (y-1) + x).ToString()));
	}

	void AddShip(int x, int y, string name, string ship_class, Player player )
	{
		GameObject shipObj = Resources.Load("Prefabs/ship") as GameObject;
		GameObject s = Instantiate(shipObj, new Vector3(0, 0, 0), Quaternion.identity);
		s.name = "ship";
		GetTileByXY(x, y).GetComponent<MyTile>().AddShipToTile(s);
		s.GetComponent<Unit>().SetupShip(ship_class, player.side, name);		
		
		ships.Add(s);
	}

	void AddShip(MyTile t, string name, string ship_class, Player player)
	{
		GameObject shipObj = Resources.Load("Prefabs/ship") as GameObject;
		GameObject s = Instantiate(shipObj, new Vector3(0, 0, 0), Quaternion.identity);
		s.name = "ship";
		t.AddShipToTile(s);
		s.GetComponent<Unit>().SetupShip(ship_class, player.side, name);

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
		else if (type == "heal")
		{
			SpriteRenderer t_sr = tile.GetComponent<SpriteRenderer>();
			t_sr.color = healTileColor;
			healTiles.Add(tile.GetComponent<MyTile>());
			tile.GetComponent<MyTile>().isHeal = true;
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

	public List<GameObject> GetTilesAround(GameObject t, int radius)
	{
		int[] xy = GetXYbyTileName(t.gameObject.name);
		int x = xy[0];
		int y = xy[1];
		List<GameObject> tiles = new List<GameObject>();
		for (int rel_x = -radius; rel_x <= radius; rel_x++)
		{
			for (int rel_y = -radius; rel_y <= radius; rel_y++)
			{
				if (Math.Abs(rel_x) == Math.Abs(rel_y) || rel_x == 0 || rel_y == 0)
				{
					if (x + rel_x <= fieldSizeX && x + rel_x >= 1 && y + rel_y <= fieldSizeY && y + rel_y >= 1)
					{
						if (rel_x == rel_y && rel_x == 0)
						{
							continue;
						}
						tiles.Add(GetTileByXY(x + rel_x, y + rel_y));
					}
				}
			}
		}
		return tiles;
	}

	public void HighlightArea(GameObject t, string type)
	{
		int[] xy = GetXYbyTileName(t.gameObject.name);
		int x = xy[0];
		int y = xy[1];
		Weather w = currentWeather;
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
			else if (type == "fire")
			{
				radius = u.fireRange;
			}
			else if (type == "heal")
			{
				radius = 1;
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
								if (currentWeather.currentWeather == Weather.weather_type.WIND)
								{
									int rad = Math.Max(Math.Abs(rel_x), Math.Abs(rel_y));
									if (rad <= radius - w.DistanceToCurrentWind(rel_x, rel_y))
									{
										HighlightTile(GetTileByXY(x + rel_x, y + rel_y), type);
									}
								}
								else
								{
									if (currentWeather.currentWeather == Weather.weather_type.CALM)
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
			int[] windDir = currentWeather.curWind;
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

	public void HighlightFriendlyShips()
	{
		//GetSelectedUnit().SetColor(new Color(1.0f, 1.0f, 1.0f, 1.0f));
		foreach (Unit u in GetPlayerUnits(currentPlayerSide))
		{
			u.gameObject.transform.parent.gameObject.GetComponent<SpriteRenderer>().color = friendlyShipTileHighlight;
		}
	}

	public void ResetAllShipsHighlights()
	{
		foreach (GameObject o in ships)
		{
			o.GetComponent<Unit>().SetColor(new Color(1.0f, 1.0f, 1.0f, 1.0f));
		}
	}

	public void ResetAllTilesHighlights()
	{
		foreach(GameObject t in tiles)
		{
			if (!healTiles.Contains(t.GetComponent<MyTile>()))
			{
				t.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			}
		}
	}

	public void NextShip()
	{
		Unit selectedShip = GetSelectedUnit();
		previouslySelectedShips.Add(selectedShip);
		Unit unitToSelect = null;
		List<Unit> playerShips = GetPlayerUnits(currentPlayerSide);
		foreach (Unit u in playerShips)
		{
			if (!previouslySelectedShips.Contains(u) && (!u.movementCompleted || !u.fireCompleted))
			{
				unitToSelect = u;
				//Debug.Log("NextShip(): " + u.shipName + " is selected");
				break;
			}
		}
		if (unitToSelect == null)
		{
			previouslySelectedShips.Clear();
			unitToSelect = playerShips[0];
		}
		SelectUnit(unitToSelect);
	}
}
