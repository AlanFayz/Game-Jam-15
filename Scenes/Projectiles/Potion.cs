using Godot;
using System;
using System.ComponentModel;

public partial class Potion : Area2D
{
	[Signal]
	public delegate void PotionBreakEventHandler(Vector2 Pos, float poolDamage);


	public Vector2 Direction;
	public float Speed;
	public float PoolDamage;
	public float BreakDamage;



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
			target.Hit(this, BreakDamage);
		}
		Break();
	}

	public void Break()
	{
		EmitSignal(SignalName.PotionBreak, GlobalPosition, PoolDamage);
		QueueFree();
	}
}
