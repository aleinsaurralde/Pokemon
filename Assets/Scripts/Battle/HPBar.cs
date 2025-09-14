using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;
    [SerializeField] TextMeshProUGUI currentHealth;
    private int displayedHP;
    public void SetHP (float hpNormalized)
    {

        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }

    public IEnumerator SetHPSmooth(float newHp)
    {
        float curHp = health.transform.localScale.x;
        float changeAmt = curHp - newHp;

        while (curHp - newHp > Mathf.Epsilon)
        {
            curHp -= changeAmt * Time.deltaTime;
            SetHP(curHp);
            yield return null;
        }
        SetHP(newHp);
    }
    public IEnumerator UpdateHealthNumber(int targetHp)
    {
        int startHp = displayedHP;
        float t = 0f;
        float duration = 0.5f; // how long the number animates

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            int hpValue = Mathf.RoundToInt(Mathf.Lerp(startHp, targetHp, t));
            if (currentHealth != null)
            {
                currentHealth.text = hpValue.ToString();
            }
            displayedHP = hpValue;
            yield return null;
        }
        if (currentHealth != null)
        {
            currentHealth.text = targetHp.ToString();
        }
        displayedHP = targetHp;
    }

}
