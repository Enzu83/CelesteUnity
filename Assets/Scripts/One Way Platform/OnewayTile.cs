using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnewayTile : MonoBehaviour
{
    public Sprite[] platformTiles;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    [SerializeField] private LayerMask wall;
    public int state;
    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();

        //Left edge - Check walls next to the platform => change tile's sprite
        if (state == -1)
        {
            if (Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.left, .0625f, wall))
            {
                sprite.sprite = platformTiles[0];
            }
            else
            {
                sprite.sprite = platformTiles[1];
            }
        }

        //Right edge - Check walls next to the platform => change tile's sprite
        else if (state == 1)
        {
            if (Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right, .0625f, wall))
            {
                sprite.sprite = platformTiles[4];
            }
            else
            {
                sprite.sprite = platformTiles[3];
            }
        }

        //Middle tile
        else if (state == 0)
        {
            sprite.sprite = platformTiles[2];
        }

        coll.enabled = false;
    }
}
