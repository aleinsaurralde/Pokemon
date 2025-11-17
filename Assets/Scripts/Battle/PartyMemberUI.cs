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
    [SerializeField] Sprite unselectedFaintedImage;
    [SerializeField] Sprite highlightedFaintedImage;

    [SerializeField] Sprite emptySlot;

    [SerializeField] Image statusArea;

    [SerializeField] Sprite psnImage;
    [SerializeField] Sprite slpImage;
    [SerializeField] Sprite parImage;
    [SerializeField] Sprite frzImage;
    [SerializeField] Sprite brnImage;
    [SerializeField] Sprite fntImage;

    Dictionary<ConditionID, Sprite> statusImage;

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

        statusImage = new Dictionary<ConditionID, Sprite>()
        {
            { ConditionID.psn, psnImage },
            { ConditionID.slp, slpImage },
            { ConditionID.brn, brnImage },
            { ConditionID.frz, frzImage },
            { ConditionID.par, parImage },
            { ConditionID.fnt, fntImage },
        };

        SetStatusImage();
        _pokemon.OnStatusChanged += SetStatusImage;

    }

    private void SetStatusImage()
    {
        if (_pokemon.Status == null)
        {
            statusArea.gameObject.SetActive(false);
        }
        else
        {
            statusArea.gameObject.SetActive(true);
            statusArea.sprite = statusImage[_pokemon.Status.Id];
        }
    }
    public void SetBGImage(bool selected, bool fainted)
    {
        if (!fainted)
        {
            if (selected)
                backgroundImage.sprite = highlightedImage;
            else
                backgroundImage.sprite = unselectedImage;
        }
        else
        {
            if (selected)
                backgroundImage.sprite = highlightedFaintedImage;
            else
                backgroundImage.sprite = unselectedFaintedImage;
        }
    }
    public void SetEmptyImage()
    {
        backgroundImage.sprite = emptySlot;
    }
   
}
