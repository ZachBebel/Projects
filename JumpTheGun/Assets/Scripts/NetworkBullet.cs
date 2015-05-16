using UnityEngine;
using System.Collections;

public class NetworkBullet : Photon.MonoBehaviour {
	
	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;

	float lerpTime = 0.0f;
	bool firstUpdate = true;
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(photonView.isMine == false)
		{
			lerpTime += Time.deltaTime;
			transform.position = Vector3.Lerp(transform.position, realPosition, lerpTime/1f);
			transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, lerpTime/1f);
		}
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.isWriting)
		{
			// OUR Player
			// Send actual position to network
			
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
		}
		else
		{
			// Other Player
			// Receive position as of a few milliseconds ago
			
			realPosition = (Vector3)stream.ReceiveNext();
			realRotation = (Quaternion)stream.ReceiveNext();

			if(firstUpdate)
			{
				transform.position = realPosition;
				transform.rotation = realRotation;
				firstUpdate = false;
			}
		}
	}
}
