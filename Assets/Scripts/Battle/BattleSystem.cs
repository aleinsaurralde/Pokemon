using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleAction { Move, SwitchPokemon, UseItem, Run }

public class BattleSystem : MonoBehaviour
{
    public static BattleSystem Instance;

    [Header("Units & UI")]
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] InventoryUI inventoryUI;

    public BattleUnit PlayerUnit => playerUnit;
    public BattleUnit EnemyUnit => enemyUnit;
    public BattleDialogBox DialogBox => dialogBox;
    public PartyScreen PartyScreen => partyScreen;
    public Image PlayerImage => playerImage;
    public Image TrainerImage => trainerImage;
    public Inventory PlayerInventory => playerParty.GetComponent<Inventory>();
    public InventoryUI InventoryUI => inventoryUI;

    public IGameState currentState { get; private set; }
    public IGameState prevState;
    public Dictionary<Type, IGameState> states { get; private set; }

    public event Action<bool> OnBattleOver;

    public int currentAction;
    public int currentMove;
    public int currentMember;
    public bool usingItem;
    public int itemIndex;

    public PokemonParty playerParty { get; private set; }
    public PokemonParty trainerParty { get; private set; }
    public Pokemon wildPokemon { get; private set; }

    private bool isTrainerBattle = false;
    public bool IsTrainerBattle => isTrainerBattle;

    public PlayerController player { get; private set; }
    public TrainerController trainer { get; private set; }

    public int escapeAttempts { get; set; }
    public IBattleAIStrategy enemyAIStrategy { get; set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
    }

    private void InitializeStates()
    {
        states = new Dictionary<Type, IGameState>
        {
            { typeof(StartState), new StartState(this) },
            { typeof(ActionSelectionState), new ActionSelectionState(this) },
            { typeof(MoveSelectionState), new MoveSelectionState(this) },
            { typeof(RunningTurnState), new RunningTurnState(this) },
            { typeof(PartyScreenState), new PartyScreenState(this) },
            { typeof(BattleOverState), new BattleOverState(this) },
            { typeof(BusyState), new BusyState(this) },
            { typeof(BagSelectionState), new BagSelectionState(this, PlayerInventory) },
        };
    }

    public void ChangeState<T>() where T : IGameState
    {
        currentState?.Exit();
        currentState = states[typeof(T)];
        currentState.Enter();
    }

    public void StartWildBattle(PokemonParty pParty, Pokemon wPokemon)
    {
        playerParty = pParty;
        wildPokemon = wPokemon;
        player = playerParty.GetComponent<PlayerController>();
        isTrainerBattle = false;

        InitializeStates();

        ChangeState<StartState>();
    }

    public void StartTrainerBattle(PokemonParty pParty, PokemonParty tParty)
    {
        playerParty = pParty;
        trainerParty = tParty;
        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();

        InitializeStates();

        ChangeState<StartState>();
    }

    public void BattleOver(bool won)
    {
        ChangeState<BattleOverState>();
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        OnBattleOver?.Invoke(won);
    }

    public void ActionSelection()
    {
        ChangeState<ActionSelectionState>();
        dialogBox.SetDialog("Choose an action");
        dialogBox.EnableActionSelector(true);
    }

    public void OpenPartyScreen()
    {
        ChangeState<PartyScreenState>();
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    public IEnumerator RunTurns(BattleAction playerAction)
    {
        ChangeState<RunningTurnState>();

        if (playerAction == BattleAction.Move)
        {
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMove];
            enemyUnit.Pokemon.CurrentMove = enemyAIStrategy.ChooseMove(enemyUnit.Pokemon, playerUnit.Pokemon);

            int playerPriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enemyPriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;
            bool playerGoesFirst = true;

            if (enemyPriority > playerPriority) playerGoesFirst = false;
            else if (enemyPriority == playerPriority) playerGoesFirst = playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed;

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (currentState == states[typeof(BattleOverState)]) yield break;

            if (secondUnit.Pokemon.HP > 0)
            {
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (currentState == states[typeof(BattleOverState)]) yield break;
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchPokemon)
            {
                var selectedPokemon = playerParty.Pokemons[currentMember];
                yield return SwitchPokemon(selectedPokemon);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                dialogBox.EnableActionSelector(false);
            }
            else if (playerAction == BattleAction.Run)
            {
                yield return TryToEscape();
            }

            if (currentState != states[typeof(BattleOverState)])
            {
                var enemyMove = enemyUnit.Pokemon.GetRandomMove();
                yield return RunMove(enemyUnit, playerUnit, enemyMove);
                yield return RunAfterTurn(enemyUnit);
            }
        }

        if (currentState != states[typeof(BattleOverState)])
        {
            ActionSelection();
        }
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.Hud.AnimateHPChangeUI();
            yield break;
        }

        yield return ShowStatusChanges(sourceUnit.Pokemon);
        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.Name}");

        if (BattleFormulas.CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
        {
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(0.50f);

            if (move.Base.Target != MoveTarget.Self) targetUnit.PlayHitAnimation();

            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                yield return targetUnit.Hud.AnimateHPChangeUI();
                yield return ShowDamageDetails(damageDetails);
            }

            if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0)
            {
                foreach (var secondary in move.Base.Secondaries)
                {
                    if ((secondary.Target != MoveTarget.Self && targetUnit.Pokemon.HP > 0) || (secondary.Target == MoveTarget.Self))
                    {
                        var rnd = UnityEngine.Random.Range(1, 101);
                        if (rnd <= secondary.Chance)
                            yield return RunMoveEffects(secondary, sourceUnit.Pokemon, targetUnit.Pokemon, secondary.Target);
                    }
                }
            }

            if (targetUnit.Pokemon.HP <= 0)
            {
                yield return HandlePokemonFainted(targetUnit);
                yield return RunAfterTurn(sourceUnit);
            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}'s attack missed!");
        }
    }

    public IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (currentState == states[typeof(BattleOverState)]) yield break;
        yield return new WaitUntil(() => currentState == states[typeof(RunningTurnState)]);

        sourceUnit.Pokemon.OnAfterTurn();
        yield return sourceUnit.Hud.AnimateHPChangeUI();
        yield return ShowStatusChanges(sourceUnit.Pokemon);

        if (sourceUnit.Pokemon.HP <= 0)
        {
            yield return HandlePokemonFainted(sourceUnit);
        }
    }

    public IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    public IEnumerator RunMoveEffects(MoveEffects effects, Pokemon sourceUnit, Pokemon targetUnit, MoveTarget moveTarget)
    {
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self) sourceUnit.ApplyBoosts(effects.Boosts);
            else targetUnit.ApplyBoosts(effects.Boosts);
        }
        if (effects.Status != ConditionID.none) targetUnit.SetStatus(effects.Status);
        if (effects.VolatileStatus != ConditionID.none) targetUnit.SetVolatileStatus(effects.VolatileStatus);

        yield return ShowStatusChanges(sourceUnit);
        yield return ShowStatusChanges(targetUnit);
    }

    public void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            faintedUnit.Pokemon.SetStatus(ConditionID.fnt);
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null) OpenPartyScreen();
            else BattleOver(false);
        }
        else
        {
            BattleOver(true);
        }
    }

    public IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f) yield return dialogBox.TypeDialog("A critical hit!");

        string effectivenessMsg = BattleFormulas.GetEffectivenessMessage(damageDetails.TypeEffectiveness);
        if (!string.IsNullOrEmpty(effectivenessMsg))
            yield return dialogBox.TypeDialog(effectivenessMsg);
    }

    public void HandleUpdate()
    {
        currentState?.HandleUpdate();
    }

    public IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        ChangeState<BusyState>();

        if (playerUnit.Pokemon.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back, {playerUnit.Pokemon.Base.Name}");
            playerUnit.Pokemon.CureVolatileStatus();
            playerUnit.Pokemon.OnBattleOver();
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");
        ChangeState<ActionSelectionState>();
    }

    private IEnumerator TryToEscape()
    {
        ChangeState<BusyState>();

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"You can't run from trainer battles!");
            ChangeState<ActionSelectionState>();
            yield break;
        }

        ++escapeAttempts;

        if (BattleFormulas.CanEscape(playerUnit.Pokemon.Speed, enemyUnit.Pokemon.Speed, escapeAttempts))
        {
            yield return dialogBox.TypeDialog($"Ran away safely!");
            BattleOver(true);
        }
        else
        {
            yield return dialogBox.TypeDialog($"Can't escape!");
            ChangeState<RunningTurnState>();
        }
    }

    private IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        yield return dialogBox.TypeDialog($"{faintedUnit.Pokemon.Base.Name} Fainted");
        faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(2f);

        if (!faintedUnit.IsPlayerUnit)
        {
            int expGain = BattleFormulas.CalculateExperienceGain(faintedUnit.Pokemon, isTrainerBattle);

            playerUnit.Pokemon.Exp += expGain;
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} gained {expGain} exp");
            yield return playerUnit.Hud.SetExpSmooth();

            while (playerUnit.Pokemon.CheckForLevelUp())
            {
                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} grew to level {playerUnit.Pokemon.Level}");
                yield return playerUnit.Hud.SetExpSmooth(true);
            }
            yield return new WaitForSeconds(1f);
        }
        CheckForBattleOver(faintedUnit);
    }
}