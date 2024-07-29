extends Node2D

var GameScene = null

func _ready():
	GameScene = preload("res://Scenes/Environment/Main.tscn")
	pass # Replace with function body.

func OnStartMenuQuitGame():
	get_tree().quit()
	pass

func _process(_delta):
	$StartMenu.Loading()
	get_tree().change_scene_to_packed(GameScene)
	pass


func OnStartMenuStartGame():
	pass # Replace with function body.
