using System.Collections.Generic;
using Fungus;
using UnityEngine;
using UnityEngine.Serialization;

[CommandInfo("Narrative", "Menu с кристаллами", "Отобразит кнопку выбора за кристаллы")]
public class MenuDiamondCommand : Command, ILocalizable, IBlockCaller
{
    [Tooltip("Количество кристаллов, необходимых для выбора варианта")]
    [SerializeField] float cristallCount;
    [Space]
    [Tooltip("Text to display on the menu button")]
    [TextArea()]
    [SerializeField] protected string text = "Option Text";

    [Tooltip("Notes about the option text for other authors, localization, etc.")]
    [SerializeField] protected string description = "";

    [FormerlySerializedAs("targetSequence")]
    [Tooltip("Block to execute when this option is selected")]
    [SerializeField] protected Block targetBlock;

    [Tooltip("Hide this option if the target block has been executed previously")]
    [SerializeField] protected bool hideIfVisited;

    [Tooltip("If false, the menu option will be displayed but will not be selectable")]
    [SerializeField] protected BooleanData interactable = new BooleanData(true);

    [Tooltip("A custom Menu Dialog to use to display this menu. All subsequent Menu commands will use this dialog.")]
    [SerializeField] protected MenuDialog setMenuDialog;

    [Tooltip("If true, this option will be passed to the Menu Dialogue but marked as hidden, this can be used to hide options while maintaining a Menu Shuffle.")]
    [SerializeField] protected BooleanData hideThisOption = new BooleanData(false);


    public override void OnEnter()
    {
        if (setMenuDialog != null)
        {
            // Override the active menu dialog
            MenuDialog.ActiveMenuDialog = setMenuDialog;
        }

        bool hideOption = (hideIfVisited && targetBlock != null && targetBlock.GetExecutionCount() > 0) || hideThisOption.Value;

        var storyMenu = GameManager.Story.UIController.MenuDialog;

        var menuDialog = storyMenu ?? MenuDialog.GetMenuDialog();
        
        if (menuDialog != null)
        {
            menuDialog.SetActive(true);

            var flowchart = GetFlowchart();
            string displayText = flowchart.SubstituteVariables(text);

            menuDialog.AddOption($"diamond$={cristallCount}=\n"+displayText, interactable, hideOption, targetBlock);
        }

        Continue();
    }

    public override void GetConnectedBlocks(ref List<Block> connectedBlocks)
    {
        if (targetBlock != null)
        {
            connectedBlocks.Add(targetBlock);
        }
    }

    public override string GetSummary()
    {
        if (targetBlock == null)
        {
            return "Error: No target block selected";
        }

        if (text == "")
        {
            return "Error: No button text selected";
        }

        return text + " : " + targetBlock.BlockName;
    }

    public override Color GetButtonColor()
    {
        return new Color32(184, 210, 235, 255);
    }

    public override bool HasReference(Variable variable)
    {
        return interactable.booleanRef == variable || hideThisOption.booleanRef == variable ||
            base.HasReference(variable);
    }

    public bool MayCallBlock(Block block)
    {
        return block == targetBlock;
    }


    #region ILocalizable implementation

    public virtual string GetStandardText()
    {
        return text;
    }

    public virtual void SetStandardText(string standardText)
    {
        text = standardText;
    }

    public virtual string GetDescription()
    {
        return description;
    }

    public virtual string GetStringId()
    {
        // String id for Menu commands is MENU.<Localization Id>.<Command id>
        return "MENU." + GetFlowchartLocalizationId() + "." + itemId;
    }

    #endregion

    #region Editor caches
#if UNITY_EDITOR
    protected override void RefreshVariableCache()
    {
        base.RefreshVariableCache();

        var f = GetFlowchart();

        f.DetermineSubstituteVariables(text, referencedVariables);
    }
#endif
    #endregion Editor caches
}
