using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable] class ContentAndPoints
{
    public GameObject textContent;
    
    [Tooltip("You can do not touch this")]
    public Point point;
}

public class ContentChanger : MonoBehaviour
{
    [SerializeField] Point point;
    [SerializeField] Transform pointContainer;
    [SerializeField] List<ContentAndPoints> contentAndPoints;
    [Space]
    [SerializeField] int changeTime;

    private int _currentObjectIndex = 0;

    void Start()
    {
        SpawnPoints();        
        contentAndPoints[_currentObjectIndex].textContent.SetActive(true);
        ActivatePoint();

        StartCoroutine(ChangeContent());
    }

    IEnumerator ChangeContent()
    {
        while(true)
        {
            yield return new WaitForSeconds(changeTime);
            SetNextContent();
        }
    }

    void SpawnPoints()
    {
        for(int i = 0; i < contentAndPoints.Count; i++)
        {
            contentAndPoints[i].textContent.SetActive(false);
            Point p = Instantiate(contentAndPoints[i].point, pointContainer);
            p.Init();
            contentAndPoints[i].point = p;
        }
    }

    void ReturnNextObjectIndex()
    {
        _currentObjectIndex++;
        if (_currentObjectIndex >= contentAndPoints.Count)
        {
            _currentObjectIndex = 0;
        }
    }

    void ReturnPreviousObjectIndex()
    {
        _currentObjectIndex--;
        if (_currentObjectIndex < 0)
        {
            _currentObjectIndex = contentAndPoints.Count - 1;
        }
    }

    public void SetNextContentClick()
    {
        SetNextContent();
        RestartCoroutine();
    }

    public void SetPreviousContentClick()
    {
        SetPreviousContent();
        RestartCoroutine();
    }

    void SetNextContent()
    {
        contentAndPoints[_currentObjectIndex].textContent.SetActive(false);
        DeactivatePoint();
        ReturnNextObjectIndex();
        contentAndPoints[_currentObjectIndex].textContent.SetActive(true);
        ActivatePoint();
    }

    void SetPreviousContent()
    {
        contentAndPoints[_currentObjectIndex].textContent.SetActive(false);
        DeactivatePoint();
        ReturnPreviousObjectIndex();
        contentAndPoints[_currentObjectIndex].textContent.SetActive(true);
        ActivatePoint();
    }

    void ActivatePoint()
    {
        contentAndPoints[_currentObjectIndex].point.SetPointActive();
    }

    void DeactivatePoint()
    {
        contentAndPoints[_currentObjectIndex].point.SetPointInactive(); 
    }

    void RestartCoroutine()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeContent());
    }
}
