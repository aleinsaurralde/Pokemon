using System.Collections;
using UnityEngine;
using Cinemachine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] RandomEncounter randomEncounter;
    [SerializeField] CinemachineVirtualCamera worldCamera;

    private IGameState currentState;
    private FreeRoamState freeRoamState;
    private WildBattleState wildBattleState;
    private TrainerBattleState trainerBattleState;
    private DialogState dialogState;
    //private CutsceneState cutsceneState;
    private bool canMove = true;
    public bool CanMove => canMove;

    public BattleSystem BattleSystem => battleSystem;
    public CinemachineVirtualCamera WorldCamera => worldCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        ConditionsDB.Init();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Creamos las instancias de los estados
        freeRoamState = new FreeRoamState(this, playerController);
        wildBattleState = new WildBattleState(this, battleSystem, playerController);
        trainerBattleState = new TrainerBattleState(this, battleSystem, playerController, trainerController);
        dialogState = new DialogState();

        // Suscribimos eventos
        randomEncounter.OnEncounter += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
        DialogManager.Instance.OnShowDialog += StartDialog;
        DialogManager.Instance.OnCloseDialog += CloseDialog;

        foreach (var trainer in FindObjectsOfType<TrainerStartBattle>())
        {
            trainer.TriggerTrainerBattle += (trigger) =>
            {
                //TransitionToState(cutsceneState);
                // Buscar el TrainerController en el objeto padre
                var trainerController = trigger.GetComponentInParent<TrainerController>();
                if (trainerController != null)
                {
                    StartCoroutine(trainerController.TriggerTrainerBattle());
                }
                else
                {
                    Debug.LogWarning($"No se encontr� TrainerController en el padre de {trigger.name}");
                }
            };
        }

        // Estado inicial
        TransitionToState(freeRoamState);
    }

    private void Update()
    {
        currentState?.HandleUpdate();
    }

    public void StopMovement()
    {
        StartCoroutine(MoveCooldown());
    }

    private IEnumerator MoveCooldown()
    {
        canMove = false;
        yield return new WaitForSeconds(1f);
        canMove = true;
    }

    public void TransitionToState(IGameState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    private void StartBattle()
    {
        TransitionToState(wildBattleState);
    }

    private void EndBattle(bool won)
    {
        TransitionToState(freeRoamState);
    }
    private void StartDialog()
    {
        TransitionToState(dialogState);
    }
    private void CloseDialog()
    {
        TransitionToState(freeRoamState);
    }
}
