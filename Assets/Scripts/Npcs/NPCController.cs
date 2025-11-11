using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    [SerializeField] Dialog dialog;

    public void Interact()
    {
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
    }
}
