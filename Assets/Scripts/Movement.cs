using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 20f;
    private bool isGrounded;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

        // Freeze rotation to prevent spinning
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    public void Move(float FB, float RL)
    {
        Vector3 movement = Vector3.zero;

        if (FB == 1)
        {
            // Move forward
            movement += transform.forward * moveSpeed * Time.deltaTime;
        }
        else if (FB == -1)
        {
            // Move back
            movement -= transform.forward * moveSpeed * Time.deltaTime;
        }

        if (RL == 1)
        {
            // Move right
            movement += transform.right * moveSpeed * Time.deltaTime;
        }
        else if (RL == -1)
        {
            // Move left
            movement -= transform.right * moveSpeed * Time.deltaTime;
        }

        rb.MovePosition(rb.position + movement);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            Vector3 jump = new Vector3(0.0f, 1.0f, 0.0f);
            rb.AddForce(jump * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}