using UnityEngine;
using System.Collections;

public class CameraPan : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(new Vector3(0, 0, 0));
        transform.Translate(Vector3.right * Time.deltaTime * 10);
    }
}