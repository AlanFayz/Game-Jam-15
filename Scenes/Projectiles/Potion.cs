using Godot;
using System;

public partial class Potion : Area2D
{
	public override void _Ready()
	{
	}


	public override void _Process(double delta)
	{

	}

	public void OnBreakTimerTimeout()
	{
		GD.Print("Potion");
		QueueFree();
	}
}
