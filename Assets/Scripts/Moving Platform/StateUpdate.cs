using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateUpdate : MonoBehaviour
{
    public int state = 0;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private Vector2 direction;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float boostFactor;
    [SerializeField] private GameObject endPoint;

    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask playerMask;
    private BoxCollider2D coll;

    private int startingTimer;
    public int waitEndTimer;
    public bool playerJumped = false;

    void Start()
    {
        startPosition = transform.position;
        endPosition = endPoint.transform.position;
        direction = (endPosition - startPosition) / Vector2.Distance(startPosition, endPosition);

        coll = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        //Update player's parent
        StickPlayerToPlatform();

        if (state == 1)
        {
            if (startingTimer > 0) //Wait some time before movement
            {
                startingTimer--;
            }
            else
            {
                //Go toward end position
                transform.position = Vector2.MoveTowards(transform.position, endPosition, moveSpeed * Time.deltaTime);

                if (Vector2.Distance(transform.position, endPosition) < .1f)
                {
                    state = 2; //Return to start position
                    waitEndTimer = 15;
                    transform.position = endPosition;
                }
            }
        }
        else if (state == 2)
        {
            if (waitEndTimer > 0)
            {
                waitEndTimer--;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, startPosition, moveSpeed / 3 * Time.deltaTime);

                if (Vector2.Distance(transform.position, startPosition) < .1f)
                {
                    state = 0; //Return to start position
                    transform.position = startPosition;
                }
            }
        }
    }

    private void StickPlayerToPlatform()
    {
        Vector2 dirBoxCheck;
        bool wallGrabbed = player.GetComponent<PlayerMovement>().wallGrabbed;
        bool facingLeft = player.GetComponent<PlayerMovement>().facingLeft;

        if (facingLeft)
        {
            dirBoxCheck = Vector2.right;
        }
        else
        {
            dirBoxCheck = Vector2.left;
        }
        //Check grab
        if (wallGrabbed && Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, dirBoxCheck, .0625f, playerMask))
        {
            player.transform.SetParent(transform);
            if (state == 0)
            {
                state = 1;
                startingTimer = 5;
            }
        }
        //Check landing
        else if (Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.up, .0625f, playerMask) && player.GetComponent<PlayerMovement>().IsGrounded() && !wallGrabbed)
        {
            player.transform.SetParent(transform);
            if (state == 0)
            {
                state = 1;
                startingTimer = 5;
            }
        }
        else
        {
            player.transform.SetParent(null);
            playerJumped = false;
        }
        if (playerJumped) //Player isn't on the platform anymore
        {
            if (EjectPlayer())
            {
                Debug.Log("ejected");
                //Velocity boost

                Rigidbody2D rbPlayer = player.GetComponent<Rigidbody2D>();
                PlayerMovement playerMove = player.GetComponent<PlayerMovement>();

                if (playerMove.wallGrabbed)
                {
                    playerMove.wallGrabbed = false; //Jumping stops the player from grabbing the wall
                    playerMove.isGrabbing = false; //Jumping ends grab
                    playerMove.grabCooldownAfterJumpingFromWall = 10; //Time before a wall can be grabbed

                    rbPlayer.velocity = new Vector2(0f, rbPlayer.velocity.y); //No horizontal speed when grabbing (before any boost)
                }

                playerMove.SetBoost(10, boostFactor * moveSpeed * direction + 5f * Vector2.up + new Vector2(rbPlayer.velocity.x, 0f), true);
                //player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y) + 0.0625f * direction;

                if (rbPlayer.velocity.x > 0) //Update facing after boost
                {
                    playerMove.facingLeft = false;
                }
                else if (rbPlayer.velocity.x < 0)
                {
                    playerMove.facingLeft = true;
                }
                player.transform.SetParent(null);
                playerJumped = false;
            }
        }
    }

    public bool EjectPlayer()
    {
        return (Vector2.Distance(startPosition, transform.position) > .25f) && (state == 1 || (state == 2 && waitEndTimer > 10));
    }
}
