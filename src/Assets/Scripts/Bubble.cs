using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float deceleration = 1f;
    private Vector2 movement;
    private Vector2 currentVelocity;

    void FixedUpdate()
    {
        // Get input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        // Apply acceleration/deceleration
        if (movement.magnitude > 0)
        {
            currentVelocity = Vector2.Lerp(currentVelocity, movement * moveSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }

        // Move the bubble
        GetComponent<Rigidbody2D>().linearVelocity = currentVelocity;
    }
}