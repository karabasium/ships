using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyOffText : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Destroy(gameObject, this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
	}

	void Awake()
	{
		gameObject.GetComponent<MeshRenderer>().sortingLayerName = "ui";
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
