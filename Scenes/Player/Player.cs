using Godot;
using System;

public partial class Player : CharacterBody2D
{
	float PlayerSpeed = 5000f;

	

	public override void _Ready()
	{
	}

    public override void _Process(double delta)
    {
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down").Normalized();
		if (inputDir.X<0)
		{
			Transform = Transform * Transform2D.FlipX;
		}
		Velocity = inputDir*PlayerSpeed*(float)delta;
		MoveAndSlide();

		
    }

}

