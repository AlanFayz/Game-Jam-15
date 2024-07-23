using Godot;
using System;

public partial class Main : Node
{
	private struct GameComponents
	{
		public AudioManager  AudioManager;
		public EnemyManager  EnemyManager;
		public MapGeneration Map;
		public Node2D		 Projectiles;
	}

	private GameComponents m_GameComponents;
	
	public override void _Ready()
	{
		m_GameComponents.AudioManager = new AudioManager();
		m_GameComponents.EnemyManager = GetNode<EnemyManager>("Enemies");
		m_GameComponents.Map		  = GetNode<MapGeneration>("Map");
		m_GameComponents.Projectiles  = GetNode<Node2D>("Projectiles");

		if (m_GameComponents.Projectiles  == null ||
			m_GameComponents.Map		  == null ||
			m_GameComponents.EnemyManager == null)
		{
			GD.PrintErr("Failed to load nodes");
			return;
		}


		Vector2I mapSize     = new Vector2I(10, 10);
		Vector2I mapPosition = new Vector2I(0, 0);

		Rect2 spawnZone = new Rect2(mapPosition, mapSize);
		m_GameComponents.EnemyManager.StartSpawning(spawnZone);
	}
	public override void _Process(double delta)
	{
	}

	public void OnPlayerPotionThrow(Vector2 Pos, Vector2 Dir, float Speed)
	{
		var potion = ResourceLoader.Load<PackedScene>("res://Scenes/Projectiles/Potion.tscn").Instantiate() as Potion;
		potion.Position = Pos;
		potion.Speed = Speed;
		potion.Direction = Dir;
		m_GameComponents.Projectiles.AddChild(potion);
		potion.Connect("PotionBreak", new Callable(this, MethodName.OnPotionPotionBreak));
	}

	public void OnPotionPotionBreak(Vector2 Pos)
	{
		PotionPool potionPool = ResourceLoader.Load<PackedScene>("res://Scenes/Projectiles/PotionPool.tscn").Instantiate() as PotionPool;
		potionPool.Position = Pos;
		m_GameComponents.Projectiles.CallDeferred("add_child", potionPool);
	}

	public void OnPlayerSlash(Vector2 SlashPos, Vector2 SlashDir)
	{
		Slash1 slash = ResourceLoader.Load<PackedScene>("res://Scenes/Melee/Slashes/Slash1.tscn").Instantiate() as Slash1;
		slash.Position = SlashPos;
		slash.AttackDir = SlashDir;
		AddChild(slash);
	}
}
