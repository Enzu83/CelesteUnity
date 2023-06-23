using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Inputs
    private enum KeyState { Off, Held, Up, Down }
    private KeyState tempKeyJump;
    private KeyState keyJump;
    private KeyState tempKeyDash;
    private KeyState keyDash;
    private KeyState tempKeyGrab;
    private KeyState keyGrab;
    public float dirX;
    public float dirY;

    //Misc. variables
    private Rigidbody2D rb;
    [HideInInspector] public BoxCollider2D coll;
    public LayerMask wall;
    public LayerMask spring;

    private Vector2 halfBottomHitboxCenter;
    private Vector2 halfBottomHitboxSize;

    public bool facingLeft = false;
    public bool isAirborne;
    private int groundedFrames = 0;
    private DeathAndRespawn deathResp;

    //Speed relatived variables
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float maxVerticalSpeed = 20f;
    [HideInInspector] public bool boostedVelocity = false;
    [HideInInspector] public int boostedTimer = 0;
    private bool keepVelocityAfterBoost = false;
    [HideInInspector] public float maxBoostedHorizontalSpeed;
    private float gravityScale;

    //Dash Variables
    [SerializeField] public int dashNumber = 1;
    [SerializeField] private int dashDuration = 8;
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private GameObject phantomMadeline;
    private Vector2 dashDirection = Vector2.zero;
    [HideInInspector] public int dashState = 0;
    [HideInInspector] public int dashLeft;
    [HideInInspector] public bool isDashing = false;

    //Grab Variables
    [HideInInspector] public bool isGrabbing = false;
    [HideInInspector] public bool wallGrabbed = false;
    public float maxStamina = 180f;
    public float staminaLeft;
    public float climbSpeed = 4f;
    [HideInInspector] public int grabCooldownAfterJumpingFromWall = 0;
    [HideInInspector] public bool slidingOnWall = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        deathResp = GetComponent<DeathAndRespawn>();

        gravityScale = rb.gravityScale;
        dashLeft = dashNumber;
        staminaLeft = maxStamina;

    }

    void Update()
    {
        //Getting inputs
        tempKeyJump = UpdateKeyState("Jump");
        tempKeyDash = UpdateKeyState("Dash");
        tempKeyGrab = UpdateKeyState("Grab");
    }

    void FixedUpdate()
    {
        ///Getting Inputs

        //Horizontal inputs
        dirX = Input.GetAxisRaw("Horizontal");
        dirY = Input.GetAxisRaw("Vertical");

        //Passing inputs from Update to FixedUpdate
        keyJump = FixedUpdateKeyState(tempKeyJump, keyJump);
        keyDash = FixedUpdateKeyState(tempKeyDash, keyDash);
        keyGrab = FixedUpdateKeyState(tempKeyGrab, keyGrab);

        //Update half hitbox
        halfBottomHitboxCenter = new Vector2(coll.bounds.center.x, coll.bounds.center.y - coll.bounds.size.y / 4);
        halfBottomHitboxSize = new Vector2(coll.bounds.size.x, coll.bounds.size.y / 2);

        if (!deathResp.dead) //Player can't act if dead
        {
            UpdateFacing();

            UpdateSliding();

            GrabCheck();

            DashCheck();

            UpdateGravity();

            UpdateVelocity();
        }

        isAirborne = !IsGrounded();

    }
    private void UpdateVelocity()
    {
        if (!isDashing && !wallGrabbed && !slidingOnWall) //Can't move freely if dashing or grabbing a wall
        {
            //Horizontal Movement

            if (!boostedVelocity) //Normal movement
            {
                if (IsGrounded()) //Horizontal movement on the ground or high speed movement
                {
                    rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
                }
                else if ((Mathf.Abs(rb.velocity.x) < moveSpeed && dirX != 0)) //Horizontal movement in the air
                {
                    float horizontalVelocity;
                    horizontalVelocity = rb.velocity.x + dirX * moveSpeed / 8;
                    if (Mathf.Abs(horizontalVelocity) > moveSpeed) //Cap horizontal speed
                    {
                        horizontalVelocity = dirX * moveSpeed;
                    }

                    rb.velocity = new Vector2(horizontalVelocity, rb.velocity.y); //Slight drift in the air
                }
                else //Maximum air speed
                {
                    rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
                }
            }
            else if (boostedVelocity) //Horizontal Movement when player is boosted (e.g. by moving platform)
            {
                if (IsGrounded())
                {
                    boostedVelocity = false; //Reset boost
                    rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
                    boostedTimer = 0;
                }
                else if (boostedTimer == 0 && dirX != 0) //Horizontal movement in the air
                {
                    float horizontalVelocity;
                    horizontalVelocity = rb.velocity.x + dirX * moveSpeed / 10;

                    if (Mathf.Abs(horizontalVelocity) > maxBoostedHorizontalSpeed) //Cap horizontal speed
                    {
                        horizontalVelocity = dirX * maxBoostedHorizontalSpeed;
                    }

                    rb.velocity = new Vector2(horizontalVelocity, rb.velocity.y); //Slight drift in the air

                    if (Mathf.Abs(rb.velocity.x) <= moveSpeed) //Return to normal movement
                    {
                        boostedVelocity = false; //Reset boost
                        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
                    }
                }

                if (boostedTimer > 0) //Timer before player can act
                {
                    boostedTimer--;
                }
                else
                {
                    boostedTimer = 0;
                    if (!keepVelocityAfterBoost)
                    {
                        boostedVelocity = false;
                    }
                }
            }
        }

        if (keyJump == KeyState.Down && IsGrounded() && !wallGrabbed && !slidingOnWall) //Jump detection
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            //Abort dash but horizontal speed is conserved
            isDashing = false;
            dashState = 0;
            dashLeft = dashNumber;
        }

        if (rb.velocity.y < -maxVerticalSpeed) //Limit vertical speed
        {
            rb.velocity = new Vector2(rb.velocity.x, -maxVerticalSpeed);
        }
    }
    private void UpdateSliding()
    {
        //Conditions : going toward a wall while being next to it
        Vector2 tempDir = Vector2.zero;
        if (facingLeft)
        {
            tempDir = Vector2.left;
        }
        else
        {
            tempDir = Vector2.right;
        }

        //Check if player can slide
        if (dirX == tempDir.x /*Same direction than input*/ && Physics2D.BoxCast(halfBottomHitboxCenter, halfBottomHitboxSize, 0f, tempDir, .0625f, wall) && !IsGrounded() && rb.velocity.y < 0f)
        {
            slidingOnWall = true;
        }
        else
        {
            slidingOnWall = false;
        }

        //Sliding movement
        if (slidingOnWall)
        {
            //Neutral jump
            if (keyJump == KeyState.Down)
            {
                Vector2 newSpeed;

                if (facingLeft)
                {
                    newSpeed = new Vector2(1.5f * moveSpeed, 0.95f * jumpForce);
                }
                else
                {
                    newSpeed = new Vector2(-1.5f * moveSpeed, 0.95f * jumpForce);
                }

                facingLeft = !facingLeft; //Invert facing

                slidingOnWall = false;

                //Apply speed
                SetBoost(10, newSpeed, false);
            }
            else
            {
                //Limit vertical speed
                if (rb.velocity.y < -maxVerticalSpeed / 2)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -maxVerticalSpeed / 2);
                }
            }
        }
    }
    private void GrabCheck()
    {
        //Start grab
        if (!isDashing) //Can't grab if dashing
        {
            if (keyGrab == KeyState.Down || keyGrab == KeyState.Held) //Conditions for attempting to grab
            {
                if (grabCooldownAfterJumpingFromWall > 0) //Can't grab immediately after jumping from a wall
                {
                    grabCooldownAfterJumpingFromWall--;
                }
                else if (staminaLeft > 0f) //Can't grab without stamina
                {
                    isGrabbing = true;
                    slidingOnWall = false;
                }
            }
            else if (keyGrab == KeyState.Up) //Stop grabbing if the button is released
            {
                isGrabbing = false;
                wallGrabbed = false;
                grabCooldownAfterJumpingFromWall = 0;
            }
        }

        //Check for wall to grab
        if (isGrabbing) //Wall can be grabbed only if the player is attemptiing to grab
        {
            Vector2 grabDirection;

            if (facingLeft) //Grabbable walls depend on facing
            {
                grabDirection = Vector2.left;
            }
            else
            {
                grabDirection = Vector2.right;
            }

            if (Physics2D.BoxCast(halfBottomHitboxCenter, halfBottomHitboxSize, 0f, grabDirection, .0625f, wall)) //Checking for a wall to grab
            {
                wallGrabbed = true; //Successfully grabbed a wall

                if (Mathf.Abs(rb.velocity.y) > .1f)
                {
                    staminaLeft--; //Stamina loss due to climbing
                }

                rb.velocity = Vector2.zero; //Stop any momentum to do properly the movements below

                if (keyJump == KeyState.Down) //Jumping while climbing
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0.85f * jumpForce);
                    wallGrabbed = false; //Jumping stops the player from grabbing the wall
                    isGrabbing = false; //Jumping ends grab
                    grabCooldownAfterJumpingFromWall = 10; //Time before a wall can be grabbed

                    staminaLeft -= 50f; //Stamina loss due to jumping
                }
                else if (Mathf.Abs(dirY) > .1f) //Check if going up or down
                {
                    rb.velocity = new Vector2(rb.velocity.x, Mathf.Sign(dirY) * climbSpeed); //Speed depending on up or down
                }

                if (staminaLeft <= 0f) //Stop grabbing if no stamina
                {
                    staminaLeft = 0f; //In case of having negative stamina (e.g. Jumping)
                    wallGrabbed = false;
                    isGrabbing = false;
                }
            }
            else
            {
                wallGrabbed = false; //Ending grab due to not being closed to a wall anymore
            }
        }
        else
        {
            wallGrabbed = false; //Ending grab due to release of key or Jump/Dash
        }

        if (IsGrounded()) //Restore stamina when touching the ground
        {
            staminaLeft = maxStamina;
            grabCooldownAfterJumpingFromWall = 0;
        }


    }
    private void DashCheck()
    {
        if (keyDash == KeyState.Down && dashLeft > 0 && !isDashing && !Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, spring)) //Can't dash if already dashing or no dash left
        {
            dashLeft--;
            isDashing = true;
            dashState = dashDuration;
            dashDirection = new Vector2(dirX, dirY);

            if (isGrabbing || wallGrabbed) //Dashing while grabbing => Cancel grab
            {
                if (dirX > .1f) //Update Facing for Dash
                {
                    facingLeft = false;
                }
                else if (dirX < -.1f)
                {
                    facingLeft = true;
                }

                //Reset grab variables
                isGrabbing = false;
                wallGrabbed = false;
                staminaLeft = maxStamina;
            }

            if (IsGrounded() && dashDirection.x != 0 && dashDirection.y == -1) //Correct dash if trying to dash toward the ground while being on it
            {
                dashDirection = new Vector2(dashDirection.x, 0); //Updating the dash direction to prevent it
            }

            if (dashDirection == Vector2.zero) //No directional inputs => Neutral dash is performed depending on facing
            {
                if (facingLeft)
                {
                    dashDirection = Vector2.left;
                }
                else
                {
                    dashDirection = Vector2.right;
                }
            }

            rb.velocity = dashDirection * dashSpeed; //Set speed relative to the dash direction
        }

        if (isDashing) //Dash Movement
        {
            if (dashState > 0) //Dash timer check
            {
                if (Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, dashDirection.x * Vector2.right, .1f, wall)) //Check if a wall is in the X-direction
                {
                    rb.velocity = new Vector2(0f, rb.velocity.y); //Stop horizontal movement if hitting a wall
                }
                if (Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, dashDirection.y * Vector2.up, .1f, wall)) //Check if a wall is in the X-direction
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0f); //Stop vertical movement if hitting a wall
                }


                if ((dashDuration - dashState) % 4 == 0) //Create phantom replica (Stylish !!)
                {
                    GameObject phantom = Instantiate(phantomMadeline, transform.position, Quaternion.identity);

                    phantom.GetComponent<PhantomVanish>().facingLeft = facingLeft; //Replica facing according the player's
                }

                dashState--; //Update dash timer
            }
            else //End dash
            {
                isDashing = false;
                rb.velocity = Vector2.zero;
            }
        }
        else if (IsGrounded()) //Restore dash when on the ground
        {
            dashLeft = dashNumber;
        }
    }
    private void UpdateFacing()
    {
        if (!isDashing && !wallGrabbed) //Facing can't be updated while dashing or grabbing
        {
            if (dirX > 0)
            {
                facingLeft = false;
            }
            else if (dirX < 0)
            {
                facingLeft = true;
            }
        }
    }
    private void UpdateGravity()
    {
        if (isDashing || wallGrabbed) //Gravity is stopped when dashing or grabbing
        {
            rb.gravityScale = 0f;
        }
        else if (slidingOnWall)
        {
            rb.gravityScale = 1f;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }

    }
    public bool IsGrounded() //Check if a wall is below the player
    {
        return (Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .0625f, wall) && !Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.zero, .0625f, wall));
    }
    public bool IsVeryGrounded() //Grounded for at least 5 frames
    {
        if (IsGrounded())
        {
            if (groundedFrames < 5)
            {
                groundedFrames++;
            }
        }
        else
        {
            groundedFrames = 0;
        }

        return groundedFrames == 5;
    }

    public void ResetDashAndGrab() //Reset all variables related to dash and grab
    {
        //Reset dash
        dashLeft = dashNumber;
        isDashing = false;
        dashState = 0;

        //Reset grab
        isGrabbing = false;
        wallGrabbed = false;
        staminaLeft = maxStamina;
        grabCooldownAfterJumpingFromWall = 0;
    }

    public void SetBoost(int boostDuration, Vector2 boostVector, bool keep)
    {
        boostedVelocity = true;
        rb.velocity = boostVector; //Boost the velocity
        boostedTimer = boostDuration;
        maxBoostedHorizontalSpeed = Mathf.Abs(rb.velocity.x);
        keepVelocityAfterBoost = keep;
    }

    private KeyState UpdateKeyState(string keyName)
    {
        KeyState key;

        if (Input.GetButton(keyName))
        {
            key = KeyState.Held;
        }
        else
        {
            key = KeyState.Off;
        }

        return key;
    }
    private KeyState FixedUpdateKeyState(KeyState tempKey, KeyState key)
    {
        /*
        An inputFixedUpdate need to be 'Down' (resp. 'Up') to be updated to 'Held' (resp. 'Off')

        If inputFixedUpdate was 'Off' (resp. 'Held') and inputUpdate is 'Held' (resp. 'Off'), then
        inputFixedUpdate is updated to 'Down' (resp. Up)
        */

        if (tempKey == KeyState.Held)
        {
            if (key == KeyState.Down || key == KeyState.Held)
            {
                key = KeyState.Held;
            }
            else
            {
                key = KeyState.Down;
            }
        }
        else if (tempKey == KeyState.Off)
        {
            if (key == KeyState.Up || key == KeyState.Off)
            {
                key = KeyState.Off;
            }
            else
            {
                key = KeyState.Up;
            }
        }

        return key;
    }
}