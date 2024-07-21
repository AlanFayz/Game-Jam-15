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
		var PotionScene = ResourceLoader.Load<PackedScene>("res://Scenes/Projectiles/Potion.tscn").Instantiate();
		
	}
}
