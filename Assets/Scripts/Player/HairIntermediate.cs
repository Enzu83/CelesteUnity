using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairIntermediate : MonoBehaviour
{
    [SerializeField] private GameObject hairPart;
    [SerializeField] private GameObject hairPartFollowed;

    void Update()
    {
        this.transform.position = hairPartFollowed.transform.position + (hairPart.transform.position - hairPartFollowed.transform.position) / 2;
    }
}
