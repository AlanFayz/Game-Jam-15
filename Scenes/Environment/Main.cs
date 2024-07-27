using Godot;
using System;

public partial class Main : Node
{
	private struct GameComponents
	{
		public AudioManager AudioManager;
		public Node2D Enemies;
		public Node2D Projectiles;
		public MapGeneration Map;
		public RandomNumberGenerator RandomNumberGenerator;
		public Timer EnemySpawnerTimer;
	}

	private GameComponents m_GameComponents;
	private Rect2 m_SpawnZone;

	public override void _Ready()
	{
		m_GameComponents.AudioManager = new AudioManager();
		m_GameComponents.RandomNumberGenerator = new RandomNumberGenerator();
		m_GameComponents.Enemies = GetNode<Node2D>("Enemies");
		m_GameComponents.Map = GetNode<MapGeneration>("Map");
		m_GameComponents.Projectiles = GetNode<Node2D>("Projectiles");
		m_GameComponents.EnemySpawnerTimer = GetNode<Timer>("Enemies/Timer");

		if (m_GameComponents.Projectiles == null ||
			m_GameComponents.Map == null ||
			m_GameComponents.Enemies == null)
		{
			GD.PrintErr("Failed to load nodes");
			return;
		}


		Vector2 mapSize = m_GameComponents.Map.GetMapSizeInLocalSpace();
		Vector2 mapPosition = m_GameComponents.Map.GetMapPositionInLocalSpace();

		m_SpawnZone = new Rect2(mapPosition, mapSize);

	}
	public override void _Process(double delta)
	{
		if (m_GameComponents.EnemySpawnerTimer.TimeLeft <= 0.0)
		{
			for (int i = 0; i < 10; i++)
				SpawnEnemy();

			m_GameComponents.EnemySpawnerTimer.Start();
		}
	}

	public void OnPlayerPotionThrow(Vector2 Pos, Vector2 Dir, float speed, float breakDamage, float poolDamage, int[] potionType)
	{
		var potion = ResourceLoader.Load<PackedScene>("res://Scenes/Projectiles/Potion.tscn").Instantiate() as Potion;
		potion.Position = Pos;
		potion.Speed = speed;
		potion.Direction = Dir;
		potion.PoolDamage = poolDamage;
		potion.BreakDamage = breakDamage;
		potion.PotionType = potionType;
		m_GameComponents.Projectiles.AddChild(potion);
		potion.Connect("PotionBreak", new Callable(this, MethodName.OnPotionPotionBreak));
	}

	public void OnPotionPotionBreak(Vector2 Pos, float poolDamage, int[] potionType)
	{
		PotionPool potionPool = ResourceLoader.Load<PackedScene>("res://Scenes/Projectiles/PotionPool.tscn").Instantiate() as PotionPool;
		potionPool.Position = Pos;
		potionPool.Damage = poolDamage;
		potionPool.PotionType = potionType;
		m_GameComponents.Projectiles.CallDeferred("add_child", potionPool);
	}
	public void SpawnEnemy()
	{
		Enemy enemy = ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/Enemy.tscn").Instantiate() as Enemy;
		enemy.Position = GetNextPosition();
		enemy.Connect("FireBolt", new Callable(this, MethodName.OnEnemyFireBolt));
		m_GameComponents.Enemies.CallDeferred("add_child", enemy);
	}

	private Vector2 GetNextPosition()
	{
		Vector2 start = m_SpawnZone.Position;
		Vector2 end = m_SpawnZone.End;

		Vector2 position = new Vector2(m_GameComponents.RandomNumberGenerator.RandfRange(start.X, end.X),
									   m_GameComponents.RandomNumberGenerator.RandfRange(start.Y, end.Y));

		return position;
	}
	public void OnPlayerSlash(Vector2 slashPos, Vector2 slashDir, float slashDamage, string slashType)
	{
		BaseSlash slash = ResourceLoader.Load<PackedScene>(slashType).Instantiate() as BaseSlash;
		slash.Position = slashPos;
		slash.AttackDir = slashDir;
		slash.Damage = slashDamage;
		AddChild(slash);
	}
	public void OnEnemyFireBolt(Vector2 Pos, Vector2 Dir, float speed, float damage)
	{
		FireBolt fireBolt = ResourceLoader.Load<PackedScene>("res://Scenes/Projectiles/FireBolt.tscn").Instantiate() as FireBolt;
		fireBolt.GlobalPosition = Pos;
		fireBolt.Direction = Dir;
		fireBolt.Speed = speed;
		fireBolt.Damage = damage;
		m_GameComponents.Projectiles.CallDeferred("add_child",fireBolt);
	}
	public void OnPlayerPlayerDeath()
	{
		GD.Print("Player Has Died");
		//add in game ending stuff (e.g. game over screen) here
	}
}
