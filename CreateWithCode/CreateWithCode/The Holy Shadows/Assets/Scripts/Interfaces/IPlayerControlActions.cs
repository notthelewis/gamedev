using System.Collections;
using UnityEngine;

// This interface exists to prevent cyclic dependency between PlayerController & PlayerStateMachine

public interface IPlayerControlActions
{
    IEnumerator Move();
    void UpdateWalk();
    void UpdateJump();
    void UpdateFall();
    void UpdateSlam();
    void EndCrouch();
    void EndWalk(); 
}
