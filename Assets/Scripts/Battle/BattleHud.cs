using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI maxHp;
    [SerializeField] TextMeshProUGUI currentHp;
    [SerializeField] HPBar hpBar;


    Pokemon _pokemon;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        levelText.text = $"{pokemon.Level}";
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHp);
        if (maxHp != null)
        {
            maxHp.text = $"{pokemon.MaxHp}";
        }
        StartCoroutine(hpBar.UpdateHealthNumber(pokemon.HP));
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHp);

    }
    public IEnumerator UpdateHPNumber()
    {
        yield return hpBar.UpdateHealthNumber(_pokemon.HP);
    }

    public void AnimateHPChange()
    {
        StartCoroutine(UpdateHP());
        if (maxHp != null)
        {
            StartCoroutine(UpdateHPNumber());
        }
    }
}
