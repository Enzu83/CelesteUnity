using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopObject : MonoBehaviour
{
    public bool stopped = false;
    private int frameDuration;
    private int timer = 0;

    private Vector2 storedVelocity;

    private bool upperTransition;
    private GameObject transitionCamera;

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

                if (upperTransition && transitionCamera.activeInHierarchy)
                {
                    storedVelocity = new Vector2(0f, 10f);
                    GetComponent<PlayerMovement>().ResetDashAndGrab();
                }

                GetComponent<Rigidbody2D>().velocity = storedVelocity;
                GetComponent<Rigidbody2D>().gravityScale = GetComponent<PlayerMovement>().gravityScale;
                GetComponent<PlayerMovement>().dashLeft = GetComponent<PlayerMovement>().dashNumber;
            }
        }
    }
    public void Stop(float timeDuration, bool up, GameObject camera)
    {
        frameDuration = (int)(timeDuration / Time.deltaTime);
        upperTransition = up;
        transitionCamera = camera;

        stopped = true;
        timer = 0;

        storedVelocity = GetComponent<Rigidbody2D>().velocity;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().gravityScale = 0f;
    }
}
