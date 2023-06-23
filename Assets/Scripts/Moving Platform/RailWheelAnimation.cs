using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailWheelAnimation : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 12f;

    void FixedUpdate()
    {
        int parentState = GetComponentInParent<StateUpdate>().state;
        //1st movement 
        if (parentState == 1)
        {
            transform.localEulerAngles += new Vector3(0f, 0f, rotateSpeed);
        }
        //2nd movement 
        else if (parentState == 2)
        {
            transform.localEulerAngles += new Vector3(0f, 0f, -rotateSpeed / 2);
        }
        else
        {
            transform.localEulerAngles = Vector3.zero;
        }
    }
}
