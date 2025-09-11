using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void HandleUpdate()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D o flechas
        float moveZ = Input.GetAxis("Vertical");   // W/S o flechas

        Vector3 move = new Vector3(moveX, 0, moveZ).normalized;

        rb.velocity = new Vector3(move.x * speed, rb.velocity.y, move.z * speed);
    }
}