using Godot;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

delegate void ProcessStateDelegate(double delta);

public partial class Enemy : CharacterBody2D
{
	private enum State
	{
		Idle = 0, Move, Attack, Death
	}

	private struct EnemyState
	{
		public State    State;
		public Vector2  Direction;
		public Vector2  Velocity;
		public float	Health;
	};

	private struct Nodes
	{
		public Timer Timer;
		public AnimationPlayer AnimationPlayer;
		public Sprite2D Sprite;
	};

	private const float  m_Speed = 10.0f;
	private const float  m_ViewRadius = 1.0f;
	private const double m_TimeBetweenStates = 10.0;
	private bool m_IsAnimationPlaying = false;

	private Dictionary<State, ProcessStateDelegate> m_States;
	private RandomNumberGenerator m_RandomNumberGenerator;

	private EnemyState m_EnemyState;
	private Nodes m_Nodes;

	public void Kill()
	{
		m_EnemyState.State = State.Death;
		m_IsAnimationPlaying = false;
		m_Nodes.AnimationPlayer.Stop();
	}
	
	// will return true if health is equal to or below 0.
	public bool Damage(float damage)
	{
		m_EnemyState.Health -= damage;

		if (m_EnemyState.Health <= 0)
			return true;

		return false;
	}

	public void SetHealth(float newHealth)
	{
		m_EnemyState.Health = newHealth;
	}

	public float GetHealth()
	{
		return m_EnemyState.Health;
	}

	public override void _Ready()
	{
		m_Nodes					= new Nodes();
		m_States				= new Dictionary<State, ProcessStateDelegate>();
		m_RandomNumberGenerator = new RandomNumberGenerator();
		m_EnemyState			= new EnemyState();

		m_Nodes.Timer			= GetNode<Timer>("Timer");
		m_Nodes.AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		m_Nodes.Sprite			= GetNode<Sprite2D>("Sprite2D");

		m_EnemyState.State     = State.Idle;
		m_EnemyState.Velocity  = Vector2.Zero;
		m_EnemyState.Health	   = 100.0f;

		InitalizeStates();

		m_States[State.Idle](m_Speed);
	}

	public override void _Process(double delta)
	{
		if (m_Nodes.Timer.TimeLeft == 0 && m_EnemyState.State != State.Death)
		{
			m_EnemyState.State = ChooseState();
			m_IsAnimationPlaying = false;
		}
		else if(m_Nodes.Timer.TimeLeft == 0 && m_EnemyState.State == State.Death)
		{
			QueueFree();
			return;
		}

		if (Position.DistanceTo(GetPlayerPosition()) <= m_ViewRadius)
			m_EnemyState.State = State.Attack;

		m_States[m_EnemyState.State](m_Speed);

		Velocity = m_EnemyState.Velocity;

		MoveAndSlide();
	}

	public override void _PhysicsProcess(double delta)
	{
	}

	private void InitalizeStates()
	{
		ProcessStateDelegate ProcessIdleState = (double delta) =>
		{
			if(!m_IsAnimationPlaying)
			{
				m_EnemyState.Direction = ChooseDirection();
				m_EnemyState.Velocity = Vector2.Zero;

				FlipSpriteIfNeeded();

				m_Nodes.AnimationPlayer.Play("Idle");
				m_Nodes.Timer.Start(5);

				m_IsAnimationPlaying = true;
			}
		};


		ProcessStateDelegate ProcessMoveState = (double delta) =>
		{
			if(!m_IsAnimationPlaying)
			{
				m_EnemyState.Direction = ChooseDirection();

				FlipSpriteIfNeeded();

				m_Nodes.AnimationPlayer.Play("Moving");
				m_Nodes.Timer.Start(5);

				m_IsAnimationPlaying = true;
			}

			m_EnemyState.Velocity = m_EnemyState.Direction * m_Speed * (float)delta;
		};

		ProcessStateDelegate ProcessAttackState = (double delta) =>
		{
			Vector2 direction = GetPlayerPosition() - Position;
			direction = direction.Normalized();

			m_EnemyState.Direction = direction;
			m_EnemyState.Velocity  = m_EnemyState.Direction * m_Speed * (float)delta;

			FlipSpriteIfNeeded();
			
			if(!m_IsAnimationPlaying)
			{
				m_Nodes.AnimationPlayer.Play("Attacking");
				m_Nodes.Timer.Start(2); //TODO: look into how to get the length of animation

				m_IsAnimationPlaying = true;
			}
		};

		ProcessStateDelegate ProcessDeathState = (double delta) =>
		{
			if(!m_IsAnimationPlaying)
			{
				m_Nodes.AnimationPlayer.Play("Death");
				m_Nodes.Timer.Start(5);

				m_IsAnimationPlaying = true;
			}
		};

		m_States.Add(State.Idle,   ProcessIdleState);
		m_States.Add(State.Attack, ProcessAttackState);
		m_States.Add(State.Move,   ProcessMoveState);
		m_States.Add(State.Death,  ProcessDeathState);
	}

	private Vector2 ChooseDirection()
	{
		return new Vector2(m_RandomNumberGenerator.RandfRange(-1.0f, 1.0f),
						   m_RandomNumberGenerator.RandfRange(-1.0f, 1.0f));
	}

	private void FlipSpriteIfNeeded()
	{
		if (m_EnemyState.Direction.X < 0)
			m_Nodes.Sprite.FlipH = true;
		else
			m_Nodes.Sprite.FlipH = false;
	}

	private State ChooseState()
	{
		return (State)m_RandomNumberGenerator.RandiRange(0, 1);
	}

	private Vector2 GetPlayerPosition()
	{
		return new Vector2(100.0f, 0.0f); //TODO: max implement this when player is finished
	}
}
