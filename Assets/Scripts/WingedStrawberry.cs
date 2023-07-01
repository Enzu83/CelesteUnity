using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingedStrawberry : MonoBehaviour
{
    private bool screenFound = false;
    public GameObject screen;
    private Vector2 initialPosition;
    private bool fly = false;
    [SerializeField] private GameObject player;
    private Rigidbody2D rb;
    private int timer;
    private bool pickedUp = false;
    private GameObject strawberry;

    void Start()
    {
        initialPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();

        strawberry = transform.GetChild(0).gameObject;
    }

    void FixedUpdate()
    {
        if (pickedUp == false)
        {
            if (fly == false)
            {
                //Player is dashing
                if (player.GetComponent<PlayerMovement>().isDashing)
                {
                    fly = true;
                    timer = 0;
                }
            }
            else //Fly off screen
            {
                if (timer < 15)
                {
                    //Move slightly up
                    transform.position = Vector2.Lerp(transform.position, initialPosition + 1.75f * Vector2.up, 12f * Time.deltaTime);

                    //Oscillation animation
                    transform.localEulerAngles = new Vector3(0f, 0f, -30f * Mathf.Cos(Mathf.PI / 7 * timer));

                    timer++;
                }
                else if (transform.position.y < screen.transform.position.y + 6f)
                {
                    transform.localEulerAngles = Vector3.zero;
                    rb.velocity += new Vector2(0f, .3f);
                }
                else
                {
                    rb.velocity = Vector2.zero;
                    GetComponent<SpriteRenderer>().enabled = false;
                }
            }
        }
        else if (strawberry.GetComponent<StrawberryCollect>().state == 0)
        {
            Refresh();

            strawberry.GetComponent<SpriteRenderer>().enabled = false;
            strawberry.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (!screenFound && coll.gameObject.CompareTag("Screen"))
        {
            screen = coll.gameObject;
            screenFound = true;
        }
        //Collected
        else if (strawberry.GetComponent<StrawberryCollect>().state == 0 && coll.gameObject.CompareTag("Player")) //Picked up
        {
            //Update child
            strawberry.GetComponent<SpriteRenderer>().enabled = true;
            strawberry.GetComponent<BoxCollider2D>().enabled = true;
            strawberry.GetComponent<StrawberryCollect>().state = 1;
            strawberry.transform.position = transform.position;
            player.GetComponent<PlayerCollectables>().strawberries.Add(strawberry);

            //Update winged strawberry
            pickedUp = true;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;

            transform.localEulerAngles = Vector3.zero;
            rb.velocity = Vector2.zero;
        }
    }

    public void Refresh()
    {
        if (strawberry.GetComponent<StrawberryCollect>().state == 0)
        {
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<BoxCollider2D>().enabled = true;
            transform.position = initialPosition;
            fly = false;
            pickedUp = false;
            timer = 0;
        }
    }
}
