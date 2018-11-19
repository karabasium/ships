using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
		int len2 = System.Math.Abs(  (dirs.Count) - len1);
		//Debug.Log("len1 = " + len1.ToString());
		//Debug.Log("len2 = " + len2.ToString());
		return System.Math.Min(len1, len2);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetWeather()
	{
		//currentWeather = (weather_type)Random.Range(0, System.Enum.GetValues(typeof(weather_type)).Length);
		currentWeather = weather_type.WIND;
		Debug.Log("WEATHER: " + currentWeather.ToString());
		if (currentWeather == weather_type.WIND || currentWeather == weather_type.STORM)
		{
			curWindIndex = Random.Range(0, dirs.Count - 1);
			//curWindIndex = 6;
			curWind = dirs[curWindIndex];
			Debug.Log("curWindIndex: " + curWindIndex.ToString());
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
		

		if (currentWeather == weather_type.WIND)
		{
			
			SetCompassArrowDirection();
		}
		else if (currentWeather == weather_type.STORM)
		{
			//w_text.text = "Storm";
		}
		else if (currentWeather == weather_type.CALM)
		{
			//w_text.text = "Calm";
		}
		else
		{
			//w_text.text = "Armageddon";
		}
	}

	private void SetCompassArrowDirection()
	{
		GameObject arrow = GameObject.Find("CompassArrow");
		arrow.transform.rotation = Quaternion.identity;
		Text w_text = GameObject.Find("WeatherText").GetComponent<Text>();
		if (curWindIndex == 0)	{
			arrow.transform.Rotate(Vector3.back * 135);
			w_text.text = "NW Breeze";
		}
		else if (curWindIndex == 1) { 
			arrow.transform.Rotate(Vector3.back * 90);
			w_text.text = "N Breeze";
		}
		else if (curWindIndex == 2) { 
			arrow.transform.Rotate(Vector3.back * 45);
			w_text.text = "NE Breeze";
		}
		else if (curWindIndex == 3) { 
			arrow.transform.Rotate(Vector3.back * 0);
			w_text.text = "E Breeze";
		}
		else if (curWindIndex == 4) { 
			arrow.transform.Rotate(Vector3.forward * 45);
			w_text.text = "SE Breeze";
		}
		else if (curWindIndex == 5) { 
			arrow.transform.Rotate(Vector3.forward * 90);
			w_text.text = "S Breeze";
		}
		else if (curWindIndex == 6) { 
			arrow.transform.Rotate(Vector3.forward * 135);
			w_text.text = "SW Breeze";
		}
		else if (curWindIndex == 7) { 
			arrow.transform.Rotate(Vector3.back * -180);
			w_text.text = "W Breeze";
		}
	}
}
