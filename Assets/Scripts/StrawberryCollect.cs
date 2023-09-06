using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrawberryCollect : MonoBehaviour
{
    public int state = 0;
    [HideInInspector] public Vector2 initialPosition;
    private Vector2 relativeOffset;
    private List<GameObject> strawberries;

    [SerializeField] private GameObject player;
    [SerializeField] private Vector2 offset;
    [SerializeField] private float lerpSpeed = 15f;
    private Animator anim;

    private void Start()
    {
        strawberries = player.GetComponent<PlayerCollectables>().strawberries;
        anim = GetComponent<Animator>();
        initialPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (state == 0 && collision.gameObject.CompareTag("Player"))
        {
            state = 1;
            player.GetComponent<PlayerCollectables>().strawberries.Add(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        if (state == -1) //Return to initial position
        {
            if (Vector2.Distance(transform.position, initialPosition) > .1f) //Check if the strawberry is too far from origin
            {
                transform.position = Vector2.Lerp(transform.position, initialPosition, Time.deltaTime * lerpSpeed); //Target position with lerp
            }
            else //Make sure the strawberry is correctly placed
            {
                GetComponent<VerticalOscillation>().timer = 0; //Reset vertical oscillation
                transform.position = initialPosition;
                state = 0;
            }
        }
        else if (state == 1) //Follow Target
        {
            strawberries = player.GetComponent<PlayerCollectables>().strawberries; //Update strawberry list

            int index = strawberries.IndexOf(this.gameObject);
            GameObject target = strawberries[index - 1];

            if (player.GetComponent<PlayerMovement>().facingLeft)
            {
                relativeOffset = new Vector2(offset.x, offset.y);
            }
            else
            {
                relativeOffset = new Vector2(-offset.x, offset.y);
            }

            Vector2 targetPosition = (Vector2)target.transform.position + relativeOffset;
            Vector2 newPositionLerp = Vector2.Lerp(transform.position, targetPosition, Time.deltaTime * lerpSpeed);

            transform.position = newPositionLerp;

            if (Vector2.Distance(transform.position, targetPosition) < .2f && player.GetComponent<PlayerMovement>().IsVeryGrounded() && target == player)
            {
                state = 2; //Vanish animation trigger
                anim.SetBool("Vanish Animation", true);
            }
        }
    }

    public void DestroyStrawberry()
    {
        if (transform.parent != null)
        {
            //Destroy(transform.parent.gameObject);
        }
        strawberries.Remove(this.gameObject);
        Destroy(this.gameObject);
    }
}
