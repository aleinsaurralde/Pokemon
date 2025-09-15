using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

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
        StartCoroutine(hpBar.UpdateHealthUINumber(pokemon.HP));
    }

    public IEnumerator UpdateHPUI()
    {
        yield return hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHp);

    }
    public IEnumerator UpdateHPNumberUI()
    {
        yield return hpBar.UpdateHealthUINumber(_pokemon.HP);
    }

    public void AnimateHPChangeUI()
    {
        if (hpBar != null )
        {
            if (_pokemon.HpChanged)
            {
                StartCoroutine(UpdateHPUI());
                if (maxHp != null)
                {
                    StartCoroutine(UpdateHPNumberUI());
                    _pokemon.HpChanged = false;
                }
            }
        }

    }
}
