using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyManager : Node2D
{
    //may make these variable in the future
    private const float m_TimeBetweenSpawn = 10.0f;
	private const int   m_NumberOfEnemiesToSpawn = 20; 

	private List<Enemy> m_EnemyList;
	private Timer m_Timer;
	private Rect2 m_SpawnZone;
	private RandomNumberGenerator m_RandomNumberGenerator;
	private bool m_StartSpawning = false;

    public void StartSpawning(Rect2 spawnZone)
    {
        m_SpawnZone = spawnZone;
        m_Timer.Start(m_TimeBetweenSpawn);
        m_StartSpawning = true;
    }

    public void StopSpawning()
    {
        m_StartSpawning = false;
        m_Timer.Stop();
    }
    public Enemy GetEnemy(int index)
    {
        if (index >= 0 && index < m_EnemyList.Count)
            return m_EnemyList[index];

        return null;
    }

	public void KillEnemy(int index)
	{
        if (index >= 0 && index < m_EnemyList.Count)
		{
            m_EnemyList[index].Kill();
			m_EnemyList.RemoveAt(index);
			return;
        }

		GD.Print("Warning: KillEnemy(", index, ") is out of range");
    }

    // will return true if health of enemy is equal to or below 0.
    public bool DamageEnemy(int index, float damage)
	{
        if (index >= 0 && index < m_EnemyList.Count)
            return m_EnemyList[index].Damage(damage);
       
        GD.Print("Warning: DamageEnemy(", index, ", ", damage, ") is out of range");
		return false;
    }

	public float GetEnemyHealth(int index)
	{
		if (index >= 0 && index < m_EnemyList.Count)
			return m_EnemyList[index].GetHealth();

        GD.Print("Warning: GetEnemyHealth(", index, ") is out of range");
		return 1f;
    }

	public int GetEnemyCount()
	{
		return m_EnemyList.Count; 
	}

    public override void _Ready()
	{
		m_RandomNumberGenerator = new RandomNumberGenerator();
		m_EnemyList				= new List<Enemy>();
		m_Timer					= new Timer();
	}

	public override void _Process(double delta)
	{
		if (!m_StartSpawning)
			return;

		if (m_Timer.TimeLeft == 0.0)
			Spawn();
	}

	private void Spawn()
	{
		m_Timer.Stop();

		for(int i = 0; i < m_NumberOfEnemiesToSpawn; i++)
		{
			Enemy enemy = new Enemy();
			AddChild(enemy);
			enemy.Position = GetNextPosition();

			m_EnemyList.Add(enemy);
		}

		m_Timer.Start(m_TimeBetweenSpawn);
	}

	private Vector2 GetNextPosition()
	{
		Vector2 start = m_SpawnZone.Position;
		Vector2 end   = m_SpawnZone.End;

		Vector2 position = new Vector2(m_RandomNumberGenerator.RandfRange(start.X, end.X),
									   m_RandomNumberGenerator.RandfRange(start.Y, end.Y));

		return position;
	}
}
