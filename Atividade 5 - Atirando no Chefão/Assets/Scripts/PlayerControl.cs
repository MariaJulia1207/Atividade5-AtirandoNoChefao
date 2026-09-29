using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public Rigidbody2D player;
    void Start()
    {
        player = this.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    public void MovePlayer()
    {
        // Get input from the player
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // Apply movement to the player
        player.linearVelocity = new Vector2(moveX * moveSpeed, moveY * moveSpeed);  
    }
}
