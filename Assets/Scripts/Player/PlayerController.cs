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

    [SerializeField] string playerName;
    [SerializeField] Sprite playerSprite;

    public float moveSpeed = 5f;
    public float gridSize = 1f;

    private bool isMoving = false;
    private Vector3 moveDirection;
    private Rigidbody rb;
    public event Action OnStepFinished;

    private CharacterAnimator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<CharacterAnimator>();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void HandleUpdate()
    {
        if (!isMoving && GameController.Instance.CanMove)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical");

            if (Mathf.Abs(moveX) > 0)
                moveZ = 0;

            if (moveX != 0 || moveZ != 0)
            {
                moveDirection = new Vector3(moveX, 0, moveZ).normalized;

                animator.MoveX = moveX;
                animator.MoveZ = moveZ;

                if (!Physics.Raycast(transform.position + Vector3.up * 0.1f, moveDirection, gridSize, GameLayers.i.ObstaclesLayer | GameLayers.i.InteractablesLayer))
                {
                    StartCoroutine(MoveOneTile());

                }
                else
                {
                    //Debug.Log("Movimiento bloqueado por obstáculo");
                }
            }
        }

        animator.IsMoving = isMoving;

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
        float duration = gridSize / moveSpeed;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = SnapToGrid(targetPos);

        isMoving = false;
        OnStepFinished?.Invoke();
    }

    private Vector3 SnapToGrid(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x - 0.5f) + 0.5f,
            pos.y,
            Mathf.Round(pos.z - 0.5f) + 0.5f
        );
    }


    private void Interact()
    {
        var facingDir = new Vector3(animator.MoveX, 0, animator.MoveZ);
        var interactPosition = transform.position + facingDir;

        Collider[] colliders = Physics.OverlapSphere(interactPosition, 0.3f, GameLayers.i.InteractablesLayer);
            
        if (colliders.Length > 0)
        {
            colliders[0].GetComponent<IInteractable>()?.Interact();
        }

        Debug.DrawLine(transform.position, interactPosition, Color.red, 0.5f);
    }

    public string PlayerName
    {
        get => playerName;
    }
    public Sprite PlayerSprite
    {
        get => playerSprite;
    }
}



