using Godot;
using System;

public partial class PotionPool : Area2D
{
	public float Damage;
	public int[] PotionType;

    public override void _Ready()
    {
        if (PotionType[0]>0)
		{
			SetCollisionLayerValue(3, true);
			SetCollisionLayerValue(6,false);
			SetCollisionMaskValue(1,false);
			Damage *= 0.7f;
		}
    }

    public void OnTimerTimeout()
	{
		QueueFree();
	}

	public void OnBodyEntered(Node2D body)
	{
		if (body is IHittable target)
		{
			target.Hit(this, Damage, new int[4]);
		}
	}
}
