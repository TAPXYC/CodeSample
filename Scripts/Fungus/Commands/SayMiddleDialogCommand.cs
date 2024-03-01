using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;

[CommandInfo("Narrative", "Say middle", "Вызывает диалог посередине экрана")]
public class SayMiddleDialogCommand : Command
{
    [SerializeField] protected string header = "...";
    // Removed this tooltip as users's reported it obscures the text box
    [TextArea(5, 10)]
    [SerializeField] protected string text = "";

    [Tooltip("Voiceover audio to play when writing the text")]
    [SerializeField] protected AudioClip voiceOverClip;

    [Tooltip("Always show this Say text when the command is executed multiple times")]
    [SerializeField] protected bool showAlways = true;

    [Tooltip("Number of times to show this Say text when the command is executed multiple times")]
    [SerializeField] protected int showCount = 1;

    [Tooltip("Type this text in the previous dialog box.")]
    [SerializeField] protected bool extendPrevious = false;

    [Tooltip("Fade out the dialog box when writing has finished and not waiting for input.")]
    [SerializeField] protected bool fadeWhenDone = true;

    [Tooltip("Wait for player to click before continuing.")]
    [SerializeField] protected bool waitForClick = true;



    //add wait for vo that overrides stopvo

    [Tooltip("Sets the active Say dialog with a reference to a Say Dialog object in the scene. All story text will now display using this Say Dialog.")]
    [SerializeField] protected SayDialog setSayDialog;

    protected int executionCount;

    #region Public members

    public override void OnEnter()
    {
        if (!showAlways && executionCount >= showCount)
        {
            Continue();
            return;
        }

        executionCount++;

        if (setSayDialog != null)
        {
            SayDialog.ActiveSayDialog = setSayDialog;
        }
        
        var sayDialog = GameManager.Story.UIController.CenterDialog;
        if (sayDialog == null)
        {
            Continue();
            return;
        }

        var flowchart = GetFlowchart();

        sayDialog.SetActive(true);

        sayDialog.SetCharacter(null);
        sayDialog.SetCharacterImage(null);

        string displayText = text;

        var activeCustomTags = CustomTag.activeCustomTags;
        for (int i = 0; i < activeCustomTags.Count; i++)
        {
            var ct = activeCustomTags[i];
            displayText = displayText.Replace(ct.TagStartSymbol, ct.ReplaceTagStartWith);
            if (ct.TagEndSymbol != "" && ct.ReplaceTagEndWith != "")
            {
                displayText = displayText.Replace(ct.TagEndSymbol, ct.ReplaceTagEndWith);
            }
        }

        string subbedText = flowchart.SubstituteVariables(displayText);

        sayDialog.Say(header, subbedText, !extendPrevious, waitForClick, fadeWhenDone, voiceOverClip, Continue);
    }

    public override string GetSummary()
    {
        return header + ": \"" + text + "\"";
    }

    public override Color GetButtonColor()
    {
        return new Color32(184, 210, 235, 255);
    }

    public override void OnReset()
    {
        executionCount = 0;
    }

    public override void OnStopExecuting()
    {
        var sayDialog = SayDialog.GetSayDialog();
        if (sayDialog == null)
        {
            return;
        }

        sayDialog.Stop();
    }

    #endregion
}
