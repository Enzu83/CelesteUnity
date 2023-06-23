using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapsingTileMovement : MonoBehaviour
{
    private bool move = false;

    private int timer = 4;

    private Vector2 offset = Vector2.zero;
    private Vector2 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (transform.parent.gameObject.GetComponent<CollapsingPlatformTiles>().collapsingState == 1)
        {
            move = true;
        }
        else if (transform.parent.gameObject.GetComponent<CollapsingPlatformTiles>().collapsingState == 2)
        {
            Destroy(this.gameObject);
        }

        if (move)
        {
            if (timer % 4 == 0)
            {
                int offsetX = (int)Random.Range(0, 3) - 1;
                int offsetY = (int)Random.Range(0, 3) - 1;

                offset = 0.0625f * new Vector2(offsetX, offsetY);
            }

            transform.position = initialPosition + offset;

            if (timer > 0)
            {
                timer--;
            }
            else
            {
                timer = 4;
            }
        }
    }
}
