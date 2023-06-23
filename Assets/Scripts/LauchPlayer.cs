using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauchPlayer : MonoBehaviour
{
    [SerializeField] private float bounceSpeed;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetComponent<Animator>().SetTrigger("Launched"); //Bouncing animation for spring

        GameObject player = collision.gameObject;
        player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, bounceSpeed); //Bounce the player

        player.GetComponent<PlayerMovement>().ResetDashAndGrab(); //Reset dash and grab
    }
}
