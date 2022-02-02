using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test2 : MonoBehaviour
{
    private void OnTriggerEnter(Collider cor)
    {
        Debug.Log("trigger");
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("colision drone");
    }
}
