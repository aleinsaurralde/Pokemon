using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomEncounter : MonoBehaviour
{
    [SerializeField] private int encounterChance = 10;

    private bool canCheck = true;
    private PlayerController playerInside; // referencia al jugador dentro del pasto

    public event Action OnEncounter;

    private void OnTriggerEnter(Collider other)
    {
        if ((GameLayers.i.PlayerLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            playerInside = other.GetComponent<PlayerController>();

            if (playerInside != null)
            {
                playerInside.OnStepFinished += HandlePlayerStep;
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
                player.OnStepFinished -= HandlePlayerStep;
            }

            playerInside = null;
        }
    }

    private void HandlePlayerStep()
    {
        if (canCheck)
        {
            if (Random.Range(1, 101) <= encounterChance)
            {
                OnEncounter?.Invoke();
            }
        }
    }
}
