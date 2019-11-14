using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public Rigidbody2D rb;
    public float speed = 5;
    public float jumpForce = 10;
    private float x;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxis("Horizontal");

        transform.position += new Vector3(x * speed * Time.deltaTime, 0, 0);
    }

    void FixedUpdate() {
        if (Input.GetButtonDown("Jump")) {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }
}
