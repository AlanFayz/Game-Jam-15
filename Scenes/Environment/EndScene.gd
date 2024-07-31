extends Node2D

# Called when the node enters the scene tree for the first time.
func _ready():
	$VBoxContainer/TimeAlive.text     = "Time Alive: "     + str(floori(Globals.TimeAlive))
	$VBoxContainer/EnemiesKilled.text = "Enemies Killed: " + str(Globals.EnemiesKilled)
	
	var percentage: float = 0
	if Globals.LightedTiles > 0:
		percentage = float(Globals.LightedTiles) / float(Globals.TotalTiles)
	
	percentage *= 100
	$VBoxContainer/MapDeshadowed.text = "Percentage of lighted tiles: " + str(percentage)
	
	if Globals.Win:
		$VBoxContainer/WinLose.text = "You Won!"
	else:
		$VBoxContainer/WinLose.text = "You Lost!"
		
	pass 

func OnButtonPressed():
	get_tree().quit()

# Called every frame. 'delta' is the elapsed time since the previous frame.
