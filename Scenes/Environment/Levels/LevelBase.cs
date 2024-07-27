using Godot;
using System;

public partial class LevelBase : Node2D
{
	protected struct GameComponents
	{
		public AudioManager AudioManager;
		public Node2D Enemies;
		public Node2D Projectiles;
		public MapGeneration Map;
		public RandomNumberGenerator RandomNumberGenerator;
		public Timer EnemySpawnerTimer;
	}

	protected GameComponents m_GameComponents;
	

	public override void _Ready()
	{
		m_GameComponents.AudioManager = new AudioManager();
		m_GameComponents.RandomNumberGenerator = new RandomNumberGenerator();
		m_GameComponents.Enemies = GetNode<Node2D>("Enemies");
		m_GameComponents.Projectiles = GetNode<Node2D>("Projectiles");

		if (m_GameComponents.Projectiles == null ||
			m_GameComponents.Map == null ||
			m_GameComponents.Enemies == null)
		{
			GD.PrintErr("Failed to load nodes");
			return;
		}
	}

	

	public void OnPlayerPotionThrow(Vector2 Pos, Vector2 Dir, float speed, float breakDamage, float poolDamage)
	{
		var potion = ResourceLoader.Load<PackedScene>("res://Scenes/Projectiles/Potion.tscn").Instantiate() as Potion;
		potion.Position = Pos;
		potion.Speed = speed;
		potion.Direction = Dir;
		potion.PoolDamage = poolDamage;
		potion.BreakDamage = breakDamage;
		GD.Print(m_GameComponents.Projectiles);
		m_GameComponents.Projectiles.AddChild(potion);
		potion.Connect("PotionBreak", new Callable(this, MethodName.OnPotionPotionBreak));
	}

	public void OnPotionPotionBreak(Vector2 Pos, float poolDamage)
	{
		PotionPool potionPool = ResourceLoader.Load<PackedScene>("res://Scenes/Projectiles/PotionPool.tscn").Instantiate() as PotionPool;
		potionPool.Position = Pos;
		potionPool.Damage = poolDamage;
		m_GameComponents.Projectiles.CallDeferred("add_child", potionPool);
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
	public virtual void OnPlayerPlayerDeath()
	{
		GD.Print("Player Has Died");
		//add in game ending stuff (e.g. game over screen) here
	}
}
