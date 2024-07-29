extends Control

class_name StartMenu

signal StartGame
signal QuitGame

func _ready():
	pass

func OnStartButtonButtonUp():
	print("StartButton")
	emit_signal(StartGame.get_name())
	pass

func OnSettingsButtonButtonUp():
	pass

func OnQuitButtonButtonUp():
	emit_signal(QuitGame.get_name())

func Loading():
	$Welcome.visible = false
	$Loading.visible = true

