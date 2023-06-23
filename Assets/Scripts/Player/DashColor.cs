using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashColor : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private PlayerMovement playerMovement;
    private SpriteRenderer sprite;

    private int dashLeft;
    private bool isDashing;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        dashLeft = playerMovement.dashLeft;
        isDashing = playerMovement.isDashing;

        if (dashLeft == 0)
        {
            sprite.color = new Color(67 / 255f, 163 / 255f, 245 / 255f);
        }
        else if (dashLeft == 1)
        {
            sprite.color = new Color(172 / 255f, 32 / 255f, 32 / 255f);
        }
        else
        {
            sprite.color = Color.green;
        }

    }
}
