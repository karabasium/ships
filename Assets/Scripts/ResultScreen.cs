using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultScreen : MonoBehaviour {
	private bool ready = false;

	// Use this for initialization
	void Start () {
		ready = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PlayAgain()
	{
		Debug.Log("Play again");
		GameManager.instance.Init();
	}

	public bool isReady()
	{
		return ready;
	}
}
