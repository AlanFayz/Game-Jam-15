extends Control

class_name StartMenu

signal StartGameEventHandler
signal QuitGameEventHandler

func _ready():
	pass

func OnStartButtonButtonUp():
	print("StartButton")
	emit_signal(StartGameEventHandler.get_name())
	pass

func OnSettingsButtonButtonUp():
	pass

func OnQuitButtonButtonUp():
	emit_signal(QuitGameEventHandler.get_name())

func Loading():
	$Welcome.visible = false
	$Loading.visible = true

