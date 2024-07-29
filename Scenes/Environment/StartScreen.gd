extends Node2D

var GameScene = null

func _ready():
	GameScene = preload("res://Scenes/Environment/Main.tscn")

func OnStartMenuQuitGame():
	get_tree().quit()
	pass

func OnStartMenuStartGame():
	$StartMenu.Loading()
	get_tree().change_scene_to_packed(GameScene)


