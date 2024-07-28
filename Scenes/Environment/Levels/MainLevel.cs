using Godot;
using System;

public partial class MainLevel : LevelBase
{
	private Rect2 m_SpawnZone;
	private PackedScene m_EnemyScene;

    public override void _Ready()
    {
        m_EnemyScene = ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/Enemy.tscn");
		m_GameComponents.Map = GetNode<MapGeneration>("Map");
		m_GameComponents.EnemySpawnerTimer = GetNode<Timer>("Enemies/EnemyTimer");
		m_GameComponents.Enemies = GetNode<Node2D>("Enemies");
		m_GameComponents.Projectiles = GetNode<Node2D>("Projectiles");

		Vector2 mapSize = m_GameComponents.Map.GetMapSizeInLocalSpace();
		Vector2 mapPosition = m_GameComponents.Map.GetMapPositionInLocalSpace();
		
        m_SpawnZone = new Rect2(mapPosition, mapSize);
    }
    public void OnEnemyTimerTimeout()
	{
		for (int i = 0; i < 10; i++)
			SpawnEnemy();
	}

	public void SpawnEnemy()
	{
		Enemy enemy = m_EnemyScene.Instantiate() as Enemy;
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
}
