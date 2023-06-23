using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalOscillation : MonoBehaviour
{
    [HideInInspector] public int timer;
    private int cycleDuration = 64;
    [SerializeField] private float amplitude;
    [SerializeField] private int ocsillationSpeed = 1;

    void Start()
    {
        timer = 8 * (int)Random.Range(0, 5); //Randomize the initial position

        amplitude *= .0625f; // 1px <-> .0625f;
    }

    void FixedUpdate()
    {
        if (timer < (int)(cycleDuration / 2))
        {
            if ((timer % (int)(8 / ocsillationSpeed)) == 0)
            {
                transform.position = new Vector2(transform.position.x, transform.position.y - amplitude);
            }
            timer++;
        }
        else if (timer < cycleDuration)
        {
            if ((timer % (int)(8 / ocsillationSpeed)) == 0)
            {
                transform.position = new Vector2(transform.position.x, transform.position.y + amplitude);
            }
            timer++;
        }
        else
        {
            timer = 0;
        }
    }
}
