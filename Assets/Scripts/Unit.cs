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
		GameObject flyOffTextObj = Resources.Load("Prefabs/FlyOffText") as GameObject;
		Vector3 unitPos = gameObject.transform.position;
		Instantiate(flyOffTextObj, unitPos, Quaternion.identity);

		float rnd = Random.Range(0.0f, 1.0f);
		Debug.Log(rnd);
		if (rnd <= GameManager.instance.HitProbability)
		{
			flyOffTextObj.GetComponent<TextMesh>().text = "Hit!";
			hp -= dmg;
			Debug.Log(rnd);
			if (hp <= 0)
			{
				GameManager.instance.ships.Remove(gameObject);
				Destroy(gameObject);
				Debug.Log("unit destroyed");
				Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
			}
		}
		else
		{
			flyOffTextObj.GetComponent<TextMesh>().text = "miss!";
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
