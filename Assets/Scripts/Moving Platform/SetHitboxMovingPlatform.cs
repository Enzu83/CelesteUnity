using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHitboxMovingPlatform : MonoBehaviour
{
    private void Start()
    {
        GetComponent<BoxCollider2D>().size = GetComponent<SpriteRenderer>().bounds.size;
    }
}
