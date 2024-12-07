using Godot;
using System;

public partial class Player : CharacterBody3D 
{
    private const float InitialSpeed = 2.0f;
    private const float MaxSpeed = 15.0f;
    private const float Acceleration = 8.0f;
    private const float MaxAcceleration = 8.0f;
    private const float JumpVelocity = 15f;
    private Vector3 _gravity = new Vector3(0, -30.8f, 0);

    private float _currentSpeed;
    private int _jumpCount = 0;
    private const int MaxJumps = 2;
    
    public override void _Ready()
    {
        _currentSpeed = InitialSpeed;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;
        
        // Apply gravity
        if (!IsOnFloor())
        {
            velocity += _gravity * (float)delta;
        }

        // Reset jump count when on the floor
        if (IsOnFloor())
        {
            _jumpCount = 0;
        }

        // Handle forward and backward movement with acceleration
        float inputDirection = Input.GetActionStrength("back") - Input.GetActionStrength("forward");
        
        if (inputDirection != 0)
        {
            float accelerationThisFrame = Acceleration * (float)delta;
            accelerationThisFrame = Mathf.Min(accelerationThisFrame, MaxAcceleration * (float)delta);
            
            _currentSpeed += accelerationThisFrame;
            _currentSpeed = Mathf.Clamp(_currentSpeed, InitialSpeed, MaxSpeed);
            velocity.Z = inputDirection * _currentSpeed;
        }
        else
        {
            _currentSpeed = InitialSpeed;
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, _currentSpeed * (float)delta);
        }
        
        // Handle jumping
        if (Input.IsActionJustPressed("jump") && _jumpCount < MaxJumps)
        {
            velocity.Y = JumpVelocity;
            _jumpCount++;
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}