extends Control

class_name StartMenu

signal StartGame
signal QuitGame
signal OpenSettings

func _ready():
	pass

func OnStartButtonButtonUp():
	print("StartButton")
	emit_signal(StartGame.get_name())
	

func OnSettingsButtonButtonUp():
	OpenSettings.emit()

func OnQuitButtonButtonUp():
	emit_signal(QuitGame.get_name())

func Disable():
	$Welcome/StartButton.disabled = true
	$Welcome/SettingsButton.disabled = true
	$Welcome/QuitButton.disabled = true

func Enable():
	$Welcome/StartButton.disabled = false
	$Welcome/SettingsButton.disabled = false
	$Welcome/QuitButton.disabled = false

func Loading():
	$Welcome.visible = false
	$Loading.visible = true

