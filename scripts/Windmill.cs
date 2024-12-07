using Godot;
using System;

public partial class Windmill : Node3D
{
	// Called when the node enters the scene tree for the first time.
	
	private AnimationPlayer _animationPlayer;
	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_animationPlayer.Play("rotate");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
