using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YieldSimpleEvent
{
   private bool _eventHappened = false;

    public IEnumerator WaitForEvent()
    {
        yield return new WaitUntil(() => _eventHappened);
        _eventHappened = false;
    }

    public void Invoke()
    {
        _eventHappened = true;
    }
}
