extends Node2D

enum Cell 
{
	Tundra = 0, Taiga, Forest, Desert, Swamp, None
}

enum Tier
{
	None = 0, Low = 20, Medium = 10, High = 2
}

class NoiseGeneration:
	var NoiseGenerationAlgorithm: NoiseGeneration
	var RandomNumbers: RandomNumberGenerator

class Biome:
	var AtlasCoordinates: Array
	var AtlasSizes: Array 
	var TileSetSourceIndex: int

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
