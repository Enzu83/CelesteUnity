using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightUpdate : MonoBehaviour
{
    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        float offset = GetComponentInParent<InitializeRail>().trafficLightOffset;

        transform.position = new Vector2(transform.position.x, transform.position.y + offset);
    }

    void FixedUpdate()
    {
        int parentState = GetComponentInParent<StateUpdate>().state;
        anim.SetInteger("state", parentState);
    }
}
