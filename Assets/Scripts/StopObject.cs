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

    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private DeathAndRespawn deathResp;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        deathResp = GetComponent<DeathAndRespawn>();
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

                if (upperTransition && transitionCamera.activeInHierarchy && deathResp.dead == false)
                {
                    storedVelocity = new Vector2(0f, 11f);
                    playerMovement.ResetDashAndGrab();
                }

                rb.velocity = storedVelocity;
                rb.gravityScale = playerMovement.gravityScale;
                playerMovement.dashLeft = playerMovement.dashNumber;


                ///Update spawnpoint
                List<GameObject> spawnpointsInCamera = new List<GameObject>();

                //Select spawnpoint only in camera
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Respawn"))
                {
                    if (VisibleInCamera(obj))
                    {
                        spawnpointsInCamera.Add(obj);
                    }
                }

                //Set new spawnpoint as the nearest spawnpoint
                deathResp.respawnPosition = deathResp.Nearest(spawnpointsInCamera.ToArray());


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

    private bool VisibleInCamera(GameObject gameObject)
    {
        if (GameObject.ReferenceEquals(gameObject.GetComponent<SpawnpointInitialization>().screen, transitionCamera.transform.parent.gameObject))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
