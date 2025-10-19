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
    private GameBattleState battleState;

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
        battleState = new GameBattleState(this, battleSystem, playerController);

        // Suscribimos eventos
        randomEncounter.OnEncounter += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        // Estado inicial
        TransitionToState(freeRoamState);
    }

    private void Update()
    {
        currentState?.HandleUpdate();
    }

    public void TransitionToState(IGameState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    private void StartBattle()
    {
        TransitionToState(battleState);
    }

    private void EndBattle(bool won)
    {
        TransitionToState(freeRoamState);
    }
}
