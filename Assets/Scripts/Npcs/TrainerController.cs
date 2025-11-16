using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
    [SerializeField] string trainerName;
    [SerializeField] Sprite trainerSprite;    
    [SerializeField] GameObject exclamation;
    [SerializeField] Dialog dialog;
    [SerializeField] GameObject fov;

    public event Action OnTrainerBattle;

    public IEnumerator TriggerTrainerBattle()
    {
        GameController.Instance.StopMovement();
        //show exclamation
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        //show dialog
        StartCoroutine (DialogManager.Instance.ShowDialog(dialog, () =>
        {
            Debug.Log("Trainer Battle");
            var battleState = GameController.Instance.BattleState;

            battleState.ConfigureTrainerBattle(this);

            OnTrainerBattle?.Invoke();

        }));
        yield return null;
    }

    private void Start()
    {
        SetFOVRotation(gameObject.GetComponentInChildren<CharacterAnimator>().DefaultDirection);
    }

    private void SetFOVRotation(FacingDirection dir)
    {
        float angle = 0f;

        switch (dir)
        {
            case FacingDirection.Left:
                angle = 270;
                break;

            case FacingDirection.Right:
                angle = 90;
                break;

            case FacingDirection.Up:
                angle = 180;
                break;

            case FacingDirection.Down:
                angle = 0;
                break;
        }

        fov.transform.eulerAngles = new Vector3 (0f,angle,0f);
    }

    public string TrainerName
    {
        get => trainerName;
    }
    public Sprite TrainerSprite
    {
        get => trainerSprite;
    }
}
