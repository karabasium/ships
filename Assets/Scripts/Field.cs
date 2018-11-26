using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour {
	public Vector3 offset;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/*void OnMouseDown()
	{
		Debug.Log("mouse down");
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));
	}*/

	//void OnMouseDrag()
	//{
		//Debug.Log("mouse drag");
	//	Vector3 newPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);		
		//if (newPosition.x)
		//{
		//	transform.position = Camera.main.ScreenToWorldPoint(newPosition) + offset;
		//}
	//}
}
