using Godot;
using System;

public partial class FSMTestMan : CharacterBody2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	public void Enter()
	{
		GD.Print("Enter");
	}
	public bool CheckEnter()
	{
		if (Input.IsKeyPressed(Key.Enter))
		{
			return true;
		}
		return false;
	}

	public void StartEnter()
	{
		GD.Print("Start Enter");
	}


	public void Space()
	{
		GD.Print("Space");
	}
	public bool CheckSpace()
	{
		if (Input.IsKeyPressed(Key.Space))
		{
			return true;
		}
		return false;
	}
	public void StartSpace()
	{
		GD.Print("Start Space");
	}

}
