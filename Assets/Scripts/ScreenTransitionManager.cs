using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransitionManager : MonoBehaviour
{
    public GameObject virtualCamera;
    [SerializeField] private GameObject player;
    private ScreenManager screenManager;

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (virtualCamera.activeInHierarchy == false && coll.CompareTag("Player") && !coll.isTrigger)
        {
            if (coll.gameObject.GetComponent<Rigidbody2D>().velocity != Vector2.zero) //Changing screen
            {
                screenManager = transform.parent.gameObject.GetComponent<ScreenManager>();

                bool upperTransition;
                upperTransition = screenManager.currentCamera.transform.position.y < transform.position.y; //Different behavior if player is going up

                virtualCamera.SetActive(true);
                screenManager.currentCamera = virtualCamera;
                player.GetComponent<StopObject>().Stop(0.4f, upperTransition, virtualCamera);

                RefreshWingedStrawberry(virtualCamera.transform.parent.gameObject);
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

    private void RefreshWingedStrawberry(GameObject screen)
    {
        GameObject[] wingedBerries = GameObject.FindGameObjectsWithTag("Winged Strawberry");
        foreach (GameObject berry in wingedBerries)
        {
            if (GameObject.ReferenceEquals(berry.GetComponent<WingedStrawberry>().screen, screen))
            {
                berry.GetComponent<WingedStrawberry>().Refresh();
            }
        }
    }
}
