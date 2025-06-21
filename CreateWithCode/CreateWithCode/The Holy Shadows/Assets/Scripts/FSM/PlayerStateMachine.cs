using System;
using UnityEngine;

public enum PlayerState : byte
{
    Idle,
    Walking,
    Jumping,
    Falling, 
    Crouching,
    Dashing,
    Slamming
}

public class PlayerStateMachine : FSM<PlayerState>
{
    private readonly IPlayerControlActions player;

    public PlayerStateMachine(IPlayerControlActions player)
    {
        this.player = player;
    }

    protected override void OnEnterState(PlayerState state)
    {
        Debug.Log($"ENTER STATE: {state}");
        switch (state)
        {
            case PlayerState.Idle:

            break;

            case PlayerState.Walking:

            break;

            case PlayerState.Jumping:
            break;

            case PlayerState.Falling:
            break;

            case PlayerState.Crouching:
            break;

            case PlayerState.Dashing:
            break; 

            case PlayerState.Slamming:
            break; 
        }

    }

    protected override void OnExitState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Crouching:
                player.EndCrouch();
            break;

            case PlayerState.Walking:
                player.EndWalk();
            break;
        }
    }

    protected override void OnUpdateState(PlayerState state)
    {
        if (state == PlayerState.Idle) {
            return;
        }

        Debug.Log($"FSM STATE: {state}");

        switch (state)
        {
            case PlayerState.Walking:
                Debug.Log("Walk");
                player.Move();
            break;

            case PlayerState.Jumping:
                Debug.Log("Jump");
                player.UpdateJump();
            break;

            case PlayerState.Falling:
                Debug.Log("Fall");
                player.UpdateFall();
            break;

            case PlayerState.Slamming:
                Debug.Log("Slam");
                player.UpdateSlam();
            break;
        }
    }
}
