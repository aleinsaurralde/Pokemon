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
    [SerializeField] Image statusArea;

    [SerializeField] Sprite psnImage;
    [SerializeField] Sprite slpImage;
    [SerializeField] Sprite parImage;
    [SerializeField] Sprite frzImage;
    [SerializeField] Sprite brnImage;

    [SerializeField] HPBar hpBar;   
    Pokemon _pokemon;
    Dictionary<ConditionID, Sprite> statusImage;
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

        statusImage = new Dictionary<ConditionID, Sprite>()
        {
            { ConditionID.psn, psnImage },
            { ConditionID.slp, slpImage },
            { ConditionID.brn, brnImage },
            { ConditionID.frz, frzImage },
            { ConditionID.par, parImage },
        };
        SetStatusImage();
        _pokemon.OnStatusChanged += SetStatusImage;
    }

    private void SetStatusImage()
    {
        if(_pokemon.Status == null)
        {
            statusArea.gameObject.SetActive(false);
        }
        else
        {
            if (_pokemon.Status.Id != ConditionID.fnt)
            {
                statusArea.gameObject.SetActive(true);            
                statusArea.sprite = statusImage[_pokemon.Status.Id];
            }
        }
    }

    public IEnumerator UpdateHPUI()
    {
        yield return hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHp);

    }
    public IEnumerator UpdateHPNumberUI()
    {
        yield return hpBar.UpdateHealthUINumber(_pokemon.HP);
    }

    //public void AnimateHPChangeUI()
    //{
    //    if (!gameObject.activeInHierarchy)
    //    {
    //        return;
    //    }
    //    if (_pokemon.HpChanged)
    //    {
    //        StartCoroutine(UpdateHPUI());
    //        if (maxHp != null)
    //        {
    //            StartCoroutine(UpdateHPNumberUI());
    //            _pokemon.HpChanged = false;
    //        }
    //    }            
    //}

    public IEnumerator AnimateHPChangeUI()
    {
        // Si el HUD no está activo o no hubo cambios, salimos
        if (!gameObject.activeInHierarchy || !_pokemon.HpChanged)
            yield break;

        // Lanzamos ambas corrutinas en paralelo y esperamos a que terminen
        yield return StartCoroutine(RunBoth(UpdateHPUI(), UpdateHPNumberUI()));

        // Reset del flag
        _pokemon.HpChanged = false;
    }
    private IEnumerator RunBoth(IEnumerator routineA, IEnumerator routineB)
    {
        bool finishedA = false;
        bool finishedB = false;

        // Arrancamos corrutina A
        StartCoroutine(RunAndNotify(routineA, () => finishedA = true));
        // Arrancamos corrutina B
        StartCoroutine(RunAndNotify(routineB, () => finishedB = true));

        // Esperamos hasta que ambas terminen
        yield return new WaitUntil(() => finishedA && finishedB);
    }
    private IEnumerator RunAndNotify(IEnumerator routine, System.Action onComplete)
    {
        yield return routine;
        onComplete?.Invoke();
    }
}
