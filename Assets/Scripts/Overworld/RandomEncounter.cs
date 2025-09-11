using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomEncounter : MonoBehaviour
{
    public LayerMask playerLayer;
    private bool canCheck = true;
    public float encounterCooldown = 0.25f;

    public event Action OnEncounter;
    private void CheckForEncounters()
    {
        if (Random.Range(1, 101) <= 10)
        {
            OnEncounter();
        }


    }

    private void OnTriggerStay(Collider other)
    {
        if ((playerLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if (rb.velocity.magnitude > 0.1f)
                {
                    if (canCheck)
                    {
                        CheckForEncounters();
                        StartCoroutine(EncounterCooldown());
                    }
                }
            }
        }
    }

    private IEnumerator EncounterCooldown()
    {
        canCheck = false;
        yield return new WaitForSeconds(encounterCooldown);
        canCheck = true;
    }
}
