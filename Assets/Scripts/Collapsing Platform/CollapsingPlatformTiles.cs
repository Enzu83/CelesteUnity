using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapsingPlatformTiles : MonoBehaviour
{
    private SpriteRenderer sprite;
    private BoxCollider2D coll;
    [SerializeField] private LayerMask playerMask;

    [SerializeField] private GameObject collapsingTile;
    public GameObject player;
    public Sprite[] tiles;
    [SerializeField] private Sprite collaspedSprite;

    public int collapsingState = 0;
    [SerializeField] private int timeBeforeCollapsing = 75;
    [SerializeField] private int timeAfterCollapsing = 105;

    private int timer = 0;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();

        sprite.sprite = collaspedSprite; //Set sprite

        for (int i = 0; i < (int)(2 * sprite.size.x); i++) //Create all platform tiles
        {
            GameObject tile = Instantiate(collapsingTile, transform.position, Quaternion.identity); //Create the tile

            tile.transform.parent = transform; //Define the platform as a parent
            tile.transform.position = new Vector2(tile.transform.position.x + ((float)i - sprite.size.x + 0.5f) / 2, tile.transform.position.y); //Transform the tile to match the position it should have

            int index = (int)Random.Range(0, 4); //Random sprite for tile
            tile.GetComponent<InitializeCollapsingTile>().SetTileSprite(index); //Change tile sprite
        }

        sprite.enabled = false; //Hiding sprite to show children's tile sprite

    }

    void FixedUpdate()
    {

        if (Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.up, .1f, playerMask) && !Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.zero, .1f, playerMask) && player.GetComponent<PlayerMovement>().IsGrounded() && collapsingState == 0)
        {
            collapsingState = 1; //Start collapsing

        }

        if (collapsingState > 0)
        {
            //Remove hitbox and change sprite
            if (collapsingState == 1 && (timer == timeBeforeCollapsing || !Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.up, .1f, playerMask))) //Collapsing earlier if player is leaving the platform
            {
                collapsingState = 2;
                timer = 30;

                coll.enabled = false;

                sprite.enabled = true;
            }


            //Timer before collapse reset
            if (timer < timeBeforeCollapsing + timeAfterCollapsing)
            {
                timer++;
            }
            else
            {
                timer = 0;
                collapsingState = 0;
                coll.enabled = true;
                Start();
            }
        }
    }
}
