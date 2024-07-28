using Godot;
using System;
using System.ComponentModel;
using System.Linq;

public partial class Potion : Area2D
{
	[Signal]
	public delegate void PotionBreakEventHandler(Vector2 Pos, float poolDamage, int[] potionType);


	public Vector2 Direction;
	public float Speed;
	public float PoolDamage;
	public float BreakDamage;
	public int[] PotionType;
	


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
			target.Hit(this, BreakDamage, new int[3]);
		}
		Break();
	}

	public void Break()
	{
		EmitSignal(SignalName.PotionBreak, GlobalPosition, PoolDamage, PotionType);
		QueueFree();
	}
}
