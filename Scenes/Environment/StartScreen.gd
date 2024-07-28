extends Node2D

var Menu = null
var GameScene = null

func _ready():
	Menu = $StartMenu
	GameScene = ResourceLoader.load("res://Scenes/Environment/Main.tscn")
	pass # Replace with function body.

func OnStartMenuQuitGame():
	get_tree().quit()
	pass

func _process(delta):
	Menu.Loading()
	get_tree().change_scene_to_packed(GameScene)
	pass
