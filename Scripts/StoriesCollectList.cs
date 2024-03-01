using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu]
public class StoriesCollectList : ScriptableObject
{
    [SerializeField] AssetReferenceT<StoryDataInfo>[] storyInfosAsset;

    public AssetReferenceT<StoryDataInfo>[] StoryInfosAsset => storyInfosAsset;

    public StoryDataInfo[] StoryInfos
    {
        get;
        private set;
    }

    private async void LoadItems(IEnumerable<AsyncOperationHandle<StoryDataInfo>> handles)
    {
        Debug.LogError(handles.Count());
        await Task.WhenAll(handles.Select(ao => ao.Task));

        Debug.LogError("Complete");
        StoryInfos = handles.Select(h => h.Result).ToArray();
    }


    public void ReleaseAssets()
    {
        foreach (var si in storyInfosAsset)
        {
            si.ReleaseAsset();
        }
    }

    public AsyncOperationHandle<StoryDataInfo>[] GetStoriesDatas()
    {
        if (StoryInfos.IsNullOrEmpty())
        {
            List<AsyncOperationHandle<StoryDataInfo>> asyncStoryDataInfoHandles = new List<AsyncOperationHandle<StoryDataInfo>>();
            foreach (var si in storyInfosAsset)
            {
                AsyncOperationHandle<StoryDataInfo> asyncStoryDataInfoHandle = si.LoadAssetAsync();
                asyncStoryDataInfoHandles.Add(asyncStoryDataInfoHandle);
            }
            LoadItems(asyncStoryDataInfoHandles);
            return asyncStoryDataInfoHandles.ToArray();
        }
        else
        {
            return null;
        }
    }
}
