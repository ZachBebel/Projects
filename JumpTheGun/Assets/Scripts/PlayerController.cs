using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterController))]
public class PlayerController : Photon.MonoBehaviour {
	
	// Physics Variables
	public float movementSpeed = 5.0f;
	public float mouseSensitivity = 5.0f;
	public float jumpSpeed = 20.0f;
	float verticalVelocity = 0.0f;
	float forwardSpeed;
	float sideSpeed;
	Vector3 speed;
	Quaternion currentRotation;
	
	float verticalRotation = 0.0f;
	public float upDownRange = 60.0f;
	
	// Player GameObject Variables
	CharacterController characterController;
	Camera playerCam;
	public GameObject mostRecentBullet;
	NetworkManager networkManager;

	//Debug Bools
	bool paused;
	
	void Start () {
		Screen.lockCursor = true;
		characterController = GetComponent<CharacterController>();
		playerCam = this.transform.FindChild ("Main Camera").camera;
		this.gameObject.tag = "Player";
		networkManager = GameObject.Find("_Scripts").GetComponent<NetworkManager>();
	}
	
	//
	// *** Check "EDIT" --> "Project Settings" --> "Input" in Unity for player input settings
	//
	
	/*
	
	Controls:
	
	Movement:
	W - Move Forward
	A - Move Left
	S - Move Backward
	D - Move Right
	SPACE - Jump
	
	Camera:
	Mouse - Look and Rotate
	
	Actions:
	Mouse Left-Click - Fire Bullet
	Mouse Right-Click - Teleport to most recently fired Bullet, Click and hold to follow Bullet

	GUI:
	P - Pause
	ESC - Lock/Unlock Cursor
	
	*/

	// Update is called once per frame
	void Update () {

		if(networkManager.gameState == GameState.Game)
		{
			Look ();
			Move ();
			Shoot ();
			Teleport ();
		}

		Paused();
		OutOfBounds();
	}

	void Look () 
	{

		//
		// Player Rotation and Camera Movement	
		//

		float rotLeftRight = Input.GetAxis("Mouse X") * mouseSensitivity;
		transform.Rotate(0, rotLeftRight, 0);
		
		verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
		verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
		playerCam.gameObject.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
	}

	void Move () 
	{

		//
		// Movement
		//

		if( characterController.isGrounded ) {
			// Jumping
			if ( Input.GetButton("Jump") ) { verticalVelocity = jumpSpeed; }
			
			// Can only control cardinal movement while on the Ground
			forwardSpeed = Input.GetAxis("Vertical") * movementSpeed;
			sideSpeed = Input.GetAxis("Horizontal") * movementSpeed;
			speed = new Vector3( sideSpeed, verticalVelocity, forwardSpeed );
			speed = transform.rotation * speed;
			currentRotation = transform.rotation;
		}
		else {
			// Can only fall and change camera viewpoint while in the Air
			verticalVelocity += Physics.gravity.y * Time.deltaTime;
			speed = new Vector3( sideSpeed, verticalVelocity, forwardSpeed );
			speed = currentRotation * speed;
		}
		
		characterController.Move( speed * Time.deltaTime );
	}

	void Shoot () 
	{

		//
		// Shooting
		//

		if( Input.GetButtonDown("Fire1") ) {
			GameObject thebullet = (GameObject)PhotonNetwork.Instantiate("Bullet", playerCam.transform.position + playerCam.transform.forward, playerCam.transform.rotation, 0);
			thebullet.GetComponent<PhotonView>().RPC("Fire", PhotonTargets.All, playerCam.transform.forward * thebullet.GetComponent<BulletScript>().speed);
			thebullet.GetComponent<BulletScript>().myPlayer = this.gameObject;
			thebullet.GetComponent<PhotonView>().RPC("SetColor", PhotonTargets.AllBuffered, GetComponent<PlayerAttributes>().playerColor);
		}
	}
	
	void Teleport () 
	{

		//
		// Teleportation to Bullet
		//

		if(mostRecentBullet != null) 
		{
			if(Input.GetMouseButtonDown(1))	// Right-Click, Single Press
			{
				// Teleport Partcile Effect
				PhotonNetwork.Instantiate("Teleport Particles", transform.position, Quaternion.identity, 0);
			}
			if (Input.GetMouseButton(1))	// Right-Click, Single Press or Held-Down
			{
				// Set new position
				transform.position = mostRecentBullet.transform.position;
				
				// Reset falling velocity
				verticalVelocity = 0;
			}
		}
	}

	void Paused () 
	{

		//
		// Pausing
		//

		if (Input.GetKeyDown (KeyCode.P))
		{
			// Switch Between Pause Menu and Game States
			if(networkManager.gameState == GameState.Game)
			{
				networkManager.gameState = GameState.PauseMenu;
				Screen.lockCursor = false;
			}
			else if(networkManager.gameState == GameState.PauseMenu)
			{
				networkManager.gameState = GameState.Game;
				Screen.lockCursor = true;
			}
		}

		// Lock or Unlock the Cursor via ESC or L
		if (Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown (KeyCode.L)) 
		{
			Screen.lockCursor = !Screen.lockCursor; 
		}
	}

	void OutOfBounds()
	{
		 
		//
		// Falling Death
		//

		if(transform.position.y < -50.0f){
			GetComponent<PlayerAttributes>().Die();
		}
	}


	
}




