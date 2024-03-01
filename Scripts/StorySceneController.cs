using System;
using UnityEngine;


/// <summary>
/// ВНИМАНИЕ! Данная сцена будет перезагружаться
/// </summary>
public class StorySceneController : MonoBehaviour
{
    [SerializeField] Transform baseStory;

    public StoryController Story
    {
        get;
        private set;
    }

    void Awake()
    {
        GameManager.Inst.SetStoryScene(this);
    }


    public void LoadStory(StoryDataInfo storyDataInfo, ItemCollection items, StoryController storyPrefab, Action onComplete)
    {
        baseStory.gameObject.SetActive(false);
        Story = Instantiate(storyPrefab, baseStory);
        baseStory.gameObject.SetActive(true);
        Story.Init(storyDataInfo.StoryDataHandler, items, onComplete, storyDataInfo.EndScreenBackground);
    }


    public void Clear()
    {
        Destroy(gameObject);
    }
}
