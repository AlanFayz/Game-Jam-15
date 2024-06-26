using Godot;
using System;

public partial class main : Node
{
	// Called when the node enters the scene tree for the first time.
	AudioManager AudioManager;

	public override void _Ready()
	{
		GD.Print("Start");
		GD.Print("Mr max man");

		AudioManager = new AudioManager();
		AddChild(AudioManager);

		AudioManager.AddAudio("Assets/Audio/AWWW.mp3", "Test");
		AudioManager.PlayAudioStream("Test");
		AudioManager.UpdatePosition("Test", new Vector2(0.0f, 50.0f));
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
