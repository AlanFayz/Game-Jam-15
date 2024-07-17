using Godot;
using System;

public partial class main : Node
{
	public AudioManager audioManager;

	public override void _Ready()
	{
		GD.Print("Start");
		GD.Print("Mr max man");

		audioManager = new AudioManager();
		AddChild(audioManager);

		audioManager.AddAudio("Assets/Audio/AWWW.mp3", "Test");
		audioManager.PlayAudioStream("Test");
		audioManager.UpdatePosition("Test", new Vector2(0.0f, 50.0f));
	}
	public override void _Process(double delta)
	{
	}
}
