using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
	public int side;
	public int hp = 3;
	public bool isSelected = false;

	// Use this for initialization
	void Start () {
		
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
