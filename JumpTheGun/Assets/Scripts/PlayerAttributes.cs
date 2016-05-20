using UnityEngine;
using System.Collections;

public class PlayerAttributes : Photon.MonoBehaviour
{

    // Health Bar Variables
    public float health;
    float maxHealth;
    int counter;
    float startingScale;
    public GameObject guiHealthBar;

    // HUD GameObject
    public GameObject guiHUD;

    // GameObject Attributes
    public int playerColor;

    // Network Access
    NetworkManager networkManager;


    void Start()
    {
        if (guiHealthBar != null) startingScale = guiHealthBar.transform.localScale.x;
        health = 100.0f;
        maxHealth = 100.0f;
        networkManager = GameObject.Find("_Scripts").GetComponent<NetworkManager>();
    }

    [PunRPC]
    public void SetColor(int colorNumber)
    {
        playerColor = colorNumber;
        GetComponent<Renderer>().material = AssetManager.colors[colorNumber];
        guiHUD.GetComponent<GUITexture>().texture = AssetManager.HUDColors[colorNumber];

    }

    [PunRPC]
    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log(health);
        if (health <= 0)
        {
            Die();
        }

        if (photonView.isMine)
        {
            AdjustHealthBar();
        }
    }

    void AdjustHealthBar()
    {

        //
        // Health Bar
        //

        float healthPercent = health / maxHealth;
        float changeScale = startingScale * healthPercent;

        guiHealthBar.transform.localScale = new Vector3(changeScale, guiHealthBar.transform.localScale.y, guiHealthBar.transform.localScale.z);

        if (healthPercent < 0.5f)
        {
            guiHealthBar.GetComponent<GUITexture>().texture = AssetManager.redHealthMat;
        }
        else
        {
            guiHealthBar.GetComponent<GUITexture>().texture = AssetManager.greenHealthMat;
        }

    }

    public void Die()
    {
        if (photonView.isMine)
        {
            Destroy(guiHealthBar);
            Destroy(guiHUD);

            PhotonNetwork.Instantiate("Death Explosion", transform.position, Quaternion.identity, 0);
            PhotonNetwork.Destroy(gameObject);

            // If we're still playing, respawn the player
            if (networkManager.gameState == GameState.Game || networkManager.gameState == GameState.PauseMenu)
            {
                networkManager.SpawnMyPlayer();
            }
        }
    }
}
