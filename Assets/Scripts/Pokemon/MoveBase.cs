using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PokemonType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int pp;

    public string Name
    {
        get { return name; }
    }
    public string Description
    {
        get { return description; }
    }
    public PokemonType Type
    {
        get { return type; }
    }
    public int Power
    {
        get { return power; }
    }
    public int Accuracy
    {
        get { return accuracy; }
    }
    public int PP
    {
        get { return pp; }
    }
}
//{

//    [field: SerializeField] public string name { get; private set; }

//    [field: SerializeField, TextArea] public string description { get; private set; }

//    [field: SerializeField] public PokemonType type { get; private set; }

//    [field: SerializeField] public int pp { get; private set; }

//    [field: SerializeField] public int power { get; private set; }

//    [field: SerializeField] public int accuracy { get; private set; }

//}
