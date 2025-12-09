using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, ISaveable
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

    }

    public object CaptureState()
    {
        var saveData = new PlayerSaveData()
        {
            position = new float[] { transform.position.x, transform.position.y, transform.position.z },
            pokemons = GetComponent<PokemonParty>().Pokemons.Select(p=> p.GetSaveData()).ToList(),
            inventory = GetComponent<Inventory>().CaptureState() as BagSaveData
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (PlayerSaveData)state;
        var pos = saveData.position;
        transform.position = new Vector3(pos[0], pos[1], pos[2]);

        GetComponent<PokemonParty>().Pokemons = saveData.pokemons.Select(s=>new Pokemon(s)).ToList();
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

[Serializable]
public class PlayerSaveData
{
    public float[] position;
    public List<PokemonSaveData> pokemons;
    public BagSaveData inventory;
}

