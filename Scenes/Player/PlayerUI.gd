extends Control

signal ProtectionAmountChange(amount)
signal EnduranceAmountChange(amount)
signal FreezeAmountChange(amount)
signal FireAmountChange(amount)
signal PoisonAmountChange(amount)

@onready var ProtectionButton = $HBoxContainer/ProtectionButton
@onready var EnduranceButton = $HBoxContainer/EnduranceButton
@onready var TundraButton = $HBoxContainer/TundraButton
@onready var DesertButton = $HBoxContainer/DesertButton
@onready var SwampButton = $HBoxContainer/SwampButton
@onready var ShadowLabel = $HBoxContainer/ShadowLabel
@onready var HealthLabel = $HealthLabel

# Called when the node enters the scene tree for the first time.
func _ready():
	pass 
# Called every frame. 'delta' is the elapsed time since the previous frame.

func SetHealthDisplay(amount):
	HealthLabel.text = "Health: "+str(amount)

func OnProtectionButtonChangeAmount(amount):
	ProtectionAmountChange.emit(amount)

func OnEnduranceButtonChangeAmount(amount):
	EnduranceAmountChange.emit(amount)

func OnTundraButtonChangeAmount(amount):
	FreezeAmountChange.emit(amount)

func OnDesertButtonChangeAmount(amount):
	FireAmountChange.emit(amount)

func OnSwampButtonChangeAmount(amount):
	PoisonAmountChange.emit(amount)

func ChangeResourceDisplay(ResourcesOwned: Array):
	for i in range(0,len(ResourcesOwned)):
		match i:
			0:
				ProtectionButton.SetDisplayAmount(ResourcesOwned[i])
			1:
				EnduranceButton.SetDisplayAmount(ResourcesOwned[i])
			2:
				TundraButton.SetDisplayAmount(ResourcesOwned[i])
			3:
				DesertButton.SetDisplayAmount(ResourcesOwned[i])
			4:
				SwampButton.SetDisplayAmount(ResourcesOwned[i])
			5:
				ShadowLabel.text = str(ResourcesOwned[i])

