using UnityEngine;
using System.Collections;

public class TimedDestroy : Photon.MonoBehaviour
{
    public float lifespan;

    void Update()
    {
        lifespan -= Time.deltaTime;

        if (lifespan <= 0 && photonView.isMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }

    }
}
