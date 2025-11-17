using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    [SerializeField] Dialog dialog;

    private ICommand interactCommand;
    private void Awake()
    {
        interactCommand = new DialogCommand(dialog);
    }
    public void Interact()
    {
        StartCoroutine(interactCommand.Execute());
    }
}
