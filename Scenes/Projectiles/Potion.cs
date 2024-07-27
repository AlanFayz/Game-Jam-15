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
	public int[] PotionEffects = new int[3];


    public override void _Ready()
    {
		for (int i = 2; i<5; i++)
		{
        	PotionEffects[i-2] = PotionType[i];
		}
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
			target.Hit(this, BreakDamage, PotionEffects);
		}
		Break();
	}

	public void Break()
	{
		EmitSignal(SignalName.PotionBreak, GlobalPosition, PoolDamage, PotionType);
		QueueFree();
	}
}
