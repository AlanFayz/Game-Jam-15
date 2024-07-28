extends Node2D

var GameScene = null

func _ready():
	GameScene = ResourceLoader.load("res://Scenes/Environment/Main.tscn")
	pass # Replace with function body.

func OnStartMenuQuitGame():
	get_tree().quit()
	pass

func _process(delta):
	var menu = $StartMenu as StartMenu
	menu.Loading()
	get_tree().change_scene_to_packed(GameScene)
	pass
