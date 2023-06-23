using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomVanish : MonoBehaviour
{
    private SpriteRenderer sprite;

    [SerializeField] private int lifeTime = 15;
    private int countdown;

    public bool facingLeft;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        countdown = lifeTime;

        //Facing flip
        if (facingLeft)
        {
            this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, 180, this.transform.eulerAngles.z);
        }
        else
        {
            this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, 0, this.transform.eulerAngles.z);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (countdown > 0)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.Pow((float)countdown / (float)lifeTime, 2));

            countdown--;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
