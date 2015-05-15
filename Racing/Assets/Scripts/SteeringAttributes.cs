using UnityEngine;
using System.Collections;

public class SteeringAttributes : MonoBehaviour {
	
	//these are common attributes required for steering calculations 
	public float maxForce = 12.0f;
	public float mass = 1.0f;
	public float radius = 6.0f;
	public float pathRadius = 5.0f;
	
	public float seekWt = 30.0f;
	public float inBoundsWt = 30.0f;
	public float avoidDist = 6.0f;

	public float separateWt = 50.0f;
	//public float alignWt = 10.0f;
	//public float cohereWt = 5.0f;
}
