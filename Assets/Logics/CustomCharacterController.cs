using System;
using UnityEngine;

public class CustomCharacterController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float dashForce = 15f;
    public float gravity = 9.8f;


    [Header("Collision Settings")]
    public LayerMask groundLayer;
    public Vector2 colliderSize = new Vector2(1f, 2f);
    [NonSerialized]
    public Vector2 velocity;
    private bool isGrounded;

    private void FixedUpdate()
    {
        // Apply gravity manually when not grounded
        if (!isGrounded)
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        // Check if grounded
        isGrounded = IsGrounded();

        // Stop vertical velocity if grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
        }

        ApplyMovement();
    }

    public void Move(float direction)
    {
        // Horizontal movement
        velocity.x = direction;
    }

    public void Jump(float jumpForce)
    {
        if (isGrounded)
        {
            velocity.y = jumpForce;
        }
    }

    public void Dash(float direction)
    {
        if (isGrounded) // Only allow dash if grounded
        {
            velocity.x = direction * dashForce;
        }
    }

    private void ApplyMovement()
    {
        Vector2 position = (Vector2)transform.position + velocity * Time.deltaTime;
        Vector2 offset = new Vector2(colliderSize.x / 2, colliderSize.y / 2);

        // Check collisions horizontally
        if (Physics2D.OverlapBox(position + new Vector2(velocity.x * Time.deltaTime, 0), colliderSize, 0, groundLayer))
        {
            velocity.x = 0;
        }

        // Check collisions vertically
        if (Physics2D.OverlapBox(position + new Vector2(0, velocity.y * Time.deltaTime), colliderSize, 0, groundLayer))
        {
            velocity.y = 0;
        }

        // Update position
        transform.position = position;
    }

    private bool IsGrounded()
    {
        Vector2 position = (Vector2)transform.position - new Vector2(0, colliderSize.y / 2);
        return Physics2D.OverlapBox(position, colliderSize, 0, groundLayer) != null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, colliderSize);
    }
}
