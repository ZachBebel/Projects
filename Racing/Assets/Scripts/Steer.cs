// The Steer component has a collection of functions
// that return forces for steering 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]

public class Steer : MonoBehaviour
{
		Vector3 dv = Vector3.zero; 	// desired velocity, used in calculations
		SteeringAttributes attr; 	// attr holds several variables needed for steering calculations
		CharacterController characterController;
		GameObject cylinder;
	
		void Start ()
		{
				cylinder = GameObject.Find ("Cylinder");
				GameObject main = GameObject.Find ("MainGO");
				attr = main.GetComponent<SteeringAttributes> ();
				characterController = gameObject.GetComponent<CharacterController> ();	
		}
	
	
		//-------- functions that return steering forces -------------//
		public Vector3 Seek (Vector3 targetPos, float maxSpeed)
		{
				//find dv, desired velocity
				dv = targetPos - transform.position;		
				dv = dv.normalized * maxSpeed; 	//scale by maxSpeed
				dv -= characterController.velocity;
				dv.y = 0;								// only steer in the x/z plane
				return dv;
		}

		public Vector3 Separate (List<GameObject> neighbors, Vector3 velocity, float maxSpeed)
		{
				Vector3 steer = Vector3.zero;
				int count = 0;
				// For every boid in the system, check if it's too close
				foreach (GameObject neighbor in neighbors) {
						float d = Vector3.Distance (transform.position, neighbor.transform.position);
						// If the distance is greater than 0 and less than an arbitrary amount (0 when you are yourself)
						if ((d > 0) && (d < attr.avoidDist)) {
								// Calculate vector pointing away from neighbor
								Vector3 diff = transform.position - neighbor.transform.position;
								diff.Normalize ();
								diff /= d;        // Weight by distance
								steer += diff;
								count++;            // Keep track of how many
						}
				}
				// Average -- divide by how many
				if (count > 0) {
						steer /= (float)count;
				}
		
				// As long as the vector is greater than 0
				if (steer.magnitude > 0) {
						// Implement Reynolds: Steering = Desired - Velocity
						steer.Normalize ();
						steer *= maxSpeed;
						steer -= velocity;
						steer = Vector3.ClampMagnitude (steer, attr.maxForce);
				}
				return steer;
		}
	
		// Current strategy: the GameManager component calculates the direction 
		// once per frame so all flockers can make use of it 
		public Vector3 Align (List<GameObject> neighbors, Vector3 velocity, float maxSpeed)
		{		
				float visionDist = 40;
				Vector3 sum = Vector3.zero;
				int count = 0;
				foreach (GameObject neighbor in neighbors) {
						float d = Vector3.Distance (neighbor.transform.position, transform.position);
						if ((d > 0) && (d < visionDist)) {
								sum += neighbor.transform.forward;
								count++;
						}
				}
				if (count > 0) {
						sum /= (float)count;
						sum.Normalize ();
						sum *= maxSpeed;
						Vector3 steer = sum - velocity;
						steer = Vector3.ClampMagnitude (steer, attr.maxForce);
						return steer;
				} else {
						return Vector3.zero;
				}
		}
	
		// Current strategy is to have the centroid calculated
		// in the GameManager component once per frame, so that all flockers can make use of it
		public Vector3 Cohere (List<GameObject> neighbors, float maxSpeed)
		{
				float visionDist = 40;
				Vector3 sum = Vector3.zero;
				int count = 0;
				foreach (GameObject neighbor in neighbors) {
						float d = Vector3.Distance (neighbor.transform.position, transform.position);
						if ((d > 0) && (d < visionDist)) {
								sum += neighbor.transform.position;
								count++;
						}
				}
				if (count > 0) {
						sum /= (float)count;
						return Seek (sum, maxSpeed);
				} else {
						return Vector3.zero;
				}
		}
	
		public Vector3 FollowPath (List<GameObject> path, Vector3 velocity, float maxSpeed, SteeringVehicle vehicle, int index)
		{
				// Predict location 50 (arbitrary choice) frames ahead
				// This could be based on speed 
				Vector3 predict = velocity;
				predict.Normalize ();
				predict *= 10;
				Vector3 predictLoc = transform.position + predict;
		
				// Now we must find the normal to the path from the predicted location
				// We look at the normal for each line segment and pick out the closest one
		
				Vector3 normal = Vector3.zero;
				Vector3 target = Vector3.zero;
				float worldRecord = 1000000;  
				// Start with a very high record distance that can easily be beaten
		
				// Loop through all points of the path
				for (int i = 0; i < path.Count; i++) {
		
						// Look at a line segment
						Vector3 a = path [i % path.Count].transform.position;
						Vector3 b = path [(i + 1) % path.Count].transform.position;
		
						// Get the normal point to that line
						Vector3 normalPoint = GetNormalPoint (predictLoc, a, b);
						// This only works because we know our path goes from left to right
						// We could have a more sophisticated test to tell if the point is in the line segment or not
				
						Vector3 dir = b - a;
				
						if (normalPoint.x < Mathf.Min (a.x, b.x) || normalPoint.x > Mathf.Max (a.x, b.x) ||
								normalPoint.y < Mathf.Min (a.y, b.y) || normalPoint.y > Mathf.Max (a.y, b.y)) {
								// This is something of a hacky solution, but if it's not within the line segment 
								// consider the normal to just be the end of the line segment (point b)
								normalPoint = b;
					
								a = path [(i + 1) % path.Count].transform.position;
								b = path [(i + 2) % path.Count].transform.position;
								dir = b - a;
						}
			
		
						// How far away are we from the path?
						float distance = Vector3.Distance (predictLoc, normalPoint);
						// Did we beat the record and find the closest line segment?
						if (distance < worldRecord) {
								worldRecord = distance;
								// If so the target we want to steer towards is the normal
								normal = normalPoint;
				
								//Look at the direction of the line segment so we can seek a little bit ahead of the normal
								dir.Normalize ();
								// This is an oversimplification
								// Should be based on distance to path & velocity
								dir *= 10;
								target = normal;
								target += dir;
				
								//vehicle.Index = (i)%path.Count;
						}
				
				}
				// Only if the distance is greater than the path's radius do we bother to steer
				if (worldRecord > attr.pathRadius) {
						//cylinder.transform.position = target;
						return Seek (target, maxSpeed);
			
				} else {
						return Vector3.zero;
				}
		
				//vehicle.Index = (index + 1)%path.Count;

		}


		// A function to get the normal point from a point (p) to a line segment (a-b)
		// This function could be optimized to make fewer new Vector objects
		Vector3 GetNormalPoint (Vector3 p, Vector3 a, Vector3 b)
		{
				// Vector from a to p
				Vector3 ap = p - a;
				// Vector from a to b
				Vector3 ab = b - a;
				ab.Normalize (); // Normalize the line
				// Project vector "diff" onto line by using the dot product
				ab *= Vector3.Dot (ap, ab);
				Vector3 normalPoint = a + ab;
				return normalPoint;
		}
	
	
	
		// tether type containment - not very good!
		public Vector3 StayInBounds (float radius, Vector3 center, float maxSpeed)
		{
				if (Vector3.Distance (transform.position, center) > radius)
						return Seek (center, maxSpeed);
				else
						return Vector3.zero;
		}
	
		public Vector3 Overtake (List<GameObject> racecars, List<GameObject> path, SteeringVehicle vehicle)
		{
				Vector3 predict = Vector3.zero; 
				predict = transform.forward;
				predict.Normalize ();
				predict *= 10;
				predict += transform.position;
				Debug.DrawLine (transform.position, predict, Color.red);
				vehicle.OvertakeTarget = path [0];
				foreach (GameObject car in racecars) {
						if (Vector3.Distance (predict, car.transform.position) < 5 && transform.position != car.transform.position &&
								Vector3.Distance (transform.position, car.transform.position) < Vector3.Distance (transform.position, vehicle.OvertakeTarget.transform.position)) {
								vehicle.OvertakeTarget = car;
				Debug.DrawLine (predict, car.transform.position, Color.white);
				Debug.DrawLine (transform.position, car.transform.position, Color.yellow);
						}
						
				}

				if (vehicle.Timer >= 5) {
						vehicle.Timer += Time.deltaTime;
						if (vehicle.OvertakeTarget == path [0])
								vehicle.OvertakeTarget = null;

						if (vehicle.Timer < 9 && vehicle.OvertakeTarget != null && vehicle.StopSeek == false) {
								if (vehicle.MaxSpeed < 26)
										vehicle.MaxSpeed += 3;
								Vector3 overtakePos = vehicle.OvertakeTarget.transform.right;
								overtakePos.Normalize ();
								overtakePos *= 10;
								overtakePos += vehicle.OvertakeTarget.transform.position;
								if (Vector3.Distance (transform.position, overtakePos) < 3) {
										vehicle.StopSeek = true;
								} else {
										vehicle.PreviousOvertakeTarget = vehicle.OvertakeTarget;					
										return Seek (overtakePos, vehicle.MaxSpeed);
								}
						} else if (vehicle.Timer >= 9) {
								vehicle.Timer = 0;
								vehicle.MaxSpeed -= 3;
								vehicle.StopSeek = false;
								//vehicle.PreviousOvertakeTarget = vehicle.gameObject;
						}


				} else if (vehicle.Timer < 5) {
						if (vehicle.OvertakeTarget == vehicle.PreviousOvertakeTarget && vehicle.OvertakeTarget != null) {
								vehicle.Timer += Time.deltaTime;
						} else if (vehicle.OvertakeTarget != vehicle.PreviousOvertakeTarget || vehicle.OvertakeTarget == null) {
								vehicle.Timer = 0;
						}
				
						


				
						//Debug.Log (vehicle + " " + vehicle.OvertakeTarget);
						//Debug.Log(vehicle + " " + transform.position + " " + predict);
			
						if (vehicle.OvertakeTarget == path [0])
								vehicle.OvertakeTarget = null;
				
						if (vehicle.OvertakeTarget != null) {			
								//Debug.DrawRay (transform.position, predict, Color.red);
								Vector3 overtakePos = vehicle.OvertakeTarget.transform.forward;
								overtakePos.Normalize ();
								overtakePos *= -7;
								overtakePos += vehicle.OvertakeTarget.transform.position;
								vehicle.PreviousOvertakeTarget = vehicle.OvertakeTarget;
								return Seek (overtakePos, vehicle.MaxSpeed);
						}

				}
				vehicle.PreviousOvertakeTarget = vehicle.OvertakeTarget;
				return Vector3.zero;
		}
}
