using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalOneWayCollision : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private BoxCollider2D coll;
    private BoxCollider2D playerColl;

    private void Start()
    {
        coll = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        playerColl = player.GetComponent<BoxCollider2D>();

        if (playerColl.bounds.center.y - (coll.bounds.center.y + coll.bounds.size.y / 2 + playerColl.bounds.size.y / 2) > 0f)
        {
            coll.enabled = true;
        }
        else
        {
            coll.enabled = false;
        }
    }
}
