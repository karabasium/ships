using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
	public int side;
	public int hp;
	private int maxHP;
	public bool isSelected;
	public bool isUnderFire;
	public string shipName;
	public bool movementCompleted;
	public bool fireCompleted;
	public int shotsCount;
	private int maxShotsCount;
	private float height;
	private float width;
	public int fireRange;
	public int movementRange;
	public int idleTurnsCount;
	public int calmMovementRange;
	public int stormDrift;
	public string shipClass;
	private GameObject tileObj;
	public List<GameObject> hp_spots = new List<GameObject>();
	private SpriteRenderer shipSprite;


	

	// Use this for initialization
	void Start () {
	}
	
	void AddHPVisual()
	{
		height = shipSprite.bounds.size.y;
		width = shipSprite.bounds.size.x;
		//Debug.Log("height = " + height.ToString() + ", width = " + width.ToString());
		GameObject HP = Resources.Load("Prefabs/HP") as GameObject;
		float hp_width = HP.GetComponent<SpriteRenderer>().bounds.size.x;
		float hp_space = hp_width / 8;
		float total_len = hp * hp_width + (hp - 1) * hp_space;
		Vector3 pos = gameObject.transform.position;
		float start_x = pos[0] - total_len / 2 + hp_width / 2;
		for (int i = 0; i < hp; i++)
		{
			GameObject hpObj = Instantiate(HP, new Vector3(start_x + (hp_width + hp_space) * i, pos[1] - 0.6f * height, 0), Quaternion.identity);
			hpObj.transform.parent = gameObject.transform;
			hp_spots.Add(hpObj);
		}
	}

	public void HealHP(int heal_hp)
	{
		int oldHP = this.hp;
		hp += heal_hp;
		if (hp > maxHP)
		{			
			hp = maxHP;
		}
		RemoveVisualHP(oldHP);
		AddHPVisual();
	}

	void RemoveVisualHP(int n)
	{
		for (int i = 0; i < n; i++)
		{
			GameObject gameObjToRemove = hp_spots[hp_spots.Count - 1];
			hp_spots.RemoveAt(hp_spots.Count - 1);
			Destroy(gameObjToRemove);
			Debug.Log("hp removed");
		}
	}

	void Awake()
	{
		isSelected = false;
		movementCompleted = false;
		fireCompleted = false;
		isUnderFire = false;
		idleTurnsCount = 0;
	}

	// Update is called once per frame
	void Update () {
	}

	public void dealDamage(int dmg)
	{

		float rnd = Random.Range(0.0f, 1.0f);
		Vector3 pos = gameObject.transform.position;
		GameObject flyOffText = Instantiate (Resources.Load("Prefabs/FlyOffText") as GameObject, pos, Quaternion.identity);
		flyOffText.transform.parent = gameObject.transform;

		if (rnd <= GameManager.instance.HitProbability)
		{
			Debug.Log("Hit! hp = " + hp.ToString());
			hp -= dmg;
			Debug.Log("Hit! hp = " + hp.ToString());
			flyOffText.GetComponent<TextMesh>().text = "Hit!";
			if (hp <= 0)
			{
				GameManager.instance.ships.Remove(gameObject);
				Destroy(gameObject);
				Debug.Log("unit destroyed");
				Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
				if (shipClass == "fort")
				{
					foreach(GameObject t_obj in GameManager.instance.GetTilesAround(tileObj,1))
					{
						t_obj.GetComponent<MyTile>().ResetHealHighlight();
						GameManager.instance.healTiles.Remove(t_obj.GetComponent<MyTile>());
					}
				}
			}
			else
			{
				RemoveVisualHP(dmg);
			}
		}
		else
		{
			flyOffText.GetComponent<TextMesh>().text = "Miss!";
			Debug.Log("miss!");
		}
		Debug.Log(rnd);
	}
	public void MakeSelected()
	{
		Unit u = GameManager.instance.GetSelectedUnit();
		if(u != null)
		{
			u.isSelected = false;
		}
		this.isSelected = true;
		Debug.Log("ship " + this.shipName + " is selected");
	}

	public void SetupShip( string ship_class, int side, string name)
	{
		shipSprite = gameObject.transform.Find(ship_class).GetComponent<SpriteRenderer>();
		tileObj = gameObject.transform.parent.gameObject;
		Debug.Log("shipTile = " + tileObj.name);
		shipSprite.enabled = true;
		shipName = name;
		shipClass = ship_class;
		this.side = side;		
		if (ship_class == "brig")
		{
			hp = 3;
			maxHP = hp;
			maxShotsCount = 1;
			fireRange = 3;

			movementRange = 5;
			calmMovementRange = 1;
			stormDrift = 2;
		}
		else if (ship_class == "ship_of_the_line_2deck")
		{
			hp = 5;
			maxHP = hp;
			maxShotsCount = 2;
			fireRange = 5;

			movementRange = 5;
			calmMovementRange = 0;
			stormDrift = 3;
		}
		else if (ship_class == "ship_of_the_line_3deck")
		{
			hp = 7;
			maxHP = hp;
			maxShotsCount = 3;
			fireRange = 6;

			movementRange = 5;
			calmMovementRange = 0;
			stormDrift = 3;
		}
		else if (ship_class == "galera")
		{
			hp = 3;
			maxHP = hp;
			maxShotsCount = 1;
			fireRange = 3;

			movementRange = 5;
			calmMovementRange = 3;
			stormDrift = 1;
		}
		else if (ship_class == "fregate")
		{
			hp = 4;
			maxHP = hp;
			maxShotsCount = 1;
			fireRange = 5;

			movementRange = 6;
			calmMovementRange = 0;
			stormDrift = 2;
		}
		else if (ship_class == "tender")
		{
			hp = 2;
			maxHP = hp;
			maxShotsCount = 1;
			fireRange = 1;

			movementRange = 6;
			calmMovementRange = 1;
			stormDrift = 2;
		}
		else if (ship_class == "fort")
		{
			hp = 7;
			maxHP = hp;
			maxShotsCount = 3;
			fireRange = 5;

			movementRange = 0;
			calmMovementRange = 0;
			stormDrift = 0;

			GameManager.instance.HighlightArea(tileObj, "heal");			
		}
		shotsCount = maxShotsCount;
		AddHPVisual();
	}
	public void SetColor(Color color)
	{
		shipSprite.GetComponent<SpriteRenderer>().color = color;
	}

	public void Fire()
	{
		shotsCount--;
		if (shotsCount <= 0)
		{
			fireCompleted = true;
			shotsCount = 0;
		}
	}

	public void NextTurnSetup()
	{
		movementCompleted = false;
		fireCompleted = false;
		shotsCount = maxShotsCount;
	}

}
