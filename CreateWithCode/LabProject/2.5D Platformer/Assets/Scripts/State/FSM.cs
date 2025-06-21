using System;

public abstract class FSM<T>
{
    protected bool _init = false;
    protected T currentState;
    protected T prevState;

    protected event Action<T> OnStateChange;
    protected abstract void OnStateEnter(T state);
    protected abstract void OnStateLeave(T state);
    protected abstract void OnStateUpdate(T state);

    public void Init(T defaultState)
    {
        if (_init) return;
        _init = true;
        currentState = defaultState;
        prevState = defaultState;
    }

    public void ChangeState(T state)
    {
        if (!_init) throw new InvalidOperationException("Tried changing state on uninitialized FSM");
        if (state.Equals(this.currentState)) return;

        prevState = currentState;
        currentState = state;

        OnStateLeave(prevState);
        OnStateEnter(state);
        OnStateChange?.Invoke(state);
    }

    public void Tick()
    {
        if (!_init) return;
        OnStateUpdate(this.currentState);
    }
}
