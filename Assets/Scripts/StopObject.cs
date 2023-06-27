using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopObject : MonoBehaviour
{
    private Rigidbody2D rb;
    public bool stopped = false;
    private int frameDuration;
    private int timer = 0;

    private Vector2 storedVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (stopped)
        {
            if (timer < frameDuration)
            {
                timer++;
            }
            else
            {
                timer = 0;
                stopped = false;
                rb.velocity = storedVelocity;
                GetComponent<PlayerMovement>().dashLeft = GetComponent<PlayerMovement>().dashNumber;
            }
        }
    }
    public void Stop(float timeDuration)
    {
        frameDuration = (int)(timeDuration / Time.deltaTime);

        stopped = true;
        timer = 0;

        storedVelocity = rb.velocity;
        rb.velocity = Vector2.zero;
    }
}
