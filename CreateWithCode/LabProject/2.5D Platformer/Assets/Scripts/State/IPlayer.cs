using UnityEngine;

public interface IPlayer
{
    public void Move();
    public void Crouch();
    public void Stand();
    public void CheckJump();
    public bool IsGrounded();
}
