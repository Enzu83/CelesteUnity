using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeCollapsingTile : MonoBehaviour
{
    public void SetTileSprite(int indexOfTile)
    {
        //Matching sprite with index of tile from parent
        GetComponent<SpriteRenderer>().sprite = GetComponentInParent<CollapsingPlatformTiles>().tiles[indexOfTile];
    }
}
