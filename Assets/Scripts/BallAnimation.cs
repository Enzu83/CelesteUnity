using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAnimation : MonoBehaviour
{
    private GameObject player;
    private SpriteRenderer sprite;
    [SerializeField] private float distanceSpeed;
    [SerializeField] private float angularSpeed;
    private float maxDistance = 1.5f;
    private float distance;
    [HideInInspector] public float angle;
    private int timer = 0;
    [HideInInspector] public bool reverse = false;

    void Start()
    {
        player = GameObject.Find("Player");
        sprite = GetComponent<SpriteRenderer>();

        transform.localScale *= 0.8f; //Reduce size of the ball
        if (reverse == false) //If reverse : going toward the player and ball smaller at the beginning
        {
            distance = 0f;
        }
        else
        {
            distance = 1f;
            transform.localScale *= Mathf.Pow(0.8f, 6);
        }
    }

    void FixedUpdate()
    {
        transform.position = (Vector2)player.transform.position + Rotation(distance * Vector2.right, angle); //Update position

        //Update distance from player
        if (reverse == false) //distance 0 => maxDistance
        {
            if (distance < maxDistance) //Check if distance from player is not too big
            {
                distance = maxDistance * distanceSpeed * (float)timer / 12; //Increase distance => squared
            }
            else //If distance is too big
            {
                distance = maxDistance;
            }
        }
        else //distance maxDistance => 0
        {
            if (distance > .1f) //Check if distance is too small
            {
                distance = maxDistance * distanceSpeed * (float)(12 - timer) / 12; //Decrease distance => squared
            }
            else //If distance is too small
            {
                distance = 0;
            }
        }

        //Update angle of ball
        if (reverse == false) //Reverse changes rotation direction
        {
            angle += angularSpeed * Time.deltaTime;
        }
        else
        {
            angle -= angularSpeed * Time.deltaTime;
        }

        if (angle > 2 * Mathf.PI) //Angle superior than 2pi is useless
        {
            angle = angle % (2 * Mathf.PI); //Angle belongs to [0, 2pi[
        }

        if (timer % 14 < 5) //Ball color matching Madeline's hair
        {
            if (player.GetComponent<PlayerMovement>().dashLeft == 0) //No dash => Blue
            {
                sprite.color = new Color(67 / 255f, 163 / 255f, 245 / 255f);
            }
            else if (player.GetComponent<PlayerMovement>().dashLeft == 1) // 1 Dash => Red
            {
                sprite.color = new Color(172 / 255f, 32 / 255f, 32 / 255f);
            }
            else //Green color for other situations
            {
                sprite.color = Color.green;
            }
        }
        else //Set back the color to white
        {
            sprite.color = Color.white;
        }

        //Reduce size of ball
        if (reverse == false) //Ball getting smaller
        {
            if (timer >= 14)
            {
                transform.localScale *= 0.8f;
            }
        }
        else //Ball getting bigger
        {
            if (timer <= 5)
            {
                transform.localScale /= 0.8f;
            }
        }

        if (timer < 20) //Time before destroying object
        {
            timer++;
        }
        else
        {
            Destroy(this.gameObject); //Destroy object
        }
    }

    private Vector2 VectorDirection(Vector2 position1, Vector2 position2)
    {
        return (position2 - position1) / Vector2.Distance(position1, position2);
    }

    private Vector2 Rotation(Vector2 vector, float angle)
    {
        return new Vector2(Mathf.Cos(angle) * vector.x + Mathf.Sin(angle) * vector.y, -Mathf.Sin(angle) * vector.x + Mathf.Cos(angle) * vector.y);
    }
}

