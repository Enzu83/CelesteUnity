using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnewayPlatformInitialization : MonoBehaviour
{
    public Sprite[] platformTiles;
    [SerializeField] private GameObject tile;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    public bool wallOnTheLeft;
    public bool wallOnTheRight;

    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();

        coll.enabled = false; //Disable collision during initialization

        ///Left edge
        GameObject leftTile = Instantiate(tile, transform.position, Quaternion.identity);
        leftTile.transform.SetParent(transform);
        leftTile.transform.position = new Vector2(leftTile.transform.position.x - (2 * sprite.size.x - 1) / 4, leftTile.transform.position.y);

        if (wallOnTheLeft)
        {
            leftTile.GetComponent<SpriteRenderer>().sprite = platformTiles[0];
        }
        else
        {
            leftTile.GetComponent<SpriteRenderer>().sprite = platformTiles[1];
        }

        ///Right edge
        GameObject rightTile = Instantiate(tile, transform.position, Quaternion.identity);
        rightTile.transform.SetParent(transform);
        rightTile.transform.position = new Vector2(rightTile.transform.position.x + (2 * sprite.size.x - 1) / 4, rightTile.transform.position.y);

        if (wallOnTheRight)
        {
            rightTile.GetComponent<SpriteRenderer>().sprite = platformTiles[4];
        }
        else
        {
            rightTile.GetComponent<SpriteRenderer>().sprite = platformTiles[3];
        }

        //Create middle tile (if sprite size >= 1.5)
        if (sprite.size.x >= 1.5)
        {
            GameObject middleTile = Instantiate(tile, transform.position, Quaternion.identity);
            middleTile.transform.SetParent(transform);
            middleTile.transform.position = transform.position;
            middleTile.GetComponent<SpriteRenderer>().sprite = platformTiles[2];
            middleTile.GetComponent<SpriteRenderer>().size = new Vector2(sprite.size.x - 1, sprite.size.y);
        }

        sprite.enabled = false;
        coll.enabled = true;
    }
}
