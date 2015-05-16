using UnityEngine;
using System.Collections;

public class SetCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.gameObject.transform.FindChild ("Main Camera").gameObject.SetActive (false);
	}
}
