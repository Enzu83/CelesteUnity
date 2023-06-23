using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalActivation : MonoBehaviour
{
    private Animator anim;
    private PlayerMovement playerMovement;
    [SerializeField] private int reloadCountdown = 30;
    private int timer;
    private bool activated = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activated && collision.gameObject.CompareTag("Player"))
        {
            playerMovement = collision.gameObject.GetComponent<PlayerMovement>();

            if (playerMovement.dashLeft < playerMovement.dashNumber || playerMovement.staminaLeft < playerMovement.maxStamina)
            {
                activated = true;
                timer = reloadCountdown;

                playerMovement.dashLeft = playerMovement.dashNumber;
                playerMovement.staminaLeft = playerMovement.maxStamina;
            }
        }
    }

    private void FixedUpdate()
    {
        //Reload the crystal
        if (activated)
        {
            if (timer > 0)
            {
                timer--;
            }
            else
            {
                activated = false;
            }
        }

        anim.SetBool("activated", activated);
    }
}
