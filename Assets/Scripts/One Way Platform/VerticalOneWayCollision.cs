using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalOneWayCollision : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private BoxCollider2D coll;
    private BoxCollider2D playerColl;
    private PlayerMovement playerMovement;

    private void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        playerColl = player.GetComponent<BoxCollider2D>();
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    void FixedUpdate()
    {
        if (playerMovement.isDashing && playerMovement.dashDirection.y > 0)
        {
            coll.enabled = false;
        }
        else if (playerColl.bounds.center.y - playerColl.bounds.size.y / 2 > coll.bounds.center.y + coll.bounds.size.y / 2)
        {
            coll.enabled = true;
        }
        else
        {
            coll.enabled = false;
        }
    }
}
