using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoryUIHandler : MonoBehaviour
{
    [SerializeField] Image storyImage;
    [SerializeField] Image tittleImage;
    [SerializeField] TextMeshProUGUI storyName;
    [SerializeField] TextMeshProUGUI storyTags;
    [Space]
    [SerializeField] Button clickButton;

    public event Action<StoryDataInfo> OnStoryClick;

    public StoryDataInfo HandledStory
    {
        get;
        private set;
    }

    public void Init(StoryDataInfo handledStory)
    {
        HandledStory = handledStory;
        storyImage.sprite = HandledStory.Image;
        tittleImage.sprite = HandledStory.Header;
        storyName.text = HandledStory.StoryName;
        storyTags.text = string.Join(", ", HandledStory.Tags);

        clickButton.onClick.AddListener(Click);
    }

    private void Click()
    {
        OnStoryClick?.Invoke(HandledStory);
    }
}
