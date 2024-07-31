extends Control

signal ChangeAmount(amount)

@onready var TotalAmount = $HBoxContainer/VBoxContainer/TotalAmount
@onready var SpendAmount = $HBoxContainer/VBoxContainer/SpendAmount

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func OnUpButtonButtonUp():
	ChangeAmount.emit(1)
	ChangeSpendAmount(int(SpendAmount.text)+1)


func OnDownButtonButtonUp():
	if int(SpendAmount.text) > 0:
		ChangeAmount.emit(-1)
		ChangeSpendAmount(int(SpendAmount.text)-1)

func ChangeResourceAmount(amount):
	TotalAmount.text = str(amount)

func ChangeSpendAmount(amount):
	SpendAmount.text = str(amount)

func SetDisplayAmount(amount):
	TotalAmount.text = str(amount)
