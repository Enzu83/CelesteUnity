using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableSpriteRenderer : MonoBehaviour
{
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
