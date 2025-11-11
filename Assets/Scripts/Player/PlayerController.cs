using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    //public float speed = 5f;

    //private Rigidbody rb;

    //private Animator animator;

    //void Start()
    //{
    //    rb = GetComponent<Rigidbody>();
    //    rb.constraints = RigidbodyConstraints.FreezeRotation;
    //}

    //public void HandleUpdate()
    //{
    //    float moveX = Input.GetAxisRaw("Horizontal"); // A/D o flechas
    //    float moveZ = Input.GetAxisRaw("Vertical");   // W/S o flechas

    //    Vector3 move = new Vector3(moveX, 0, moveZ)/*.normalized*/;

    //    rb.velocity = new Vector3(move.x * speed, rb.velocity.y, move.z * speed);

    //}
    public float moveSpeed = 5f;
    public float gridSize = 1f;
    public LayerMask obstacleLayer;
    public LayerMask interactableLayer;

    private bool isMoving = false;
    private Vector3 moveDirection;
    private Rigidbody rb;
    public event Action OnStepFinished;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void HandleUpdate()
    {
        if (!isMoving)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical");

            if (Mathf.Abs(moveX) > 0)
                moveZ = 0;

            if (moveX != 0 || moveZ != 0)
            {
                moveDirection = new Vector3(moveX, 0, moveZ).normalized;

                animator.SetFloat("moveX", moveX);
                animator.SetFloat("moveZ", moveZ);

                if (!Physics.Raycast(transform.position + Vector3.up * 0.1f, moveDirection, gridSize, obstacleLayer | interactableLayer))
                {
                    StartCoroutine(MoveOneTile());

                }
                else
                {
                    //Debug.Log("Movimiento bloqueado por obstáculo");
                }
            }
        }
        animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Interact();
        }
    }

    private IEnumerator MoveOneTile()
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + moveDirection * gridSize;

        float elapsedTime = 0f;
        float duration = gridSize / moveSpeed; // tiempo para moverse una casilla

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        isMoving = false;

        OnStepFinished?.Invoke();
    }

    private void Interact()
    {
        var facingDir = new Vector3(animator.GetFloat("moveX"), 0, animator.GetFloat("moveZ"));
        var interactPosition = transform.position + facingDir;

        Collider[] colliders = Physics.OverlapSphere(interactPosition, 0.3f, interactableLayer);

        if (colliders.Length > 0)
        {
            colliders[0].GetComponent<IInteractable>()?.Interact();
        }

        Debug.DrawLine(transform.position, interactPosition, Color.red, 0.5f);
    }

}



