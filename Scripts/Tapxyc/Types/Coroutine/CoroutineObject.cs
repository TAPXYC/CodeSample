using System;
using System.Collections;
using UnityEngine;


public sealed class CoroutineObject : CoroutineObjectBase
{
    public Func<IEnumerator> Routine { get; private set; }

    public override event Action Finished;

    public CoroutineObject(MonoBehaviour owner, Func<IEnumerator> routine)
    {
        Owner = owner;
        Routine = routine;
    }

    private IEnumerator Process()
    {
        yield return Routine.Invoke();

        Coroutine = null;

        Finished?.Invoke();
    }

    public void Start()
    {
        Stop();

        Coroutine = Owner.StartCoroutine(Process());
    }

    public void Stop()
    {
        if(IsProcessing)
        {
            Owner.StopCoroutine(Coroutine);

            Coroutine = null;
        }
    }
}


public class CoroutineObject<T> : CoroutineObjectBase
{
    public Func<T, IEnumerator> Routine { get; private set; }

    public override event Action Finished;

    public CoroutineObject(MonoBehaviour owner, Func<T, IEnumerator> routine)
    {
        Owner = owner;
        Routine = routine;
    }

    private IEnumerator Process(T arg)
    {
        yield return Routine.Invoke(arg);

        Coroutine = null;

        Finished?.Invoke();
    }

    public void Start(T arg)
    {
        Stop();

        Coroutine = Owner.StartCoroutine(Process(arg));
    }

    public void Stop()
    {
        if (IsProcessing)
        {
            Owner.StopCoroutine(Coroutine);

            Coroutine = null;
        }
    }
}



public class CoroutineObject<T, U> : CoroutineObjectBase
{
    public Func<T, U, IEnumerator> Routine { get; private set; }

    public override event Action Finished;

    public CoroutineObject(MonoBehaviour owner, Func<T, U, IEnumerator> routine)
    {
        Owner = owner;
        Routine = routine;
    }

    private IEnumerator Process(T arg1, U arg2)
    {
        yield return Routine.Invoke(arg1, arg2);

        Coroutine = null;

        Finished?.Invoke();
    }

    public void Start(T arg1, U arg2)
    {
        Stop();
        Coroutine = Owner.StartCoroutine(Process(arg1, arg2));
    }

    public void Stop()
    {
        if (IsProcessing)
        {
            Owner.StopCoroutine(Coroutine);

            Coroutine = null;
        }
    }
}
