using Godot;
using System;

public partial class FireBolt : Area2D
{
	public float Damage;
	public float Speed;
	public Vector2 Direction;


	public override void _Ready()
	{
		LookAt(Direction+GlobalPosition);
		
	}


	public override void _Process(double delta)
	{
		Position += Direction*Speed*(float)delta;
	}

	public void OnTimerTimeout()
	{
		QueueFree();
	}

	public void OnBodyEntered(Node2D body)
	{
		if (body is IHittable target)
		{
			target.Hit(this, Damage, new int[]{0,0,1});
		}
		QueueFree();
	}
}
