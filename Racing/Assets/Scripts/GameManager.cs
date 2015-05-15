using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	private GameObject myGuy;
	private GameObject target;
	public List<GameObject> racecars;
	public List<GameObject> path;
	private int numNodes;
	private int index;
	
	//these variable are visible in the Inspector
	public GameObject centroid;
	public GameObject pathView;
	public GameObject TargetPrefab;
	public GameObject GuyPrefab;
	
	// Use this for initialization
	void Start() {
		// the plane is 50x50 (with 0,0,0 in center), so make stuff within 40x40
		centroid = GameObject.Find("Centroid");
		pathView = GameObject.Find("Path");
		
		numNodes = 35;
		
		path = new List<GameObject>();
		for(int i = 1; i <= numNodes; i++)
		{
			path.Add(GameObject.Find("Waypoint " + i));
		}
		
		//Vector3 pos;
		//Vector3 start;

		
		//target = (GameObject)GameObject.Instantiate(TargetPrefab, pos, Quaternion.identity);
		target = path[0];
		index = 0;
		
		racecars = new List<GameObject>();
		
		for (int i = 1; i <= 16; i++)
		{
			//start = path[0].transform.position;
			//pos = new Vector3(start.x + Random.Range(-20, 20), 4f, start.z + Random.Range(-20, 20));
			//racecars.Add((GameObject)GameObject.Instantiate(GuyPrefab, pos, Quaternion.identity));
			racecars.Add(GameObject.Find("Aston " + i));
			racecars[i-1].GetComponent<SteeringVehicle>().Target = target.gameObject;
			racecars[i-1].GetComponent<SteeringVehicle>().MaxSpeed = Random.Range(23, 25);
			racecars[i-1].GetComponent<SteeringVehicle>().Path = path;
		}
		
		for (int i = 1; i <= 16; i++)
		{
			racecars[i-1].GetComponent<SteeringVehicle>().Racecars = racecars;
		}
		
		//tell camera to follow a random traveler
		Camera.main.GetComponent<SmoothFollow>().target = racecars[(int)Random.Range(0, racecars.Count-1)].transform;
		
	}
	
	void calcCentriod() {
		// Calculate the center position of the racecars
		Vector3 sum = Vector3.zero;
		foreach (GameObject traveler in racecars) {
    	    sum += traveler.transform.position;
    	}
		sum /= racecars.Count;
		
		// Set the position of centriod GameObject to that position
		centroid.transform.position = sum;
	}
	
	void calcAverageForward() {
		// Calculate the average forward vector
		Vector3 sum = Vector3.zero;
		foreach (GameObject traveler in racecars) {
    	    sum += traveler.transform.forward;
    	}
		sum /= racecars.Count;
		
		// Set the rotation of centriod GameObject to that value
		centroid.transform.forward = sum;
	}
	
	// Update is called once per frame
	void Update () {
		calcCentriod();
		calcAverageForward();
		//SeekNearestWaypoint();
		int random = 0;
		
		if (Input.GetKeyDown (KeyCode.Space))
		{
			//tell camera to follow a random traveler
			random = (int)Random.Range(0, racecars.Count);
			Camera.main.GetComponent<SmoothFollow>().target = racecars[random].transform;

		}
		if (Input.GetKeyDown (KeyCode.C))
		{
			//tell camera to follow a random traveler
			Camera.main.GetComponent<SmoothFollow>().target = pathView.transform;
			
			pathView.transform.Rotate(0, 90, 0, Space.Self);
			//pathView.transform.position = new Vector3((pathView.transform.position.x - 200)%400, pathView.transform.position.y, (pathView.transform.position.z - 200)%400);
		}
		Debug.Log(racecars[random].GetComponent<SteeringVehicle>().Timer);

	}
	
	// Debugger method to see if path is in order, basic path following
	void SeekNearestWaypoint ()
	{
		//GameObject nearestWaypoint = path[0];
		//index = 0;
		//
		//for(int i = 1; i < numNodes; i++)
		//{
		//	if(Vector3.Distance(centroid.transform.position, path[i].transform.position) < Vector3.Distance(centroid.transform.position, nearestWaypoint.transform.position))
		//	{
		//		nearestWaypoint = path[i];
		//		index = i;
		//	}
		//}
		if(Vector3.Distance(centroid.transform.position, target.transform.position) < 10) 
		{
			if(index >= numNodes - 1) index = -1;
			target = path[index+1];
			foreach (GameObject traveler in racecars)
			{
				traveler.GetComponent<SteeringVehicle>().Target = target.gameObject;
			}
			index++;
		}
	}
}
