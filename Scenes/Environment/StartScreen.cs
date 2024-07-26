using Godot;
using System;

public partial class StartScreen : Node2D
{
	StartMenu Menu;
    public override void _Ready()
    {
        Menu = GetNode<StartMenu>("StartMenu");
    }
    public void OnStartMenuQuitGame()
	{
		GetTree().Quit();
	}
	public void OnStartMenuStartGame()
	{
		Menu.Loading();
		GetTree().ChangeSceneToFile("res://Scenes/Environment/Main.tscn");
	}
}
