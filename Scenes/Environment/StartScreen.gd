extends Node2D

var GameScene = null
@onready var SettingsMenu = $Settings
@onready var WelcomeMenu = $StartMenu

func ReadyHook():
	_ready()

func _ready():
	GameScene = preload("res://Scenes/Environment/Main.tscn")
	SettingsMenu.Disable()
	WelcomeMenu.Enable()
	StartModulate()
	print(WelcomeMenu.modulate)
	print("Ready")

func StartModulate():
	SettingsMenu.modulate = Color(1,1,1,0)
	WelcomeMenu.modulate = Color(1,1,1,1)

func _Process(_delta):
	print(SettingsMenu.modulate)

func OnStartMenuQuitGame():
	get_tree().quit()

func OnStartMenuStartGame():
	$StartMenu.Loading()
	get_tree().change_scene_to_packed(GameScene)


func OnStartMenuOpenSettings():
	$AnimationPlayer.play("OpenSettings")


func OnSettingsReturnToMenu():
	$AnimationPlayer.play_backwards("OpenSettings")
