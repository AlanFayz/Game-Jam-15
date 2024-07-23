using Godot;
using System;

public partial class Slash1 : Area2D
{
	public Vector2 AttackDir;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LookAt(AttackDir);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SlashEnd()
	{
		QueueFree();
	}
}
