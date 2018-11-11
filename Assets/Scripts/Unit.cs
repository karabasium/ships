using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
	public int side;
	public int hp = 3;
	public bool isSelected = false;
	public bool isUnderFire;
	public string shipName;
	public bool movementCompleted;
	public bool fireCompleted;

	// Use this for initialization
	void Start () {
		movementCompleted = false;
		fireCompleted = false;
		isUnderFire = false;
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
	public void MakeSelected()
	{
		Unit u = GameManager.instance.GetSelectedUnit();
		if(u != null)
		{
			u.isSelected = false;
		}
		this.isSelected = true;
	}

}
