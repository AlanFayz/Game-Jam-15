using Godot;
using System;

public partial class StartMenu : Control
{
	[Signal]
	public delegate void StartGameEventHandler();
	[Signal]
	public delegate void QuitGameEventHandler();

	public VBoxContainer Welcome;
	public VBoxContainer Load;

	public override void _Ready()
	{
		Welcome = GetNode<VBoxContainer>("Welcome");
		Load = GetNode<VBoxContainer>("Loading");
	}
	public void OnStartButtonButtonUp()
	{
		GD.Print("StartButton");
		EmitSignal(SignalName.StartGame);
	}
	public void OnSettingsButtonButtonUp()
	{

	}
	public void OnQuitButtonButtonUp()
	{
		EmitSignal(SignalName.QuitGame);
	}
	public void Loading()
	{
		Welcome.Visible = false;
		Load.Visible = true;
		GD.Print("Loading");
	}
}
