using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
<<<<<<< Updated upstream
    public float speed = 5f;
=======
    [SerializeField] string playerName;
    [SerializeField] Sprite sprite;

    //public float speed = 5f;
>>>>>>> Stashed changes

    private Rigidbody rb;

    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void HandleUpdate()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D o flechas
        float moveZ = Input.GetAxis("Vertical");   // W/S o flechas

        Vector3 move = new Vector3(moveX, 0, moveZ)/*.normalized*/;

        rb.velocity = new Vector3(move.x * speed, rb.velocity.y, move.z * speed);


        
        //transform.Translate(Vector3.right * moveX * speed * Time.deltaTime);
        //transform.Translate(Vector3.forward * moveZ* speed * Time.deltaTime);
    }
<<<<<<< Updated upstream
=======

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
        var facingDir = new Vector3(animator.MoveX, 0, animator.MoveZ);
        var interactPosition = transform.position + facingDir;

        Collider[] colliders = Physics.OverlapSphere(interactPosition, 0.3f, GameLayers.i.InteractablesLayer);
            
        if (colliders.Length > 0)
        {
            colliders[0].GetComponent<IInteractable>()?.Interact();
        }

        Debug.DrawLine(transform.position, interactPosition, Color.red, 0.5f);
    }

    public string PlayerName { get => playerName; }
    public Sprite Sprite { get => sprite; }
>>>>>>> Stashed changes
}

