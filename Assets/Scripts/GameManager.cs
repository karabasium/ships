using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public GameObject tileObj;

	// Use this for initialization
	void Start () {
		
		tileObj = Resources.Load("Prefabs/Tile") as GameObject;
		Renderer r = tileObj.GetComponent<Renderer>();
		float width = r.bounds.size[0];
		float height = r.bounds.size[1];
		Vector2 topRightCorner = new Vector2(1, 1);

		Vector2 edgeVector = Camera.main.ViewportToWorldPoint( topRightCorner );
		float screenWidth = edgeVector.x * 2;
		float screenHeight = edgeVector.y * 2;
		float screenZeroX = - edgeVector.x;
		float screenZeroY = edgeVector.y;

		int fieldSizeX = (int) (screenWidth / width);
		int fieldSizeY = (int) (screenHeight / height);

		for (int x = 0; x < fieldSizeX; x++)
		{
			for (int y = 0; y < fieldSizeY; y++)
			{
				Instantiate(tileObj, new Vector3((screenZeroX + (x + 1) * width - width/2), (screenZeroY - (y+1)*height+height/2), 0), Quaternion.identity);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Awake()
	{
		MakeSingleton();
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
