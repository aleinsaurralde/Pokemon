using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] int lettersPerSecond;
    public event Action OnShowDialog;
    public event Action OnCloseDialog;
    public static DialogManager Instance { get; private set;  }



    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    Dialog dialog;
    Action onDialogFinished;
    int currentLine = 0;
    bool isTyping;

    public IEnumerator ShowDialog(Dialog dialog, Action onFinished = null)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();

        this.dialog = dialog;
        onDialogFinished = onFinished;

        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public IEnumerator TypeDialog(string dialog)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping=false;
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z) )
        {
            if (!isTyping)
            {
                ++currentLine;
                if (currentLine < dialog.Lines.Count)
                {
                    StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
                }
                else
                {
                    currentLine = 0;
                    dialogBox.SetActive(false);
                    onDialogFinished?.Invoke();
                    OnCloseDialog?.Invoke();
                }
            }
            else
            {
                FinishCurrentLine(currentLine);
            }
        }
    }
    public void FinishCurrentLine(int currentLine) 
    {
        StopAllCoroutines(); // stop the typing coroutine
        dialogText.text = dialog.Lines[currentLine]; // instantly show full line
        isTyping = false; // mark typing as finished
    }
}
