using Godot;
using System;

public partial class StartScreen : Node2D
{
	public StartMenu Menu;
	public PackedScene GameScene;
	public override void _Ready()
	{
		Menu = GetNode<StartMenu>("StartMenu");
		GameScene = ResourceLoader.Load<PackedScene>("res://Scenes/Environment/Main.tscn");
	}
	public void OnStartMenuQuitGame()
	{
		GetTree().Quit();
	}
	public void OnStartMenuStartGame()
	{
		Menu.Loading();
		GetTree().ChangeSceneToPacked(GameScene);
	}
}
