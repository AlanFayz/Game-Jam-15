extends Node2D

# Called when the node enters the scene tree for the first time.
func _ready():
	$VBoxContainer/TimeAlive.text     = "Time Alive: "     + str(floori(Globals.TimeAlive))
	$VBoxContainer/EnemiesKilled.text = "Enemies Killed: " + str(Globals.EnemiesKilled)
	if Globals.Win:
		$VBoxContainer/WinLose.text = "You Won!"
	else:
		$VBoxContainer/WinLose.text = "You Lost!"
		
	pass 

func OnButtonPressed():
	var scene = preload("res://Scenes/Environment/StartScreen.tscn")
	get_tree().change_scene_to_packed(scene)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
