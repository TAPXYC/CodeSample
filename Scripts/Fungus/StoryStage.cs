using Fungus;
using UnityEngine;

public class StoryStage : Stage
{
    [SerializeField] MainCharacterController mainCharacterVisual;
    [SerializeField] Character mainCharacter;


    public MainCharacterController MainCharacter => mainCharacterVisual;

    private bool _isInit = false;


    public void Init(ISelectableInfo[] defaultItems)
    {
        if (!_isInit)
        {
            mainCharacterVisual.Init(mainCharacter, defaultItems);
            mainCharacterVisual.ForceFade(0);

            mainCharacter.State.holder.transform.SetParent(stage.PortraitCanvas.transform, false);
            SetRectTransform(mainCharacter.State.holder, stage.DefaultPosition.GetComponent<RectTransform>());

            _isInit = true;
        }
    }


    protected override void CreatePortraitObject(Character character, float fadeDuration)
    {
        if (character == mainCharacter)
            ShowMainCharacter(character, fadeDuration);
        else
            base.CreatePortraitObject(character, fadeDuration);
    }


    private void ShowMainCharacter(Character character, float fadeDuration)
    {
        /*
        
                    if (item == character.ProfileSprite)
                    {
                        character.State.portraitImage = pi;
                    }
        }*/
    }
}
