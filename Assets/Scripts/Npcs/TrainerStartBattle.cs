using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TrainerStartBattle : MonoBehaviour
{
    private PlayerController playerInside;
    private bool defeated;
    public event Action <TrainerStartBattle> TriggerTrainerBattle;

    private void OnTriggerEnter(Collider other)
    {
        if ((GameLayers.i.PlayerLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            playerInside = other.GetComponent<PlayerController>();

            if (playerInside != null)
            {
                TriggerTrainerBattle?.Invoke(this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((GameLayers.i.PlayerLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {

            }
        }
    }
}
