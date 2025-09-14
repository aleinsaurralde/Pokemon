using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI maxHp;
    [SerializeField] TextMeshProUGUI currentHp;
    [SerializeField] HPBar hpBar;

    [SerializeField] Image pokemonSprite;
    [SerializeField] Image backgroundImage;
    [SerializeField] Sprite unselectedImage;
    [SerializeField] Sprite highlightedImage;

    Pokemon _pokemon;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        levelText.text = $"Lv.{pokemon.Level}";
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHp);
        pokemonSprite.sprite = pokemon.Base.FrontSprite;
        maxHp.text = $"{pokemon.MaxHp}";
        currentHp.text = $"{pokemon.HP}";
    }

    public void SetSelected(bool selected)
    {
        if (selected) 
            backgroundImage.sprite = highlightedImage;
        else
            backgroundImage.sprite = unselectedImage;
    } 
}
