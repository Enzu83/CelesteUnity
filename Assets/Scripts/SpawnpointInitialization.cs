using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointInitialization : MonoBehaviour
{
    public GameObject screen;
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Screen"))
        {
            screen = coll.gameObject;

            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
