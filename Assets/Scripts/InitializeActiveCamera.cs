using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeActiveCamera : MonoBehaviour
{
    public void Start()
    {
        foreach (GameObject screen in GameObject.FindGameObjectsWithTag("Screen"))
        {
            if (Mathf.Abs(transform.position.x - screen.transform.position.x) < 20f / 2 && Mathf.Abs(transform.position.y - screen.transform.position.y) < 11.25f / 2)
            {
                screen.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                screen.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}
