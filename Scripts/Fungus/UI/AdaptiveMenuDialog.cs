using System.Collections;
using System.Linq;
using Fungus;
using Gravitons.UI.Modal;
using UnityEngine;
using UnityEngine.EventSystems;

public class AdaptiveMenuDialog : MenuDialog
{
    [SerializeField] RectTransform baseButtonGroupIdlePosition;
    [SerializeField] RectTransform baseButtonGroup;

    private MenuButton_custom[] _menuButtons;
    private StoryDiamondCountUI _diamondCountUI;
    private bool _isClear = false;
    private bool _isInited = false;

    protected override void Awake()
    {
        Init();
    }


    public void Init()
    {
        if (!_isInited)
        {
            base.Awake();
            _isInited = true;
            gameObject.SetActive(false);
            _menuButtons = cachedButtons.Select(cb => cb.GetComponent<MenuButton_custom>()).ToArray();
        }
    }


    public override void Clear()
    {
        //Debug.LogError("CLEAR ");
        if (_diamondCountUI != null)
            _diamondCountUI.Hide();

        _isClear = true;
        base.Clear();
        baseButtonGroup.SetParent(baseButtonGroupIdlePosition);
    }


    private void ButtonClick(MenuButton_custom sender, Block block)
    {
        if (sender.DiamondsCount > 0)
        {
            if (_diamondCountUI.GetDiamondCount() >= sender.DiamondsCount)
            {
                _diamondCountUI.ChangeDiamond(-sender.DiamondsCount);
                GameManager.Story.AddMenuID(sender.ID);
            }
            else
            {
                //show message
                ModalManager.Show("Недостаточно алмазов", "Перейдите в магазин покупок.", new ModalButton[] { new ModalButton("Ок") });
                return;
            }
        }

        EventSystem.current.SetSelectedGameObject(null);
        StopAllCoroutines();
        // Stop timeout
        Clear();
        HideSayDialog();

        if (block != null)
        {
            var flowchart = block.GetFlowchart();
            gameObject.SetActive(false);
            flowchart.StartCoroutine(CallBlock(block));
        }
    }


    public override bool AddOption(string text, bool interactable, bool hideOption, Block targetBlock)
    {
        Init();
        _diamondCountUI = GameManager.Story.UIController.DiamondCountUI;
        DropdownSayDialog ddDialog = SayDialog.GetSayDialog() as DropdownSayDialog;
        MenuButton_custom button = _menuButtons[nextOptionIndex];

        if (text.Contains("diamond$="))
        {
            _diamondCountUI.Show(ddDialog.DiamondCountPosition);

            string diamondParamString = text.Split('\n')[0];
            string diamondValue = diamondParamString.Split('=')[1];
            int requreDiamonds = int.Parse(diamondValue);
            text = text.Remove(0, diamondParamString.Length + 1);

            string menuID = $"{targetBlock.BlockName}{requreDiamonds}{new string(text.Split(new char[]{' ', '\t', '\n', '\r'}, System.StringSplitOptions.RemoveEmptyEntries).Select(w => w[0]).ToArray())}";

            if (GameManager.Story.HasMenuID(menuID))
            {
                button.SetSimpleMode();
            }
            else
            {
                button.SetCristalMode(requreDiamonds, _diamondCountUI.GetDiamondCount(), menuID);
            }
        }
        else
        {
            button.SetSimpleMode();
        }

        SetPosition(ddDialog);

        var block = targetBlock;
        UnityEngine.Events.UnityAction clickAction = delegate { ButtonClick(button, block); };

        return base.AddOption(text, interactable, hideOption, clickAction);
    }


    void SetPosition(DropdownSayDialog ddDialog)
    {
        if (ddDialog != null && _isClear)
        {
            _isClear = false;
            //Debug.LogError("Move to position " + ddDialog.DownPosition);
            baseButtonGroup.SetParent(ddDialog.DownPoint);
            baseButtonGroup.localPosition = Vector3.zero;
            ddDialog.SetClickAreaActive(false);
        }
    }
}
