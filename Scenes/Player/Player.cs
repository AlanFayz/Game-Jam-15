using Godot;
using System;

public partial class Player : CharacterBody2D
{
	float PlayerSpeed = 50000f;
	bool IsWalking = false;

	

	public override void _Ready()
	{
	}

    public override void _Process(double delta)
    {
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down").Normalized();
		
		if (inputDir == Vector2.Zero)
		{
			IsWalking = false;
		}
		else
		{
			IsWalking = true;
		}
		Velocity = inputDir*PlayerSpeed*(float)delta;
		MoveAndSlide();
    }

}

