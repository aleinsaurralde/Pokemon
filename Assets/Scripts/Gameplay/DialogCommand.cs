using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogCommand : ICommand
{
    private Dialog dialog;

    public DialogCommand (Dialog dialog)
    {
        this.dialog = dialog;
    }

    public IEnumerator Execute()
    {
        yield return DialogManager.Instance.ShowDialog(dialog);
    }
}
