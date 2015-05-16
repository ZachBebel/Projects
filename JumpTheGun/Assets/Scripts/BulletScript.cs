using UnityEngine;
using System.Collections;

public class BulletScript : Photon.MonoBehaviour {
	
	float lifespan = 2.0f;
	public float speed = 30.0f;
	int damage = 10;

	public GameObject myPlayer;
	public int bulletColor;
	
	// Use this for initialization
	void Start () {
		if(photonView.isMine) 
		{ 
			myPlayer.GetComponent<PlayerController>().mostRecentBullet = this.gameObject;
		}
	}
	
	// Update is called once per frame
	void Update () {
		lifespan -= Time.deltaTime;
		
		if(lifespan <= 0) {
			Explode();
		}
	}

	[RPC]
	public void SetColor(int colorNumber) {
		bulletColor = colorNumber;
		renderer.material = AssetManager.colors[colorNumber];
	}

	[RPC]
	public void Fire(Vector3 position) 
	{
		rigidbody.AddForce(position, ForceMode.Impulse);
	}
	
	void OnCollisionEnter(Collision collision) {

		// Make sure the bullet doesn't ever collide with its shooter
		if (collision.gameObject == myPlayer)
		{
			collider.isTrigger = true;
		}
		else
		{
			if(collision.gameObject.tag == "Player")
			{
				collision.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, damage);
			}
			Explode();
		}
	}

	void OnCollisionExit(Collision collision) {

		// Allow the bullet to collide with other gameObjects once more after having collided with the shooter
		if (collision.gameObject == myPlayer)
		{
			collider.isTrigger = false;
		}
	}

	void Explode() {

		// Have shooter delete its bullet
		// "PhotonNetwork.Destroy" should only be called once across all clients
		if(myPlayer != null)
		{
			PhotonNetwork.Destroy(gameObject);
			PhotonNetwork.Instantiate("Sparks", transform.position, Quaternion.identity, 0);
		}
	}
}
