using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var conditions = kvp.Value;

            conditions.Id = conditionId;
        }
    }
    public static Dictionary<ConditionID, Conditions> Conditions { get; set; } = new Dictionary<ConditionID, Conditions>()
    {
        { ConditionID.psn,
            new Conditions()
            {
                Name = "Poison",
                StartMessage = "has been poisoned",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} was hurt due to poison");
                }
            }
        },
        { 
            ConditionID.brn,
            new Conditions()
            {
                Name = "Burn",
                StartMessage = "has been burned",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    if(pokemon.MaxHp/16 < 1)
                    {
                        pokemon.UpdateHP(1);
                    }
                    else
                    {
                        pokemon.UpdateHP(pokemon.MaxHp / 16);
                    }
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} was hurt due to it's burn");
                }
            }
        },
        { 
            ConditionID.par,
            new Conditions()
            {
                Name = "Paralyzed",
                StartMessage = "has been paralyzed",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1, 5) == 1) 
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}'s paralyzed and cant move!");
                        return false;
                    }
                    return true; 
                }
            }
        },
        { 
            ConditionID.frz,
            new Conditions()
            {
                Name = "Freeze",
                StartMessage = "has been frozen",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1, 6) == 1) 
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} has thawed out!");
                        return true;
                    }
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is frozen solid!");
                    return false; 
                }
            }
        },
        { 
            ConditionID.slp,
            new Conditions()
            {
                Name = "Sleep",
                StartMessage = "has fell asleep",
                OnStart = (Pokemon pokemon) =>
                {
                    //Sleep for 1-3 turns
                    pokemon.StatusTime = Random.Range(1, 4) ;
                    Debug.Log($"Will be asleep for {pokemon.StatusTime} turns");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if(pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} woke up!");
                        return true;
                    }
                    
                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is sleeping");
                    return false; 
                }
            }
        },
        { 
            ConditionID.confusion,
            new Conditions()
            {
                Name = "Confusion",
                StartMessage = "has been confused",
                OnStart = (Pokemon pokemon) =>
                {
                    //Confused for 2-5 turns
                    pokemon.VolatileStatusTime = Random.Range(2, 6) ;
                    Debug.Log($"Will be confused for {pokemon.VolatileStatusTime} turns");
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if(pokemon.VolatileStatusTime <= 0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} snapped out of its confusion!");
                        return true;
                    }
                    
                    pokemon.VolatileStatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is confused");
                    if(Random.Range(1, 4) == 1) //66% chance to act
                    {
                        return true;
                    }
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.CurrentMove.PP--;
                    pokemon.StatusChanges.Enqueue($"It hurt itself due to it's confusion!");
                    return false; 
                }
            }
        },
        {
            ConditionID.fnt,
            new Conditions()
            {
                Name = "Faint",
                StartMessage = "has fell fainted",                
                
            }
        },
    };
}

public enum ConditionID
{
    none, psn, brn, slp, par, frz, fnt,
    confusion,
}
