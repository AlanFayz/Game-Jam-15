using Godot;
using System;

public partial class main : Node
{
	public AudioManager audioManager;
	

	public override void _Ready()
	{
		GD.Print("Start");
		GD.Print("Mr max man");
	}
	public override void _Process(double delta)
	{
	}

	public void OnPlayerPotionThrow(Vector2 Pos, Vector2 Dir, float Speed)
	{
		var potion = ResourceLoader.Load<PackedScene>("res://Scenes/Projectiles/Potion.tscn").Instantiate() as Potion;
		potion.Position = Pos;
		potion.Speed = Speed;
		potion.Direction = Dir;
		GetNode<Node2D>("Projectiles").AddChild(potion);
		potion.Connect("PotionBreak", new Callable(this, MethodName.OnPotionPotionBreak));
	}

	public void OnPotionPotionBreak(Vector2 Pos)
	{
		PotionPool potionPool = ResourceLoader.Load<PackedScene>("res://Scenes/Projectiles/PotionPool.tscn").Instantiate() as PotionPool;
		potionPool.Position = Pos;
		GetNode<Node2D>("Projectiles").AddChild(potionPool);
	}
}
