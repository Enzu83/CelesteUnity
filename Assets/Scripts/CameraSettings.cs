using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSettings : MonoBehaviour
{
    [SerializeField] private bool enableFollowingCamera = false;

    [SerializeField] private Transform player;

    [SerializeField] private float cameraSize = 1f;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }
    private void Update()
    {
        if (enableFollowingCamera)
        {
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);

            cam.orthographicSize = 1 / cameraSize;
        }
    }
}
