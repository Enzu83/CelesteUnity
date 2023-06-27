using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxMacthingSpriteSize : MonoBehaviour
{
    private void Start()
    {
        float horizontalSizeFactor = 1f;
        if (this.gameObject.CompareTag("Spike")) //Smaller hitbox for sprite
        {
            horizontalSizeFactor = 0.9f;
        }

        Vector2 spriteSize = GetComponent<SpriteRenderer>().size;
        Vector2 boxSize = GetComponent<BoxCollider2D>().size;
        GetComponent<BoxCollider2D>().size = new Vector2(spriteSize.x * horizontalSizeFactor, boxSize.y);
    }
}
