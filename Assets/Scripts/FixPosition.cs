using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixPosition : MonoBehaviour
{
    private Vector2 fixedPosition;
    void Start()
    {
        fixedPosition = transform.position;
    }

    void Update()
    {
        transform.position = fixedPosition;
    }
}
