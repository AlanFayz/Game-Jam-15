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
	
	public void PlayAudioStream(string Name)
	{
		if (m_AudioMap.ContainsKey(Name))
		{
			AudioInfo Info = m_AudioMap[Name];
			m_AudioStreamPlayer2DList[Info.p_StreamerIndex].Stream = Info.p_Stream;
			m_AudioStreamPlayer2DList[Info.p_StreamerIndex].Position = Info.p_Position;
			m_AudioStreamPlayer2DList[Info.p_StreamerIndex].Play();
			return;
		}

		GD.PrintErr("stream name ", Name, " not found");
	}

	public void AddAudio(string Path, string Name)
	{
		AudioStream Stream = ResourceLoader.Load<AudioStream>(Path);
		
		if (Stream == null)
		{
			GD.PrintErr("Invalid path ", Path);
			return;
		}

		AudioStreamPlayer2D StreamPlayer2D = new AudioStreamPlayer2D();
		AddChild(StreamPlayer2D);
		m_AudioStreamPlayer2DList.Add(StreamPlayer2D);

		m_AudioMap[Name] = new AudioInfo(new Vector2(0.0f, 0.0f)); //changeable

		AudioInfo Info = m_AudioMap[Name];
		Info.p_Stream = Stream;
		Info.p_StreamerIndex = m_AudioStreamPlayer2DList.Count - 1;

		m_AudioMap[Name] = Info;
	}

	public void UpdatePosition(string Name, Vector2 Position)
	{
		UpdateAudioInfo Info = new UpdateAudioInfo();
		Info.p_Name = Name;
		Info.p_Position = Position;

		m_UpdateAudioStack.Push(Info);
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
			UpdateAudioInfo Info = m_UpdateAudioStack.Pop();

			AudioInfo AudioInfo = m_AudioMap[Info.p_Name];
			AudioInfo.p_Position = Info.p_Position;

			m_AudioMap[Info.p_Name] = AudioInfo;
		}
	}
}
