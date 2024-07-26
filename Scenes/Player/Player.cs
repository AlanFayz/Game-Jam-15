using Godot;
using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

public partial class Player : CharacterBody2D, IHittable
{
	[Signal]
	public delegate void PotionThrowEventHandler(Vector2 Pos, Vector2 Dir, float speed, float breakDamage, float poolDamage);
	[Signal]
	public delegate void SlashEventHandler(Vector2 Pos, Vector2 Dir, float slashDamage, String slashType);
	[Signal]
	public delegate void PlayerDeathEventHandler();

	
	float PlayerSpeed = 50000f;

	bool IsWalking = false;

	bool IsDying = false;


	private float health = 100f;
	public float Health 
	{
		get {return health;} 
		set 
		{
			health = value;
			CheckDeath();
		}
	}

	bool IsImmune = false;
	



	bool CanThrow = true;
	float ThrowSpeed = 400f;
	float BreakDamage = 5f;
	float PoolDamage = 30f;


	float[] SlashDistances = {15f,30f};
	bool CanSlash = true;
	float SlashDamage = 10f;
	string[] Slashes = {"res://Scenes/Melee/Slashes/Slash1.tscn", "res://Scenes/Melee/Slashes/Slash2.tscn"};



	//Node references
	Timer ThrowCooldown;
	Timer SlashCooldown;
	Timer ImmunityFrames;
	AnimationPlayer Animation;

	public override void _Ready()
	{
		ThrowCooldown = GetNode<Timer>("ThrowCooldown");
		SlashCooldown = GetNode<Timer>("SlashCooldown");
		ImmunityFrames = GetNode<Timer>("ImmunityFrames");
		Animation = GetNode<AnimationPlayer>("AnimationPlayer");


	}

	public override void _Process(double delta)
	{
		if (IsDying) {return;}

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
		if (CanSlash && Input.IsActionPressed("primary_attack"))
		{
			SlashAttack();
		}

	}

	public void OnSlashCooldownTimeout()
	{
		CanSlash = true;
	}

	public void ThrowPotion()
	{
		CanThrow = false;
		ThrowCooldown.Start();
		EmitSignal(SignalName.PotionThrow, GlobalPosition, GetLocalMousePosition().Normalized(),ThrowSpeed, BreakDamage, PoolDamage);
	}

	public void OnThrowCooldownTimeout()
	{
		CanThrow = true;
	}

	public void Hit(Node Origin, float damage)
	{
		if (!IsImmune)
		{
			Health -= damage;
			GD.Print($"Health = {Health}");
			IsImmune = true;
			ImmunityFrames.Start();
		}
	}

	public void OnImmunityFramesTimeout()
	{
		IsImmune = false;
	}

	public void SlashAttack()
	{
		CanSlash = false;
		SlashCooldown.Start();
		Vector2 mouseDir = GetLocalMousePosition().Normalized();
		int slashType = Math.Abs((int)GD.Randi())%2;
		Vector2 LocalSlashLocation = mouseDir*SlashDistances[slashType];
		EmitSignal(SignalName.Slash, GlobalPosition+LocalSlashLocation, mouseDir, SlashDamage, Slashes[slashType]);
	}

	public void CheckDeath()
	{
		if (health <= 0)
		{
			StartDeath();
		}
	}

	public void StartDeath()
	{
		IsDying = true;
		Animation.Play("Death", 0, 0.25f, false);
	}
	public void EndDeath()
	{
		EmitSignal(SignalName.PlayerDeath);

		IsDying = false; //For testing, remove this in final ver
		Health = 100f;
	}
}