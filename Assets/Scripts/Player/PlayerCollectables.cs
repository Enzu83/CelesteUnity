using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollectables : MonoBehaviour
{
    public List<GameObject> strawberries = new List<GameObject>();

    private DeathAndRespawn deathResp;
    void Start()
    {
        strawberries.Add(this.gameObject);
        deathResp = GetComponent<DeathAndRespawn>();
    }

    private void Update()
    {
        if (deathResp.dead) //Death consequences
        {
            if (strawberries != new List<GameObject>()) //Check if 
            {
                foreach (GameObject strawberry in strawberries) //Reset strawberries state
                {
                    if (!strawberry.CompareTag("Player"))
                    {
                        strawberry.GetComponent<StrawberryCollect>().state = -1; //In this state, strawberry goes to their original position
                    }
                }
            }
            strawberries = new List<GameObject>();
            strawberries.Add(this.gameObject); //Remove all strawberries from the list
        }
    }
}
