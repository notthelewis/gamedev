
using System;
using System.Linq;
using UnityEngine;

[Flags]
public enum State : byte
{
    Idle        = 0,  
    Moving      = 1 << 0,
    Crouching   = 1 << 1,
    Jumping     = 1 << 2,
    Falling     = 1 << 3,
    Slamming    = 1 << 4,
}

public class PlayerState : FSM<State>
{
    private readonly IPlayer _player;

    private State currentStates = State.Idle;
    private State previousStates = State.Idle;

    private static readonly State[] AllFlags =
        Enum.GetValues(typeof(State))
            .Cast<State>()
            .Where(s =>
            {
                int val = (int)s;
                return val != 0 && (val & (val - 1)) == 0;
            })
            .ToArray();
    
    public PlayerState(IPlayer player)
    {
        _player = player;
    }

    public bool Has(State state) => (currentStates & state) != 0;

    public void Set(State state)
    {
        if (!Has(state))
        {
            previousStates = currentStates;
            currentStates |= state;
            OnStateEnter(state);
        } 
    }

    public void Toggle(State state)
    {
        if (Has(state))
            Clear(state);
        else
            Set(state);
    }

    public void Clear(State state)
    {
        if (Has(state))
        {
            previousStates = currentStates;
            currentStates &= ~state; 
            OnStateLeave(state);
        }
    }

    public void ClearAll(State state)
    {
        foreach (State s in Enum.GetValues(typeof(State)))
        {
            Clear(s);
        }
    }

    public new void Tick()
    {
        State flags = currentStates & ~State.Idle;
        
        for (int i = 0; i < AllFlags.Length; i++)
        {
            State s = AllFlags[i];
            if ((flags & s) != 0)
            {
                OnStateUpdate(s);
            }
        }
    }

    protected override void OnStateEnter(State state)
    {
        Debug.Log($"Enter: {state}");

        switch (state) 
        {
            case State.Moving:
                _player.Move();
            break;

            case State.Crouching:
                if (Has(State.Jumping) || Has(State.Falling))
                {
                    Debug.Log("crouch while jump");
                    Clear(State.Crouching);
                } else
                {
                    _player.Crouch();
                }
            break;

            case State.Jumping:
                _player.CheckJump();
            break;
        }
    }

    protected override void OnStateLeave(State state)
    {
        Debug.Log($"Exit: {state}");
        switch (state)
        {
            case State.Crouching:
                _player.Stand();
            break;

            case State.Jumping:
                if (!_player.IsGrounded()) {
                    Set(State.Falling);
                }
            break;
        }
    }

    protected override void OnStateUpdate(State state)
    {
        switch (state)
        {
            case State.Moving:
                _player.Move();
            break;

            case State.Crouching:
            break;

            case State.Jumping:
                if (_player.IsGrounded())
                {
                    Clear(State.Falling);
                } else
                {

                }
                    break;

            case State.Falling:
                if (_player.IsGrounded())
                {
                    Clear(State.Falling);
                }
            break;
        }
    }
}
