using Godot;
using System;
using System.ComponentModel;

public partial class Potion : Area2D
{
	[Signal]
	public delegate void PotionBreakEventHandler(Vector2 Pos);

	public Vector2 Direction;
	public float Speed;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		Position += Direction*Speed*(float)delta;
	}

	public void OnBreakTimerTimeout()
	{
		Break();
	}

	public void OnBodyEntered(Node2D body)
	{
		if (body is IHittable target)
		{
			target.Hit(this);
		}
		Break();
	}

	public void Break()
	{
		EmitSignal(SignalName.PotionBreak, GlobalPosition);
		QueueFree();
	}
}
