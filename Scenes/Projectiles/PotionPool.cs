using Godot;
using System;

public partial class PotionPool : Area2D
{
	public float Damage;
	public int[] PotionType;
	public int[] Effects = new int[3];

    public override void _Ready()
    {
        if (PotionType[0]>0)
		{
			SetCollisionLayerValue(3, true);
			SetCollisionLayerValue(6,false);
			SetCollisionMaskValue(1,false);
			Damage *= 0.7f;
		}
		Timer timer = GetNode<Timer>("Timer");
		timer.Start(5+PotionType[1]);

		for (int i = 2; i<5; i++)
		{
        	Effects[i-2] = PotionType[i];
		}

		GD.Print($"PotionType: ");
		Console.WriteLine("[{0}]", string.Join(", ", PotionType));
		GD.Print($"PotionEffects: ");
		Console.WriteLine("[{0}]", string.Join(", ", Effects));
    }

    public void OnTimerTimeout()
	{
		QueueFree();
	}

	public void OnBodyEntered(Node2D body)
	{
		if (body is IHittable target)
		{
			target.Hit(this, Damage, Effects);
		}
	}
}
