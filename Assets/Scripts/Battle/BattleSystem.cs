using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum BattleAction {Move, SwitchPokemon, UseItem, Run }

public class BattleSystem : MonoBehaviour
{
    public static BattleSystem Instance;

    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;

    public BattleUnit PlayerUnit => playerUnit;
    public BattleUnit EnemyUnit => enemyUnit;
    public BattleDialogBox DialogBox => dialogBox;
    public PartyScreen PartyScreen => partyScreen;
    public Image PlayerImage => playerImage;
    public Image TrainerImage => trainerImage;

    public IGameState currentState { get; private set;}
    public IGameState prevState;
    public Dictionary<Type, IGameState> states { get; private set; }

    public event Action<bool> OnBattleOver;

    public int currentAction;
    public int currentMove;
    public int currentMember;

    public PokemonParty playerParty { get; private set; }
    public PokemonParty trainerParty { get; private set; }

    private bool isTrainerBattle = false;
    public bool IsTrainerBattle => isTrainerBattle;
    public PlayerController player { get; private set; }
    public TrainerController trainer { get; private set; }
    public Pokemon wildPokemon { get; private set; }

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
        DontDestroyOnLoad(gameObject);

        states = new Dictionary<Type, IGameState>
        {
            { typeof(StartState), new StartState(this) },
            { typeof(ActionSelectionState), new ActionSelectionState(this) },
            { typeof(MoveSelectionState), new MoveSelectionState(this) },
            { typeof(RunningTurnState), new RunningTurnState(this) },
            { typeof(PartyScreenState), new PartyScreenState(this) },
            { typeof(BattleOverState), new BattleOverState(this) },
            { typeof(BusyState), new BusyState(this) },
        };
    }

    public void ChangeState<T>() where T : IGameState
    {
        currentState?.Exit();
        currentState = states[typeof(T)];
        currentState.Enter();
    }


    public void StartWildBattle(PokemonParty PlayerParty, Pokemon WildPokemon)
    {
        playerParty = PlayerParty;
        wildPokemon = WildPokemon;
        isTrainerBattle = false;

        ChangeState<StartState>();
    }
    public void StartTrainerBattle(PokemonParty PlayerParty, PokemonParty TrainerParty)
    {
        playerParty = PlayerParty;
        trainerParty = TrainerParty;

        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();
        ChangeState<StartState>();
    }
    private void BattleOver(bool won)
    {
        ChangeState<BattleOverState>();
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        OnBattleOver(won);        
    }
    public void ActionSelection()
    {
        ChangeState<ActionSelectionState>();
        dialogBox.SetDialog("Choose an action");
        dialogBox.EnableActionSelector(true);
    }
    private void OpenPartyScreen()
    {
        ChangeState<PartyScreenState>();
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);

        if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    public IEnumerator RunTurns (BattleAction playerAction)
    {
        ChangeState<RunningTurnState>();

        if (playerAction == BattleAction.Move)
        {
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();

            int playerMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;

            //check who goes first, with priority first then with speed

            bool playerGoesFirst = true;

            if (enemyMovePriority > playerMovePriority)
            {
                playerGoesFirst = false;
            }
            else if (enemyMovePriority == playerMovePriority)
            {
                playerGoesFirst = playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed;
            }
                


                var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var secondPokemon = secondUnit.Pokemon;
            //First turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (currentState == states[typeof(BattleOverState)])
            {
                yield break;
            }
            if (secondPokemon.HP > 0)
            {
                //Second Turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (currentState == states[typeof(BattleOverState)])
                {
                    yield break;
                }
            }
        }
        else 
        {
            if (playerAction == BattleAction.SwitchPokemon)
            {
                var selectedPokemon = playerParty.Pokemons[currentMember];
                ChangeState<BusyState>();
                yield return SwitchPokemon(selectedPokemon);
            }
            //enemy turn
            var enemyMove = enemyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (currentState == states[typeof(BattleOverState)])
            {
                yield break;
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

        if (CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
        {
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(0.50f);
            if (move.Base.Target != MoveTarget.Self)
            {
                targetUnit.PlayHitAnimation();
            }
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

            if(move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 )
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
                yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name} Fainted");
                targetUnit.PlayFaintAnimation();

                yield return new WaitForSeconds(2f);

                CheckForBattleOver(targetUnit);
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

        yield return new WaitUntil(()=> currentState == states[typeof(RunningTurnState)]);
        //stat conditions might faint a pokemon after its turn
        sourceUnit.Pokemon.OnAfterTurn();
        yield return sourceUnit.Hud.AnimateHPChangeUI();
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        if (sourceUnit.Pokemon.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} Fainted");
            sourceUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);

            CheckForBattleOver(sourceUnit);
        }
    }
    private bool CheckIfMoveHits(Move move, Pokemon sourceUnit, Pokemon targetUnit)
    {
        if (move.Base.AlwaysHit)
        {
            return true;
        }
        float moveAccuracy = move.Base.Accuracy;

        int accuracy = sourceUnit.StatBoosts[Stat.Accuracy];
        int evasion = targetUnit.StatBoosts[Stat.Evasion];

        var boostValues = new float[] { 1f, 4f / 3f, 5f/3f, 2f, 7f/3f, 8f/3f, 3f };

        if(accuracy > 0)
        {
            moveAccuracy *= boostValues[accuracy];
        }
        else 
        {
            moveAccuracy /= boostValues[-accuracy];
        }
        if(evasion > 0)
        {
            moveAccuracy /= boostValues[evasion];
        }
        else 
        {
            moveAccuracy *= boostValues[-evasion];
        }

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
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
        //stat boost
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
            {
                sourceUnit.ApplyBoosts(effects.Boosts);
            }
            else
            {
                targetUnit.ApplyBoosts(effects.Boosts);
            }
        }
        //altered condition
        if (effects.Status != ConditionID.none)
        {
            targetUnit.SetStatus(effects.Status);
        }
        // volatile status
        if (effects.VolatileStatus != ConditionID.none)
        {
            targetUnit.SetVolatileStatus(effects.VolatileStatus);
        }
        yield return ShowStatusChanges(sourceUnit);
        yield return ShowStatusChanges(targetUnit);
    }

    public void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            faintedUnit.Pokemon.SetStatus(ConditionID.fnt);
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                OpenPartyScreen();
            }
            else
                BattleOver(false);
        }
        else
        {
            BattleOver(true);
        }

    }

    public IEnumerator ShowDamageDetails (DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("A critical hit!"); //crit

        switch (damageDetails.TypeEffectiveness)
        {
            case > 1 and < 4:
                yield return dialogBox.TypeDialog("It's super effective!"); //x2 weak
                break;
            case >= 4:
                    yield return dialogBox.TypeDialog("It's ultra mega duper effective!"); //x4 weak
                break;
            case < 1 and  > 0.25f:
                    yield return dialogBox.TypeDialog("It's not very effective..."); //resistant
                break;
            case <= 0.25f and  > 0:
                    yield return dialogBox.TypeDialog("It's almost ineffective..."); //4x resistant
                break;
            case <= 0:
                    yield return dialogBox.TypeDialog("Seems unnaffected..."); //immune
                break;
        }
    }

    public void HandleUpdate()
    {
        currentState?.HandleUpdate();
    }

    public IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        if (playerUnit.Pokemon.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back, {playerUnit.Pokemon.Base.Name}");
            playerUnit.Pokemon.CureVolatileStatus();
            playerUnit.Pokemon.OnBattleOver(); //reset stat boost on switch for active unit
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);            
        }

        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);

        yield return (dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!"));

        ChangeState<ActionSelectionState>();
    }
   
}
