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
    private StopObject stop;
    public LayerMask wall;
    public LayerMask spring;

    private Vector2 hitboxCenter;
    private Vector2 hitboxSize;
    private Vector2 halfBottomHitboxCenter = Vector2.zero;
    private Vector2 halfBottomHitboxSize = Vector2.zero;
    public Vector2 squishedOffset = Vector2.zero;
    private Vector2 squishedLimit;

    public bool facingLeft = false;
    public bool isAirborne;
    private int groundedFrames = 0;
    private DeathAndRespawn deathResp;

    //Speed relatived variables
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float maxVerticalSpeed = 20f;
    public bool boostedVelocity = false;
    public int boostedTimer = 0;
    private bool keepVelocityAfterBoost = false;
    [HideInInspector] public float maxBoostedHorizontalSpeed;
    public float gravityScale;

    //Dash Variables
    [SerializeField] public int dashNumber = 1;
    [SerializeField] private int dashDuration = 8;
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float waveDashFactor;
    [SerializeField] private bool isWaveDashing = false;
    [SerializeField] private GameObject phantomMadeline;
    [HideInInspector] public Vector2 dashDirection = Vector2.zero;
    public int dashState = 0;
    [HideInInspector] public int dashLeft;
    [HideInInspector] public bool isDashing = false;
    private bool wallBouncing = false;

    //Grab Variables
    [HideInInspector] public bool isGrabbing = false;
    [HideInInspector] public bool wallGrabbed = false;
    public float maxStamina = 180f;
    public float staminaLeft;
    public float climbSpeed = 4f;
    [HideInInspector] public int grabCooldownAfterJumpingFromWall = 0;
    [SerializeField] private bool nextToWall = false;
    private int nextToWallDirection = 0;
    public bool slidingOnWall = false;
    private int canNeutralJumpTimer = 0; //After changing direction, the player is not sliding anymore but has few frames to perform a wallbounce
    [SerializeField] private int canNeutralJumpDuration = 10;
    private bool neutralJumpFacingLeft;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        deathResp = GetComponent<DeathAndRespawn>();
        stop = GetComponent<StopObject>();

        gravityScale = rb.gravityScale;
        dashLeft = dashNumber;
        staminaLeft = maxStamina;

        squishedLimit = new Vector2(coll.bounds.size.x - 2 * 0.0625f, coll.bounds.size.y - 2 * 0.0625f);
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

        //Update hitbox references
        halfBottomHitboxCenter = new Vector2(coll.bounds.center.x, coll.bounds.center.y + coll.bounds.size.y / 4);
        halfBottomHitboxSize = new Vector2(coll.bounds.size.x, coll.bounds.size.y / 2);

        hitboxCenter = coll.bounds.center;
        hitboxSize = coll.bounds.size;

        if (!deathResp.dead && !stop.stopped) //Player can't act if dead
        {
            UpdateFacing();

            UpdateSliding();

            GrabCheck();

            DashCheck();

            UpdateWaveDash();

            UpdateBoost();

            UpdateGravity();

            UpdateVelocity();

            UpdateSquish();
        }

        isAirborne = !IsGrounded();
    }
    private void UpdateVelocity()
    {
        if (!isDashing && !isWaveDashing && !wallGrabbed && !slidingOnWall) //Can't move freely if dashing or grabbing a wall
        {
            //Horizontal Movement

            if (!boostedVelocity) //Normal movement
            {
                if (IsGrounded()) //Horizontal movement on the ground or high speed movement
                {
                    rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
                }
                else if (Mathf.Abs(rb.velocity.x) < moveSpeed && dirX != 0) //Horizontal movement in the air
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
            else if (boostedVelocity && boostedTimer == 0) //Horizontal Movement when player is boosted (e.g. by moving platform)
            {
                if (IsGrounded() && !isWaveDashing)
                {
                    boostedVelocity = false; //Reset boost
                    rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
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
            }
        }

        if (keyJump == KeyState.Down && IsGrounded() && !isDashing && !wallGrabbed && !slidingOnWall) //Jump detection
        {
            if (transform.parent != null)
            {
                GameObject obj = transform.parent.gameObject;
                if (obj.CompareTag("Moving Platform")) //If the player is on a moving platform => doesn't jump but ejected
                {
                    StateUpdate state = obj.GetComponent<StateUpdate>();
                    if (state.EjectPlayer())
                    {
                        state.playerJumped = true;
                    }
                    else //Normal jump
                    {
                        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

                        //Abort dash but horizontal speed is conserved
                        isDashing = false;
                        dashState = 0;
                        dashLeft = dashNumber;
                    }
                }
            }
            else //Normal jump
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

                //Abort dash but horizontal speed is conserved
                isDashing = false;
                dashState = 0;
                dashLeft = dashNumber;
            }
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
        //Check if player is next to a wall
        if (Physics2D.BoxCast(halfBottomHitboxCenter, halfBottomHitboxSize, 0f, Vector2.left, .0625f, wall) && !IsGrounded() && !wallGrabbed)
        {
            nextToWall = true;
            nextToWallDirection = -1;
        }
        else if (Physics2D.BoxCast(halfBottomHitboxCenter, halfBottomHitboxSize, 0f, Vector2.right, .0625f, wall) && !IsGrounded() && !wallGrabbed)
        {
            nextToWall = true;
            nextToWallDirection = 1;
        }
        else
        {
            nextToWall = false;
            nextToWallDirection = 0;
        }
        //Sliding movement
        if (slidingOnWall)
        {
            neutralJumpFacingLeft = facingLeft;

            //Limit vertical speed
            if (keyJump != KeyState.Down || canNeutralJumpTimer == 0)
            {
                if (rb.velocity.y < -maxVerticalSpeed / 2)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -maxVerticalSpeed / 2);
                }
            }
        }
        if (nextToWall)
        {
            neutralJumpFacingLeft = facingLeft;
            //Update walljump timer - max when sliding
            canNeutralJumpTimer = canNeutralJumpDuration;
        }
        else if (IsGrounded())
        {
            canNeutralJumpTimer = 0;
        }
        else
        {
            //Update wallbounce timer - decrease when not sliding
            if (canNeutralJumpTimer > 0)
            {
                canNeutralJumpTimer--;
            }
        }

        //Neutral jump
        if (keyJump == KeyState.Down && canNeutralJumpTimer > 0)
        {
            canNeutralJumpTimer = 0;

            Vector2 newSpeed = Vector2.zero;

            if (slidingOnWall) //Sliding
            {
                if (neutralJumpFacingLeft)
                {
                    newSpeed = new Vector2(1.6f * moveSpeed, 0.9f * jumpForce);
                }
                else
                {
                    newSpeed = new Vector2(-1.6f * moveSpeed, 0.9f * jumpForce);
                }
                facingLeft = !neutralJumpFacingLeft; //Invert facing

                //Apply speed
                SetBoost(10, newSpeed, false);
            }
            else if (nextToWall) //Only next to a wall
            {
                if (nextToWallDirection == -1)
                {
                    facingLeft = false;
                    newSpeed = new Vector2(1.6f * moveSpeed, 0.9f * jumpForce);
                }
                else if (nextToWallDirection == 1)
                {
                    facingLeft = true;
                    newSpeed = new Vector2(-1.6f * moveSpeed, 0.9f * jumpForce);
                }

                //Apply speed
                SetBoost(10, newSpeed, false);
            }

            slidingOnWall = false;
            nextToWall = false;
            nextToWallDirection = 0;
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
                    nextToWall = false;
                    canNeutralJumpTimer = 0;
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
                    if (transform.parent != null)
                    {
                        GameObject obj = transform.parent.gameObject;
                        if (obj.CompareTag("Moving Platform")) //If the player is on a moving platform => doesn't jump but ejected
                        {
                            StateUpdate state = obj.GetComponent<StateUpdate>();
                            if (state.EjectPlayer())
                            {
                                state.playerJumped = true;
                            }
                            else
                            {
                                rb.velocity = new Vector2(rb.velocity.x, 0.85f * jumpForce);

                                wallGrabbed = false; //Jumping stops the player from grabbing the wall
                                isGrabbing = false; //Jumping ends grab
                                grabCooldownAfterJumpingFromWall = 10; //Time before a wall can be grabbed
                                staminaLeft -= 50f; //Stamina loss due to jumping
                            }
                        }
                    }
                    else
                    {
                        rb.velocity = new Vector2(rb.velocity.x, 0.85f * jumpForce);

                        wallGrabbed = false; //Jumping stops the player from grabbing the wall
                        isGrabbing = false; //Jumping ends grab
                        grabCooldownAfterJumpingFromWall = 10; //Time before a wall can be grabbed
                        staminaLeft -= 50f; //Stamina loss due to jumping
                    }
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
                canNeutralJumpTimer = 0;
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
                //Wavedash check
                if (IsGrounded() && keyJump == KeyState.Down && dashDirection.x != 0)
                {
                    float tempSpeedX;
                    if (facingLeft)
                    {
                        tempSpeedX = -waveDashFactor * moveSpeed;
                    }
                    else
                    {
                        tempSpeedX = waveDashFactor * moveSpeed;
                    }


                    //Check wavedash efficiency
                    float midDuration = dashDuration / 2;

                    if (dashDirection.y != 0 && Mathf.Abs(dashState - midDuration) <= 2) //Restore dash if wavedash is efficient
                    {
                        dashLeft = dashNumber;
                    }

                    if (dashDirection.y == 0 && dashDuration - dashState > 5) //Restore dash condition when performed on ground
                    {
                        dashLeft = dashNumber;
                    }

                    if (dashDirection.y == 0 && dashState < dashDuration - 2) //Horizontal speed effiency
                    {
                        float reduceFactor;
                        reduceFactor = 0.6f + 0.4f * 1 / 3 * Mathf.Max(0, 5 - Mathf.Abs(dashDuration - midDuration));
                        tempSpeedX *= reduceFactor;
                    }

                    dashState = 0;

                    SetBoost(12, new Vector2(tempSpeedX, jumpForce), true);
                    isWaveDashing = true;
                }
                //Wallbounce check (wall on the left) - only if dash is up
                else if (dashDirection == Vector2.up && keyJump == KeyState.Down && Physics2D.BoxCast(halfBottomHitboxCenter, halfBottomHitboxSize, 0f, Vector2.left, .5f, wall))
                {
                    isDashing = false;
                    dashState = 0;
                    facingLeft = false;
                    wallBouncing = true;

                    SetBoost(8, dashSpeed * new Vector2(0.53f, 1.2f), true);
                }
                //Wallbounce check (wall on the right) - only if dash is up
                else if (dashDirection == Vector2.up && keyJump == KeyState.Down && Physics2D.BoxCast(halfBottomHitboxCenter, halfBottomHitboxSize, 0f, Vector2.right, .5f, wall))
                {
                    isDashing = false;
                    dashState = 0;
                    facingLeft = true;
                    wallBouncing = true;

                    SetBoost(8, dashSpeed * new Vector2(-0.53f, 1.2f), true);
                }
                //Update dash direction if hitting a wall
                else
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
            }
            else if (!isWaveDashing) //End dash
            {
                isDashing = false;
                rb.velocity = Vector2.zero;
            }
        }
        else if (IsGrounded() && !isWaveDashing) //Restore dash when on the ground
        {
            dashLeft = dashNumber;
        }

        if (wallBouncing)
        {
            //Stop wall bouncing direction when hitting a wall
            if (Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, dashDirection.x * Vector2.right, .1f, wall)) //Check if a wall is in the X-direction
            {
                rb.velocity = new Vector2(0f, rb.velocity.y); //Stop horizontal movement if hitting a wall
            }
            if (Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, dashDirection.y * Vector2.up, .1f, wall)) //Check if a wall is in the X-direction
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f); //Stop vertical movement if hitting a wall
            }
        }

        //End of boost of wallbounce and wavedash
        if (boostedTimer == 0 && (wallBouncing || isWaveDashing))
        {
            if (boostedVelocity)
            {
                rb.velocity = new Vector2(0.95f * rb.velocity.x, rb.velocity.y);

                if (Mathf.Abs(rb.velocity.x) < .0625f)
                {
                    rb.velocity = new Vector2(0f, rb.velocity.y);
                    boostedVelocity = false;
                }
            }
            else if (wallBouncing)
            {
                wallBouncing = false;
            }
            else if (isWaveDashing)
            {
                isWaveDashing = false;
            }
        }
    }
    private void UpdateWaveDash()
    {
        if (isWaveDashing)
        {
            if (boostedTimer == 0)
            {
                isWaveDashing = false;
                isDashing = false;
            }
        }
    }
    private void UpdateBoost()
    {
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
        if ((isDashing && !isWaveDashing) || wallGrabbed) //Gravity is stopped when dashing or grabbing
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
    private void UpdateSquish() //Size is reduced if a wall is next to it. If size is too small => Kill the player
    {
        Vector2 tempUpdatedOffset = Vector2.zero; //Changes in size is stored and updated at the end to keep the same squished offsets for all 4 checks

        //Horizontal check
        if (Physics2D.BoxCast(hitboxCenter, hitboxSize - squishedOffset, 0f, Vector2.left, 0.0625f, wall) && Physics2D.BoxCast(hitboxCenter, hitboxSize - squishedOffset, 0f, Vector2.right, 0.0625f, wall))
        {
            tempUpdatedOffset = Vector2.left;
        }

        //Vertical check
        if (Physics2D.BoxCast(hitboxCenter, hitboxSize - squishedOffset, 0f, Vector2.down, 0.0625f, wall) && Physics2D.BoxCast(hitboxCenter, hitboxSize - squishedOffset, 0f, Vector2.up, 0.0625f, wall))
        {
            tempUpdatedOffset = Vector2.up;
        }

        //Update squished offsets

        //Horizontal offset
        if (tempUpdatedOffset.x == 0) //If no changes in offset, decrease it
        {
            if (squishedOffset.x > 0)
            {
                squishedOffset = new Vector2(squishedOffset.x - 0.0625f, squishedOffset.y);
            }
        }
        else
        {
            if (squishedOffset.x < squishedLimit.x)
            {
                squishedOffset = new Vector2(squishedOffset.x + 0.0625f, squishedOffset.y);
            }
            else
            {
                //Kill player
                deathResp.dead = true; //Death trigger
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0f; //Stop gravity
            }
        }
        //Vertical offset
        if (tempUpdatedOffset.y == 0) //If no changes in offset, decrease it
        {
            if (squishedOffset.y > 0)
            {
                squishedOffset = new Vector2(squishedOffset.x, squishedOffset.y - 0.0625f);
            }
        }
        else
        {
            if (squishedOffset.y < squishedLimit.y)
            {
                squishedOffset = new Vector2(squishedOffset.x, squishedOffset.y + 0.0625f);
            }
            else
            {
                //Kill player
                deathResp.dead = true; //Death trigger
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0f; //Stop gravity
            }
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