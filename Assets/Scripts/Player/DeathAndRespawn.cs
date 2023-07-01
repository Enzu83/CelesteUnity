using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAndRespawn : MonoBehaviour
{
    private GameObject[] spawnPoints;
    [SerializeField] private GameObject Ball;
    private Rigidbody2D rb;
    public Vector2 respawnPosition = Vector2.zero;
    [SerializeField] private float deadSpeed = 5f;

    public bool dead = false;
    private int deathAnimationTimer = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //Find the initial nearest spawn point
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        respawnPosition = Nearest(spawnPoints);
    }
    private void FixedUpdate()
    {
        if (dead) //Death animation
        {
            rb.velocity *= 0.9f; //Slow down speed for fluidity

            //Death animation
            if (deathAnimationTimer == 6) //Create death balls
            {
                for (int i = 0; i < 8; i++)
                {
                    GameObject ball = Instantiate(Ball, this.transform.position, Quaternion.identity);
                    ball.GetComponent<BallAnimation>().angle = (float)i * Mathf.PI / 4;
                }
            }

            //Deactivate sprite and hair after ball animation end
            if (deathAnimationTimer > 15)
            {
                foreach (Transform transf in GetComponentsInChildren<Transform>())
                {
                    transf.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
            else
            {
                foreach (Transform transf in GetComponentsInChildren<Transform>())
                {
                    transf.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                }
            }


            //Respawn
            if (deathAnimationTimer == 30)
            {
                transform.position = respawnPosition; //Snap back player to spawn point
                transform.localScale = 1f * Vector3.one; //Back to normal size
                rb.velocity = Vector2.zero; //Stop velocity
                GetComponent<PlayerMovement>().dashLeft = GetComponent<PlayerMovement>().dashNumber; //Reset dash
                GetComponent<PlayerMovement>().staminaLeft = GetComponent<PlayerMovement>().maxStamina; //Reset stamina
                GetComponent<PlayerMovement>().ResetDashAndGrab();
                GetComponent<Animator>().SetBool("dead", false); //Death animation state
                GetComponent<BoxCollider2D>().enabled = true; //Reactive hitbox after respawn
                for (int i = 0; i < 8; i++) //Create ball but reverse
                {
                    GameObject ball = Instantiate(Ball, this.transform.position, Quaternion.identity);
                    ball.GetComponent<BallAnimation>().angle = (float)i * Mathf.PI / 4;
                    ball.GetComponent<BallAnimation>().reverse = true;
                }

                //Refresh Winged Strawberry if they flew off
                foreach (GameObject wingedStrawberry in GameObject.FindGameObjectsWithTag("Winged Strawberry"))
                {
                    wingedStrawberry.GetComponent<WingedStrawberry>().Refresh();
                }

                //Reset camera
                GetComponent<InitializeActiveCamera>().Start();
            }

            //Timer and end of death
            if (deathAnimationTimer < 50) //Timer before restarting
            {
                deathAnimationTimer++;
            }
            else //End of death
            {

                dead = false; //Reset death trigger
                deathAnimationTimer = 0; //Reset timer
                GetComponent<PlayerMovement>().ResetDashAndGrab(); //Stop dash and grab when respawning
                foreach (Transform transf in GetComponentsInChildren<Transform>()) //Reactivate Madeline and Hair rendering
                {
                    transf.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike")) //Checking for spikes
        {
            dead = true; //Death trigger
            rb.gravityScale = 0f; //Stop gravity

            //Speed direction when dying
            rb.velocity = deadSpeed * Vector2.one;
            if (collision.transform.position.y > transform.position.y) //Going to the bottom if spike is above
            {
                rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
            }
            if (!GetComponent<PlayerMovement>().facingLeft) //Ejection depends on facing
            {
                rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            }
            GetComponent<Collider2D>().enabled = false; //Deactivate collision
            GetComponent<Animator>().SetBool("dead", true); //Death animation state
        }
    }

    public Vector2 Nearest(GameObject[] gameObjectList) //Find the nearest from player
    {
        int index = 0;
        float minDist = Mathf.Infinity;

        for (int i = 0; i < gameObjectList.Length; i++) //Search in the list
        {
            float dist = Vector2.Distance(transform.position, gameObjectList[i].transform.position);

            if (dist < minDist)
            {
                index = i;
                minDist = dist;
            }
        }

        return gameObjectList[index].transform.position;
    }
}
