using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

public partial class AudioManager : Node
{
	private struct AudioInfo
	{
		public AudioStream Stream { get; set; }
		public Vector2 Position { get; set; }
		public int StreamerIndex { get; set; } 

		public AudioInfo(Vector2 position)
		{
			Stream = null;
			Position = position;

			StreamerIndex = 0; //0 is default and will play error audio for debugging
		}
	}
	private struct UpdateAudioInfo
	{
		public Vector2 Position { get; set; }
		public string Name { get; set; }
	}
	
	private Dictionary<string, AudioInfo> m_AudioMap;
	private List<AudioStreamPlayer2D> m_AudioStreamPlayer2DList;
	private Stack<UpdateAudioInfo> m_UpdateAudioStack;
	
	public void PlayAudioStream(string name)
	{
		if (m_AudioMap.ContainsKey(name))
		{
			AudioInfo info = m_AudioMap[name];
			m_AudioStreamPlayer2DList[info.StreamerIndex].Stream = info.Stream;
			m_AudioStreamPlayer2DList[info.StreamerIndex].Position = info.Position;
			m_AudioStreamPlayer2DList[info.StreamerIndex].Play();
			return;
		}

		GD.PrintErr("stream name ", name, " not found");
	}

	public void AddAudio(string Path, string Name)
	{
		AudioStream stream = ResourceLoader.Load<AudioStream>(Path);
		
		if (stream == null)
		{
			GD.PrintErr("Invalid path ", Path);
			return;
		}

		AudioStreamPlayer2D streamPlayer2D = new AudioStreamPlayer2D();
		AddChild(streamPlayer2D);
		m_AudioStreamPlayer2DList.Add(streamPlayer2D);

		m_AudioMap[Name] = new AudioInfo(new Vector2(0.0f, 0.0f)); //changeable

		AudioInfo Info = m_AudioMap[Name];
		Info.Stream = stream;
		Info.StreamerIndex = m_AudioStreamPlayer2DList.Count - 1;

		m_AudioMap[Name] = Info;
	}

	public void UpdatePosition(string Name, Vector2 Position)
	{
		UpdateAudioInfo info = new UpdateAudioInfo();
		info.Name = Name;
		info.Position = Position;

		m_UpdateAudioStack.Push(info);
	}

	public override void _Ready()
	{
		m_AudioMap		            = new Dictionary<string, AudioInfo>();
		m_AudioStreamPlayer2DList   = new List<AudioStreamPlayer2D>();
		m_UpdateAudioStack			= new Stack<UpdateAudioInfo>();
	}
	public override void _Process(double delta)
	{
		while (m_UpdateAudioStack.Count != 0)
		{
			UpdateAudioInfo info = m_UpdateAudioStack.Pop();

			AudioInfo audioInfo = m_AudioMap[info.Name];
			audioInfo.Position = info.Position;

			m_AudioMap[info.Name] = audioInfo;
		}
	}
}
