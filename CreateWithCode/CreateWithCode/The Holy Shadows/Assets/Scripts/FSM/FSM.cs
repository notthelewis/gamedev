using System;
using UnityEngine;

public abstract class FSM<T> 
{
    public T CurrentState { get; private set; }

    public event Action<T> OnStateChanged;

    private bool _init = false;

    protected abstract void OnEnterState(T state);
    protected abstract void OnExitState(T state);
    protected abstract void OnUpdateState(T state);

    public void Init(T initialState)
    {
        if (_init) throw new InvalidOperationException("FSM Already initialized..");
        _init = true;
        CurrentState = initialState;
        OnEnterState(CurrentState);
        OnStateChanged?.Invoke(initialState);
    }

    public void ChangeState(T state)
    {
        if (Equals(CurrentState, state)) return;
        OnExitState(CurrentState);
        OnEnterState(state);
        OnUpdateState(state);
        OnStateChanged?.Invoke(state);
    }

    public void Tick()
    {
        if (!_init) throw new InvalidOperationException("State machine not initialized");
        OnUpdateState(CurrentState);
    }
}
