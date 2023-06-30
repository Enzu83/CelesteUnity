using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public GameObject currentCamera = null;

    private void FixedUpdate()
    {
        if (Camera.current != null)
        {
            currentCamera = Camera.current.gameObject;
        }
    }
}
