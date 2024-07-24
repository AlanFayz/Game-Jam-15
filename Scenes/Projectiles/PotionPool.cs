using Godot;
using System;

public partial class PotionPool : Area2D
{
	public float Damage;
	public void OnTimerTimeout()
	{
		QueueFree();
	}

	public void OnBodyEntered(Node2D body)
	{
		if (body is IHittable target)
		{
			target.Hit(this, Damage);
		}
	}
}
