using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather : MonoBehaviour {
	public int[][] directions = new int[][] { new int[] { -1, 1  }, new int[] { 0, 1 },   new int[] { 1, 1  },
											new int[] { -1, 0  },                       new int[] { 1, 0  },
											new int[] { -1, -1 }, new int[] { 0, -1 }, new int[]  { 1,-1 }};

	private List<int[]> dirs = new List<int[]>();

	public int[] curWind;
	private int curWindIndex;
	public enum weather_type
	{
		WIND,
		STORM,
		CALM
	}
	public weather_type currentWeather;
	// Use this for initialization
	void Start () {

	}

	void Awake()
	{
		dirs.Add(new int[] { -1, 1 });
		dirs.Add(new int[] { 0, 1 });
		dirs.Add(new int[] { 1, 1 });
		dirs.Add(new int[] { 1, 0 });
		dirs.Add(new int[] { 1, -1 });
		dirs.Add(new int[] { 0, -1 });
		dirs.Add(new int[] { -1, -1 });
		dirs.Add(new int[] { -1, 0 });
		//SetWeather();
	}

	public int DistanceToCurrentWind( int dirX, int dirY)
	{
		int dirIndex = -100;
		for (int i=0; i < dirs.Count; i++)
		{
			if (dirs[i][0] == System.Math.Sign(dirX) && dirs[i][1] == System.Math.Sign(dirY))
			{
				dirIndex = i;
				break;
			}
		}
		int len1 = System.Math.Abs(curWindIndex - dirIndex);
		int len2 = System.Math.Abs( -dirIndex + (dirs.Count) + curWindIndex);
		return System.Math.Min(len1, len2);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetWeather()
	{
		currentWeather = (weather_type)Random.Range(0, System.Enum.GetValues(typeof(weather_type)).Length);
		Debug.Log("WEATHER: " + currentWeather.ToString());
		if (currentWeather == weather_type.WIND || currentWeather == weather_type.STORM)
		{
			curWindIndex = Random.Range(0, dirs.Count - 1);
			curWind = dirs[curWindIndex];
			if (currentWeather == weather_type.STORM)
			{
				GameManager.instance.StormMovesShips();
			}
		}
		else
		{
			if (currentWeather == weather_type.CALM)
			{
				curWindIndex = -1;
				curWind = null;
			}
		}
	}
}
