using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightUpdate : MonoBehaviour
{
    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();

        //Position offset
        float parentSizeY = GetComponentInParent<SpriteRenderer>().size.y;
        float offset = -parentSizeY / 2 + 2 * 0.0625f + GetComponent<SpriteRenderer>().size.y / 2;

        transform.position = new Vector2(transform.position.x, transform.position.y + offset);
    }

    void FixedUpdate()
    {
        int parentState = GetComponentInParent<StateUpdate>().state;
        anim.SetInteger("state", parentState);
    }
}
