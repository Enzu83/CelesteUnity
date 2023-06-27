using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnewayPlatformInitialization : MonoBehaviour
{
    [SerializeField] private GameObject tile;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;

    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();

        coll.enabled = false; //Disable collision during initialization

        //Resize collision with sprite size
        BoxCollider2D newColl = coll;
        newColl.size = new Vector2(sprite.size.x, newColl.size.y);

        ///Left edge
        GameObject leftTile = Instantiate(tile, transform.position, Quaternion.identity);
        leftTile.transform.SetParent(transform);
        leftTile.transform.position = new Vector2(leftTile.transform.position.x - (sprite.size.x - 1), leftTile.transform.position.y);
        leftTile.GetComponent<OnewayTile>().state = -1;

        ///Right edge
        GameObject rightTile = Instantiate(tile, transform.position, Quaternion.identity);
        rightTile.transform.SetParent(transform);
        rightTile.transform.position = new Vector2(rightTile.transform.position.x + (sprite.size.x - 1), rightTile.transform.position.y);
        rightTile.GetComponent<OnewayTile>().state = 1;

        //Create middle tile (if sprite size >= 1.5)
        if (sprite.size.x >= 1.5)
        {
            GameObject middleTile = Instantiate(tile, transform.position, Quaternion.identity);
            middleTile.transform.SetParent(transform);
            middleTile.transform.position = transform.position;
            middleTile.GetComponent<OnewayTile>().state = 0;
            middleTile.GetComponent<SpriteRenderer>().size = new Vector2(sprite.size.x - 1, sprite.size.y);
        }

        sprite.enabled = false;
        coll.enabled = true;
    }
}
