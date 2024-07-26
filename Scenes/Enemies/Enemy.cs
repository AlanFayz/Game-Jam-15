using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;


public partial class Enemy : CharacterBody2D, IHittable
{
    delegate void ProcessStateDelegate(double delta);


    [Signal]
	public delegate void FireBoltEventHandler(Vector2 Pos, Vector2 Dir, float Speed, float Damage);

    private enum State
	{
		Idle = 0, Move, Attack, Death
	}

	private struct EnemyState
	{
		public State State;
		public Vector2 Direction;
		public Vector2 Velocity;
		public float Health;
	};

	private struct Nodes
	{
		public Timer Timer;
		public AnimationPlayer AnimationPlayer;
		public Sprite2D Sprite;
		public CollisionShape2D CollisionShape;
		public Node Parent;
		public Player Player;
		public Marker2D ShootPoint;
	};

	private struct EnemyProperties
	{
		// (in pixel space)
        public const float Speed = 100.0f;
        public const float ViewRadius = 300.0f;
		public const float AttackRadius = 150.0f;
        public const float Damage = 25.0f;
        public const float ProjectileSpeed = 350.0f;
    };

    private bool  m_IsAnimationPlaying = false;
	private Rect2 m_Bounds;

    private Dictionary<State, ProcessStateDelegate> m_States;
	private RandomNumberGenerator m_RandomNumberGenerator;

	private EnemyState m_EnemyState;
	private Nodes m_Nodes;

	public void Kill()
	{
		m_EnemyState.State = State.Death;
		m_IsAnimationPlaying = false;
		m_Nodes.CollisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		m_Nodes.AnimationPlayer.Play("Death");
	}

	public override void _Ready()
	{
		m_Nodes = new Nodes();
		m_States = new Dictionary<State, ProcessStateDelegate>();
		m_RandomNumberGenerator = new RandomNumberGenerator();
		m_EnemyState = new EnemyState();

		m_Nodes.Timer = GetNode<Timer>("Timer");
		m_Nodes.AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		m_Nodes.Sprite = GetNode<Sprite2D>("Sprite2D");
		m_Nodes.CollisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		m_Nodes.Parent = GetParent().GetParent();
		m_Nodes.Player = m_Nodes.Parent.GetNode<Player>("Player");
		m_Nodes.ShootPoint = GetNode<Marker2D>("ShootPoint");

		MapGeneration map = m_Nodes.Parent.GetNode<MapGeneration>("Map");

		m_Bounds = new Rect2(map.GetMapPositionInLocalSpace(), map.GetMapSizeInLocalSpace());

        m_EnemyState.State = State.Idle;
		m_EnemyState.Velocity = Vector2.Zero;
		m_EnemyState.Health = 100.0f;

		InitalizeStates();

		m_States[State.Idle](EnemyProperties.Speed);
	}

	public override void _Process(double delta)
	{
		if (m_EnemyState.State != State.Death)
		{
			if (m_Nodes.Timer.TimeLeft == 0)
			{
				m_EnemyState.State = ChooseState();
				m_IsAnimationPlaying = false;
			}
		
			if (GlobalPosition.DistanceTo(GetGlobalPlayerPosition()) <= EnemyProperties.ViewRadius)
			{
				m_EnemyState.State = State.Attack;
                m_IsAnimationPlaying = false;
            }

			m_States[m_EnemyState.State](EnemyProperties.Speed);

			Velocity = m_EnemyState.Velocity;

			MoveAndSlide();

			Position = ClampPosition(Position);
		}
		else if (m_Nodes.Timer.TimeLeft == 0)
		{
			QueueFree();
			return;
		}
	}

	public void Hit(Node origin, float damage)
	{
		m_EnemyState.Health -= damage;
		GD.Print(m_EnemyState.Health);
		CheckHealth();
	}
	public void CheckHealth()
	{
		if (m_EnemyState.Health <= 0)
		{
			Kill();
		}
	}

	private void InitalizeStates()
	{
		ProcessStateDelegate ProcessIdleState = (double delta) =>
		{
			if (!m_IsAnimationPlaying)
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
			if (!m_IsAnimationPlaying)
			{
				m_EnemyState.Direction = ChooseDirection();

				FlipSpriteIfNeeded();

				m_Nodes.AnimationPlayer.Play("Moving");
				m_Nodes.Timer.Start(5);

				m_IsAnimationPlaying = true;
			}

			m_EnemyState.Velocity = m_EnemyState.Direction * (float)delta;
		};

		ProcessStateDelegate ProcessAttackState = (double delta) =>
		{
			Vector2 direction = GetGlobalPlayerPosition() - GlobalPosition;
			direction = direction.Normalized();

			m_EnemyState.Direction = direction;

			if (Position.DistanceTo(GetGlobalPlayerPosition()) >= EnemyProperties.AttackRadius)
				m_EnemyState.Velocity = m_EnemyState.Direction * (float)delta;
			else
				m_EnemyState.Velocity = new Vector2(0.0f, 0.0f);

			FlipSpriteIfNeeded();

			if (!m_IsAnimationPlaying)
			{
				m_Nodes.AnimationPlayer.Play("Attacking");
				m_Nodes.Timer.Start(2); //TODO: look into how to get the length of animation

				m_IsAnimationPlaying = true;
			}
		};

		ProcessStateDelegate ProcessDeathState = (double delta) =>
		{
			if (!m_IsAnimationPlaying)
			{
				m_Nodes.AnimationPlayer.Play("Death");
				m_Nodes.Timer.Start(5);

				m_IsAnimationPlaying = true;
			}
		};

		m_States.Add(State.Idle, ProcessIdleState);
		m_States.Add(State.Attack, ProcessAttackState);
		m_States.Add(State.Move, ProcessMoveState);
		m_States.Add(State.Death, ProcessDeathState);
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

	private Vector2 GetGlobalPlayerPosition()
	{
		return m_Nodes.Player.GlobalPosition;
	}
	private Vector2 GetLocalPlayerPosition()
	{
		return m_Nodes.Player.Position; 
	}

	private void Shoot()
	{
		Vector2 direction = (GetGlobalPlayerPosition() - m_Nodes.ShootPoint.GlobalPosition);

        EmitSignal(
			SignalName.FireBolt, 
			m_Nodes.ShootPoint.GlobalPosition,
            direction.Normalized(), 
			EnemyProperties.ProjectileSpeed, 
			EnemyProperties.Damage);
	}

	private Vector2 ClampPosition(Vector2 position)
	{
		if (m_Bounds.HasPoint(position))
			return position;

		float xFactor = 0.0f;

		if (position.X > m_Bounds.End.X)
			xFactor = position.X - m_Bounds.End.X;
		else if (position.X < m_Bounds.Position.X)
			xFactor = m_Bounds.Position.X - position.X;

		float yFactor = 0.0f;

        if (position.Y > m_Bounds.End.Y)
            yFactor = position.Y - m_Bounds.End.Y;
        else if (position.Y < m_Bounds.Position.Y)
            yFactor = m_Bounds.Position.Y - position.Y;

		return new Vector2(position.X - xFactor, position.Y - yFactor);
	}
}
