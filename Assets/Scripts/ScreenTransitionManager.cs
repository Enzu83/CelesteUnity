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
            virtualCamera.SetActive(true);
            player.GetComponent<StopObject>().Stop(0.4f);
        }
    }

    private void OnTriggerExit2D(Collider2D coll)
    {
        if (virtualCamera.activeInHierarchy == true && coll.CompareTag("Player") && !coll.isTrigger)
        {
            virtualCamera.SetActive(false);
        }
    }


}
