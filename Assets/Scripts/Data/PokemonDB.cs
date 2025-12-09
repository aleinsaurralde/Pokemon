using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PokemonDB
{
    static Dictionary<string, PokemonBase> pokemons;
    public static void Init()
    {
        pokemons = new Dictionary<string, PokemonBase>();

        var pokemonArray = Resources.LoadAll<PokemonBase>("");
        foreach (var pokemon in pokemonArray)
        {
            if (pokemons.ContainsKey(pokemon.Name))
            {
                Debug.LogError($"There are two pokemons with the name {pokemon.Name}");
                continue;
            }
            pokemons[pokemon.Name] = pokemon;
        }
    }

    public static PokemonBase GetPokemonByName(string name)
    {
        if (!pokemons.ContainsKey(name))
        {
            Debug.LogError($"Pokemon with name {name}not found in db");
            return null;
        }
        return pokemons[name];
    }
}
