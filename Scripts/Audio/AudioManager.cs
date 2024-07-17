using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

public partial class AudioManager : Node
{
	private struct AudioInfo
	{
		public AudioStream p_Stream { get; set; }
		public Vector2 p_Position   { get; set; }
		public int p_StreamerIndex { get; set; } 

		public AudioInfo(Vector2 Position)
		{
			p_Stream = null;
			p_Position = Position;

			p_StreamerIndex = 0; //0 is default and will play error audio for debugging
		}
	}
	private struct UpdateAudioInfo
	{
		public Vector2 p_Position { get; set; }
		public string p_Name { get; set; }
	}
	
	private Dictionary<string, AudioInfo> m_AudioMap;
	private List<AudioStreamPlayer2D> m_AudioStreamPlayer2DList;
	private Stack<UpdateAudioInfo> m_UpdateAudioStack;
	
	public void PlayAudioStream(string name)
	{
		if (m_AudioMap.ContainsKey(name))
		{
			AudioInfo info = m_AudioMap[name];
			m_AudioStreamPlayer2DList[info.p_StreamerIndex].Stream = info.p_Stream;
			m_AudioStreamPlayer2DList[info.p_StreamerIndex].Position = info.p_Position;
			m_AudioStreamPlayer2DList[info.p_StreamerIndex].Play();
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
		Info.p_Stream = stream;
		Info.p_StreamerIndex = m_AudioStreamPlayer2DList.Count - 1;

		m_AudioMap[Name] = Info;
	}

	public void UpdatePosition(string Name, Vector2 Position)
	{
		UpdateAudioInfo info = new UpdateAudioInfo();
		info.p_Name = Name;
		info.p_Position = Position;

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

			AudioInfo audioInfo = m_AudioMap[info.p_Name];
			audioInfo.p_Position = info.p_Position;

			m_AudioMap[info.p_Name] = audioInfo;
		}
	}
}
