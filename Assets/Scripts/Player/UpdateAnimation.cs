using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateAnimation : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;

    private enum AnimationState { idle, walk, jump, fall, dash, climb };
    private AnimationState state;

    [SerializeField] private PlayerMovement playerMovement;

    private int lowStaminaTimer = 0;
    //Hair Movement
    [SerializeField] private HairAnchor hairAnchor;

    [Header("Hair Animation Offsets")]
    [SerializeField] private Vector2 idleOffset;
    [SerializeField] private Vector2 walkOffset;
    [SerializeField] private Vector2 jumpOffset;
    [SerializeField] private Vector2 fallOffset;
    [SerializeField] private Vector2 dashOffset;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        //Facing flip
        if (playerMovement.facingLeft)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
        else
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
        }

        //Update Animation State
        if (playerMovement.wallGrabbed)
        {
            state = AnimationState.climb;

            if (Mathf.Abs(playerMovement.dirY) > .1f) //Stop animation when climbing
            {
                anim.speed = 1f;
            }
            else
            {
                anim.speed = 0f;
            }
        }
        else if (playerMovement.slidingOnWall)
        {
            state = AnimationState.climb;
            anim.speed = 0f;
        }
        else
        {
            anim.speed = 1f;

            if (playerMovement.isDashing)
            {
                state = AnimationState.dash;
            }
            else if (rb.velocity.y > .1f)
            {
                state = AnimationState.jump;
            }
            else if (rb.velocity.y < -.1f)
            {
                state = AnimationState.fall;
            }
            else if (playerMovement.IsGrounded())
            {
                if (rb.velocity.x != 0)
                {
                    state = AnimationState.walk;
                }
                else
                {
                    state = AnimationState.idle;
                }
            }
        }

        anim.SetInteger("state", (int)state);


        //Update Hair Offset
        Vector2 currentOffset = Vector2.zero;

        if (state == AnimationState.idle)
        {
            currentOffset = idleOffset;
        }
        else if (state == AnimationState.walk)
        {
            currentOffset = walkOffset;
        }
        else if (state == AnimationState.jump)
        {
            currentOffset = jumpOffset;
        }
        else if (state == AnimationState.fall)
        {
            currentOffset = fallOffset;
        }
        else if (state == AnimationState.dash)
        {
            currentOffset = dashOffset;
        }
        else if (state == AnimationState.climb || playerMovement.slidingOnWall) //Sliding hair movement
        {
            if (rb.velocity.y > .1f)
            {
                currentOffset = jumpOffset;
            }
            else if (rb.velocity.y < -.1f)
            {
                currentOffset = fallOffset;
            }
        }
        else if (state == AnimationState.climb)
        {
            if (playerMovement.dirY > .1f)
            {
                currentOffset = jumpOffset;
            }
            else if (playerMovement.dirY < -.1f)
            {
                currentOffset = fallOffset;
            }
            else
            {
                currentOffset = idleOffset;
            }
        }

        if (playerMovement.facingLeft)
        {
            currentOffset = new Vector2(-currentOffset.x, currentOffset.y);
        }

        hairAnchor.partOffset = currentOffset;


        //Low stamina
        if (playerMovement.staminaLeft < 60f)
        {
            if (lowStaminaTimer % (4 + (int)(playerMovement.staminaLeft / 2)) <= 1) //Condition for blinking faster when stamina decreases
            {
                sprite.color = Color.red;
            }
            else
            {
                sprite.color = Color.white;
            }

            if (lowStaminaTimer < 60) //Clock for blinking red
            {
                lowStaminaTimer++;
            }
            else
            {
                lowStaminaTimer = 0;
            }

        }
        else
        {
            lowStaminaTimer = 0;
            sprite.color = Color.white;
        }
    }
}
