using Godot;
using System;

public partial class Player : CharacterBody2D, IHittable
{
	[Signal]
	public delegate void PotionThrowEventHandler(Vector2 Pos, Vector2 Dir, float Speed);
	
	float PlayerSpeed = 50000f;

	bool IsWalking = false;

	bool CanThrow = true;
	float ThrowSpeed = 400f;

	//Node references
	Timer ThrowCooldown;
	AnimationPlayer Animation;

	public override void _Ready()
	{
		ThrowCooldown = GetNode<Timer>("ThrowCooldown");
		Animation = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _Process(double delta)
	{
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down").Normalized();
		
		if (inputDir == Vector2.Zero)
		{
			Animation.Play("idle");
		}
		else if (inputDir.X>0)
		{
			Animation.Play("walk_right");
		}
		else if (inputDir.X < 0)
		{
			Animation.Play("walk_left");
		}
		else
		{
			Animation.Play("walk_vertical");
		}
		
		Velocity = inputDir*PlayerSpeed*(float)delta;
		MoveAndSlide();

		if (CanThrow & Input.IsActionPressed("throw_potion"))
		{
			ThrowPotion();
		}
	}

	public void ThrowPotion()
	{
		CanThrow = false;
		ThrowCooldown.Start();
		EmitSignal(SignalName.PotionThrow, GlobalPosition, GetLocalMousePosition().Normalized(),ThrowSpeed);
	}

	public void OnThrowCooldownTimeout()
	{
		CanThrow = true;
	}

	public void Hit(Node Origin)
	{
		GD.Print($"Hit by {Origin}");
	}


}

