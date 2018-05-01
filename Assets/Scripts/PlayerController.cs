using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed;
    public float jumpheight;

    private float hzaxis;
    private float jump;
    private Rigidbody2D rb;
    private int jumpcountdown;

    // Use this for initialization
    void Start () {
        jumpcountdown = 0;
        rb = GetComponent<Rigidbody2D>(); // Get the rigidbody of the palyer
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        jumpcountdown--; // Countdown the jump countdown
        hzaxis = Input.GetAxis("Horizontal");
        jump = Input.GetAxis("Jump"); // Get control axes
        transform.rotation = Quaternion.Euler(0, 0, 0); // Enforce zero rotation
        rb.AddForce(new Vector2(hzaxis * speed, 0));  // Propel the player in their desired direction of motion
	}

    private void OnCollisionStay2D(Collision2D collision) // If they are colliding with a tile
    {
        if (jump > 0 && jumpcountdown < 0) // If they press jump and they have not jumped in the last 5 physics updates
        {
            rb.AddForce(new Vector2(0, jumpheight)); // Jump
            jumpcountdown = 5; // Set a countdown (this stops huge launches into the air)
        }
    }
}
