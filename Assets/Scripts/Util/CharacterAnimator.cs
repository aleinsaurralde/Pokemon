using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> WalkDownSprites;
    [SerializeField] List<Sprite> WalkUpSprites;
    [SerializeField] List<Sprite> WalkRightSprites;
    [SerializeField] List<Sprite> WalkLeftSprites;
    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;
    // Parameters
    public float MoveX { get; set; }
    public float MoveZ { get; set; }
    public bool IsMoving { get; set; }

    // States
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;

    SpriteAnimator currentAnim;
    private bool wasPreviouslyMoving;

    // References
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(WalkDownSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(WalkUpSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(WalkRightSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(WalkLeftSprites, spriteRenderer);

        SetFacingDirection(defaultDirection);
        currentAnim = walkDownAnim;
    }

    private void Update()
    {
        var prevAnim = currentAnim;

        if (MoveX == 1)
        {
            currentAnim = walkRightAnim;
        }
        else if (MoveX == -1)
        {
            currentAnim = walkLeftAnim;
        }
        else if (MoveZ == 1)
        {
            currentAnim = walkUpAnim;
        }
        else if (MoveZ == -1)
        {
            currentAnim = walkDownAnim;
        }

        if (currentAnim != prevAnim || IsMoving != wasPreviouslyMoving)
        {
            currentAnim.Start();
        }

        if (IsMoving)
        {
            currentAnim.HandleUpdate();
        }
        else
        {
            spriteRenderer.sprite = currentAnim.Frames[0];
        }

        wasPreviouslyMoving = IsMoving;
    }

    public void SetFacingDirection(FacingDirection dir)
    {
        switch (dir)
        {
            case FacingDirection.Left:
                MoveX = -1;
                break;

            case FacingDirection.Right:
                MoveX = 1;
                break;

            case FacingDirection.Up:
                MoveZ = 1;
                break;

            case FacingDirection.Down:
                MoveZ = -1;
                break;
        }
    }
    public FacingDirection DefaultDirection
    {
        get => defaultDirection;
    }
}

public enum FacingDirection { Up, Down, Left, Right } 
