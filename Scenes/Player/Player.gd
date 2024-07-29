extends CharacterBody2D
class_name Player

signal PotionThrow(pos, dir, speed, breakDamage, poolDamage, potionType)
signal PurificationPotionThrow(pos, dir, speed, radius)
signal Slash(pos, dir, slashDamage, slashType)
signal PlayerDeath()

var PlayerSpeed: float = 300

var DashSpeed: float = 600
var DashDir: Vector2


var IsWalking: bool = true
var IsDashing: bool = false
var CanDash: bool = true

var IsDying: bool = false
var IsSlowed: bool = false

var FreezeImmunity: bool = false
func SetFreezeImmunity(value):
	if value:
		FreezeImmunityTimer.Start()

var IsOnFire: bool = false
var IsPoisoned: bool = false
var FireDamage: float = 0
var PoisonDamage: float = 0
var PoisonWeaknessLevel: float = 0
var SlowLevel: float = 0

var Health: float = 100
func SetHealth(value):
	Health = value
	print("Health = ", Health)
	CheckDeath()

var IsImmune: bool = false

var CanThrow: bool = true
var ThrowSpeed: float = 400
var BreakDamage: float = 5
var PoolDamage: float = 15
#Potion's structure is [Protection, Endurance, Freeze, Burn, Poison]
var PotionType = [0,1,0,1,1]

var PurificationRadius: float = 100


var SlashDistances = [15.0,30.0]
var CanSlash = true
var SlashDamage: float = 10
var Slashes = ["res://Scenes/Melee/Slashes/Slash1.tscn", "res://Scenes/Melee/Slashes/Slash2.tscn"]

var OldPosition: Vector2


@onready var ThrowCooldown = $Timers/ThrowCooldown
@onready var SlashCooldown = $Timers/SlashCooldown
@onready var ImmunityFrames = $Timers/ImmunityFrames
@onready var FireCountdown = $Timers/FireTimeLeft
@onready var FireTicks = $Timers/FireTicks
@onready var PoisonCountdown = $Timers/PoisonTimeLeft
@onready var PoisonTicks = $Timers/PoisonTicks
@onready var FreezeCountdown = $Timers/FreezeTimeLeft
@onready var FreezeImmunityTimer = $Timers/FreezeImmunity
@onready var DashCooldown = $Timers/DashCooldown
@onready var animation = $AnimationPlayer
@onready var SlashAudioPlayer = $SlashAudioPlayer
@onready var WalkingAudioPlayer = $WalkingDirtAudioPlayer



func _process(delta):
	if IsDying:
		return
	elif not IsDashing:
		var inputDir = Input.get_vector("move_left", "move_right", "move_up", "move_down").normalized();
			
		if (inputDir == Vector2.ZERO):
			animation.play("idle")

		elif (inputDir.x>0):
			animation.play("walk_right")

		elif (inputDir.x < 0):
			animation.play("walk_left")

		else:
			animation.play("walk_vertical")

		velocity = inputDir*PlayerSpeed*GetFreezeSlowdown()

		move_and_slide()

		if OldPosition == self.position:
			WalkingAudioPlayer.stop()
		elif OldPosition != self.position && WalkingAudioPlayer.playing == false:
			WalkingAudioPlayer.play()

		WalkingAudioPlayer.position = self.position
		OldPosition = self.position
		
		if CanDash and Input.is_action_pressed("dash"):
			DashStart(inputDir)
		
	elif IsDashing == true:
		velocity = DashDir*DashSpeed*GetFreezeSlowdown()
		move_and_slide()
	
	if CanThrow and Input.is_action_pressed("throw_potion"):
		ThrowPotion()
	
	if (CanSlash and Input.is_action_pressed("primary_attack")):
		SlashAttack()
		
	if CanThrow and Input.is_action_pressed("throw_purification_potion"):
		ThrowPurificationPotion()

func DashStart(Dir):
	CanDash = false
	IsDashing = true
	DashDir = Dir
	animation.play("Dash", -1, 2.5)

func DashEnd():
	IsDashing = false
	DashCooldown.start()

func ThrowPotion():
	CanThrow = false
	ThrowCooldown.start()
	PotionThrow.emit(global_position, 
					 get_local_mouse_position().normalized(), 
					 ThrowSpeed * GetPoisonWeakness(), 
					 BreakDamage * GetPoisonWeakness(), 
					 PoolDamage,
					 PotionType)

func ThrowPurificationPotion():
	CanThrow = false
	ThrowCooldown.start()
	PurificationPotionThrow.emit(global_position, get_local_mouse_position().normalized(), ThrowSpeed*GetPoisonWeakness(), 100)
	
func Hit(_Origin, damage, Effects):
	if IsImmune:
		return
	SetHealth(Health-damage)
	IsImmune = true
	ImmunityFrames.start()
	
	#Freeze
	if Effects[0] > 2 and not IsSlowed:
		IsSlowed = true
		SlowLevel = Effects[0]
		FreezeCountdown.start(0.75)
	
	elif (Effects[0] > 0 && !IsSlowed):
		IsSlowed = true
		FreezeCountdown.start(2.5)
		SlowLevel = Effects[0]
	
	#Burn
	if (Effects[1] > 0):
		IsOnFire = true
		FireCountdown.start(1.0+(0.5*Effects[1]))
		FireDamage = 2+0.5*Effects[1]
		FireTicks.start()
	
	#Poison
	if (Effects[2] > 0):
		IsPoisoned = true;
		PoisonCountdown.start(5+2*Effects[2])
		PoisonDamage = 2+0.5*Effects[1]
		PoisonTicks.start()
		PoisonWeaknessLevel = Effects[2]
	
func SlashAttack():
	CanSlash = false
	SlashCooldown.start(0.25*PoisonWeaknessLevel)
	var mouseDir = get_local_mouse_position().normalized()
	var slashType = abs(randi())%2
	var LocalSlashLocation = mouseDir*SlashDistances[slashType]
	Slash.emit(global_position+LocalSlashLocation, mouseDir, SlashDamage*GetPoisonWeakness(), Slashes[slashType])
	SlashAudioPlayer.play()

func CheckDeath():
	if Health < 0:
		StartDeath()

func StartDeath():
	IsDying = true
	animation.play("Death", 0, 0.25, false)
	
func EndDeath():
	PlayerDeath.emit()
	
	#for testing
	Health = 10000
	IsDying = false

func GetFreezeSlowdown() -> float:
	return clampf(1-SlowLevel/3,0,1)

func GetPoisonWeakness() -> float:
	return 1/clamp(PoisonWeaknessLevel*2/3, 1, 2^32);

func OnThrowCooldownTimeout():
	CanThrow = true


func OnSlashCooldownTimeout():
	CanSlash = true


func OnImmunityFramesTimeout():
	IsImmune = false


func OnFireTimeLeftTimeout():
	IsOnFire = false
	FireTicks.stop()


func OnFreezeTimeLeftTimeout():
	IsSlowed = false
	SlowLevel = 0
	FreezeImmunity = true


func OnPoisonTimeLeftTimeout():
	IsPoisoned = false
	PoisonWeaknessLevel = 0
	PoisonTicks.stop()


func OnFireTicksTimeout():
	SetHealth(Health - FireDamage) 


func OnPoisonTicksTimeout():
	SetHealth(Health - PoisonDamage) 

func OnFreezeImmunityTimeout():
	FreezeImmunity = false


func OnDashCooldownTimeout():
	CanDash = true
