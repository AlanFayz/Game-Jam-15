using Godot;
using System;

public partial class BaseSlash : Area2D
{
	public Vector2 AttackDir;
	public float Damage;
	protected float SlashSpeed;

	protected virtual float SetSpeed()
	{
		return 3f;
	}

	public override void _Ready()
	{
		SlashSpeed = SetSpeed();

		LookAt(AttackDir+GlobalPosition);
		if (AttackDir.X<0)
		{
			Vector2 scale = Scale;
			scale.Y *= -1;
			Scale = scale;
		}

		
		if (GD.Randi()%2==0)
		{
			GetNode<AnimationPlayer>("AnimationPlayer").Play("Slash", -1, SlashSpeed);
		}
		else
		{
			GetNode<AnimationPlayer>("AnimationPlayer").Play("UpSlash", -1, SlashSpeed);
		}
	}

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
