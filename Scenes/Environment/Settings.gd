extends Control
class_name Settings

signal ReturnToMenu

@onready var MasterVolumeSlider = $MarginContainer/VBoxContainer/ScrollContainer/Audio/MasterVolume
@onready var SoundVolumeSlider = $MarginContainer/VBoxContainer/ScrollContainer/Audio/SoundVolume
@onready var MusicVolumeSlider = $MarginContainer/VBoxContainer/ScrollContainer/Audio/MusicVolume


# Called when the node enters the scene tree for the first time.
func _ready():
	Disable()

func _process(_delta):
	if Input.is_action_pressed("escape"):
		ReturnToMenu.emit()

func Disable():
	MasterVolumeSlider.scrollable = false
	SoundVolumeSlider.scrollable = false
	MusicVolumeSlider.scrollable = false

func Enable():
	MasterVolumeSlider.scrollable = true
	SoundVolumeSlider.scrollable = true
	MusicVolumeSlider.scrollable = true


func OnMasterVolumeValueChanged(value):
	Globals.MasterVolume = value


func OnSoundVolumeValueChanged(value):
	Globals.SoundVolume = value


func OnMusicVolumeValueChanged(value):
	Globals.MusicVolume = value


func OnEscapeButtonButtonUp():
	ReturnToMenu.emit()


