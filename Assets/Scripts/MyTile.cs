using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTile : MonoBehaviour {
	public GameObject ship;
	public GameObject tileObj;
	public string st;

	// Use this for initialization
	void Start()
	{
		//tileObj = Resources.Load("Prefabs/Tile") as GameObject;
		//Instantiate(tileObj, new Vector3(0, 0, 0), Quaternion.identity);
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void AddShipToTile(GameObject s)
	{
		//ship = s;
		Debug.Log("ship added");
	}
}
