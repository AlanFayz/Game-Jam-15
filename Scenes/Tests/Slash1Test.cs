using Godot;
using System;

public partial class Slash1Test : Area2D
{
	public Vector2 AttackDir;
	public float Damage;

	public override void _Ready()
	{
		LookAt(AttackDir+GlobalPosition);
		if (AttackDir.X<0)
		{
			Vector2 scale = Scale;
			scale.Y *= -1;
			Scale = scale;
		}

		
		if (GD.Randi()%2==0)
		{
			GetNode<AnimationPlayer>("AnimationPlayer").Play("Slash", -1, 3);
		}
		else
		{
			GetNode<AnimationPlayer>("AnimationPlayer").Play("UpSlash", -1, 3);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void OnBodyEntered(Node2D body)
	{
		if (body is IHittable target)
		{
			target.Hit(this, Damage);
		}
	}

	public void SlashEnd()
	{
		QueueFree();
	}
}
