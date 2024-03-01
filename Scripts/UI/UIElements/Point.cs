using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Point : MonoBehaviour
{
    [SerializeField] Color activeColor;
    [SerializeField] Color inactiveColor;
    [SerializeField] Image image;

    public void Init()
    {
        SetPointInactive();
    }

    public void SetPointActive()
    {
        image.color = activeColor;
    }

    public void SetPointInactive()
    {
        image.color = inactiveColor;
    }
}
