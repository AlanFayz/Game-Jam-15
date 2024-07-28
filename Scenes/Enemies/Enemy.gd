extends CharacterBody2D

signal FireBoltEventHandler(position: Vector2, direction: Vector2, speed: float, damage: float)

enum State
{
	Idle = 0, Move, Attack, Death
}

class EnemyState:
	var StateType: State
	var Direction: Vector2
	var Velocity: Vector2
	var Health: float


class Nodes:
	var Parent: Node
	var PlayerNode: Player

class EnemyProperties:
	# in pixel space
	const Speed: float = 100.0
	const ViewRadius: float = 300.0
	const AttackRadius: float = 150.0
	const Damage: float = 10.0
	const ProjectileSpeed: float = 350.0


var m_IsAnimationPlaying = false
var m_Bounds: Rect2

var m_States: Dictionary
var m_RandomNumberGenerator : RandomNumberGenerator

var m_EnemyState: EnemyState 
var m_Nodes: Nodes

func Kill():
	m_EnemyState.State = State.Death
	m_IsAnimationPlaying = false
	$CollisionShape2D.set_deferred("disabled", true)
	$AnimationPlayer.play("Death")

func _ready():
	m_Nodes = Nodes.new()
	m_States = {}
	m_RandomNumberGenerator = RandomNumberGenerator.new()
	m_EnemyState = EnemyState.new()

	m_Nodes.Parent = get_parent().get_parent()
	m_Nodes.Player = m_Nodes.Parent.get_node("Player") 

	var map = m_Nodes.Parent.get_node("Map")

	m_Bounds = Rect2(map.GetMapPositionInLocalSpace(), map.GetMapSizeInLocalSpace())

	m_EnemyState.State = State.Idle
	m_EnemyState.Velocity = Vector2.ZERO
	m_EnemyState.Health = 100.0

	InitalizeStates()

	m_States[State.Idle].call(EnemyProperties.Speed)


func _process(delta):
	if m_EnemyState.State != State.Death:
		if m_Nodes.Timer.TimeLeft == 0:
			m_EnemyState.State = ChooseState()
			m_IsAnimationPlaying = false
	
		if self.global_position.distance_to(GetGlobalPlayerPosition()) <= EnemyProperties.ViewRadius:
			m_EnemyState.State = State.Attack;
			m_IsAnimationPlaying = false;
		

		m_States[m_EnemyState.State].call(EnemyProperties.Speed);

		self.velocity = m_EnemyState.Velocity;

		move_and_slide();

		self.position = ClampPosition(self.position)
		
	elif m_Nodes.Timer.TimeLeft == 0:
		queue_free()
		return

func Hit(origin: Node, damage: float, potionEffects: Array):
	m_EnemyState.Health -= damage
	print(m_EnemyState.Health)
	CheckHealth()

func CheckHealth():
	if m_EnemyState.Health <= 0:
		Kill();


func ProcessIdleStateFun(delta):
	if !m_IsAnimationPlaying:
		m_EnemyState.Direction = ChooseDirection()
		m_EnemyState.Velocity = Vector2.ZERO

		FlipSpriteIfNeeded()

		$AnimationPlayer.play("Idle")
		$Timer.start(5)

		m_IsAnimationPlaying = true

func ProcessMoveStateFun(delta):
	if !m_IsAnimationPlaying:
		m_EnemyState.Direction = ChooseDirection();

		FlipSpriteIfNeeded();

		$AnimationPlayer.play("Moving");
		$Timer.start(5);

		m_IsAnimationPlaying = true;
		

	m_EnemyState.Velocity = m_EnemyState.Direction * delta;
	
func ProcessAttackStateFun(delta):
	var direction = GetGlobalPlayerPosition() - self.global_position
	
	m_EnemyState.Direction =  direction.normalized()

	if self.position.distance_to(GetGlobalPlayerPosition()) >= EnemyProperties.AttackRadius:
		m_EnemyState.Velocity = m_EnemyState.Direction * delta
	else:
		m_EnemyState.Velocity = Vector2(0.0, 0.0)

	FlipSpriteIfNeeded()

	if !m_IsAnimationPlaying:
		$AnimationPlayer.play("Attacking")
		%Timer.start(2)

		m_IsAnimationPlaying = true;

func ProcessDeathStateFun(delta):
	if !m_IsAnimationPlaying:
		$AnimationPlayer.Play("Death")
		$Timer.Start(5)
		m_IsAnimationPlaying = true

func InitalizeStates():
	var ProcessIdleState = ProcessIdleStateFun
	var ProcessMoveState = ProcessMoveStateFun	
	var ProcessAttackState = ProcessAttackStateFun
	var ProcessDeathState = ProcessDeathStateFun

	m_States[State.Idle]   = ProcessIdleState
	m_States[State.Attack] = ProcessAttackState
	m_States[State.Move]   = ProcessMoveState
	m_States[State.Death]  = ProcessDeathState


func ChooseDirection() -> Vector2:
	return Vector2(m_RandomNumberGenerator.randf_range(-1.0, 1.0),
				   m_RandomNumberGenerator.randf_range(-1.0, 1.0))


func FlipSpriteIfNeeded():
	if m_EnemyState.Direction.x < 0:
		$Sprite2D.FlipH = true;
	else:
		$Sprite2D.FlipH = false;


func ChooseState() -> State:
	return m_RandomNumberGenerator.randi_range(0, 1) as State

func GetGlobalPlayerPosition() -> Vector2:
	return m_Nodes.Player.global_position

func GetLocalPlayerPosition() -> Vector2:
	return m_Nodes.Player.position 

func Shoot():
	var direction = (GetGlobalPlayerPosition() - %ShootPoint.global_position)

	emit_signal(
		FireBoltEventHandler.get_name(),
		$ShootPoint.global_position,
		direction.Normalized(), 
		EnemyProperties.ProjectileSpeed, 
		EnemyProperties.Damage)


func ClampPosition(position: Vector2) -> Vector2:
	var clampedX = clamp(position.x, m_Bounds.position.x, m_Bounds.end.x)
	var clampedY = clamp(position.y, m_Bounds.position.y, m_Bounds.end.y)

	return Vector2(clampedX, clampedY);

