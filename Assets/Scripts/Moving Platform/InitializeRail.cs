using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeRail : MonoBehaviour
{
    [SerializeField] private GameObject endPoint;
    [HideInInspector] public Vector2 startPosition;
    [HideInInspector] public Vector2 endPosition;

    [SerializeField] private GameObject trafficLight;
    [SerializeField] private GameObject railObject;
    [SerializeField] private GameObject railWheel;
    public float trafficLightOffset;

    void Start()
    {
        startPosition = transform.position;
        endPosition = endPoint.transform.position;

        GameObject light = Instantiate(trafficLight, startPosition, Quaternion.identity);
        light.transform.parent = transform;

        GameObject startWheel = Instantiate(railWheel, startPosition, Quaternion.identity);
        startWheel.transform.parent = transform;


        GameObject endWheel = Instantiate(railWheel, endPosition, Quaternion.identity);
        endWheel.transform.parent = transform;


        GameObject rail = Instantiate(railObject, startPosition, Quaternion.identity);
        rail.transform.parent = transform;

        rail.transform.position += 0.5f * (Vector3)(endPosition - startPosition);

        float initialAngle = -Vector2.Angle(Vector2.up, (Vector2)(endPosition - startPosition));
        rail.transform.localEulerAngles = new Vector3(0f, 0f, initialAngle);

        if (endPosition.x < startPosition.x) //Flip rail
        {
            rail.transform.localEulerAngles = new Vector3(0f, 180f, rail.transform.localEulerAngles.z);
        }

        SpriteRenderer railSprite = rail.GetComponent<SpriteRenderer>();
        railSprite.size = new Vector2(0.625f, 2f * Vector2.Distance(startPosition, endPosition));
    }
}
