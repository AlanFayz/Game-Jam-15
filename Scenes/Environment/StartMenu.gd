extends Control

signal StartGameSignal
signal QuitGameSignal

func _ready():
	pass

func OnStartButtonButtonUp():
	print("StartButton")
	emit_signal(StartGameSignal.get_name())
	pass

func OnSettingsButtonButtonUp():
	pass

func OnQuitButtonButtonUp():
	emit_signal(QuitGameSignal.get_name())

func Loading():
	$Welcome.visible = false
	$Loading.visible = true

func _process(delta):
	pass
