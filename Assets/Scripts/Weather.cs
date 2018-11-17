using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather : MonoBehaviour {
	//private int[] l1 = new int[4] = {}; 
	private int[][] directions = new int [][] { new int[] { -1, 1  }, new int[] { 0, 1 },   new int[] { 1, 1  },
												new int[] { -1, 0  },                       new int[] { 1, 0  },
												new int[] { -1, -1 }, new int[] { 0, -1 }, new int[]  { 1,-1 }};
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
