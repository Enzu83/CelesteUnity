using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransitionManager : MonoBehaviour
{
    public GameObject virtualCamera;
    [SerializeField] private GameObject player;

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (virtualCamera.activeInHierarchy == false && coll.CompareTag("Player") && !coll.isTrigger)
        {
            if (coll.gameObject.GetComponent<Rigidbody2D>().velocity != Vector2.zero)
            {
                bool upperTransition;
                upperTransition = Camera.current.transform.position.y < transform.position.y; //Different behavior if player is going up

                virtualCamera.SetActive(true);
                player.GetComponent<StopObject>().Stop(0.4f, upperTransition, virtualCamera.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D coll)
    {
        if (virtualCamera.activeInHierarchy == true && coll.CompareTag("Player") && !coll.isTrigger)
        {
            if (coll.gameObject.GetComponent<Rigidbody2D>().velocity != Vector2.zero)
            {
                virtualCamera.SetActive(false);
            }
        }
    }


}
