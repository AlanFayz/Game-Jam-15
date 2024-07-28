extends Node2D

class_name MapGeneration

enum Cell 
{
	Tundra = 0, Taiga, Forest, Desert, Swamp, None
}

enum Tier
{
	None = 0, Low = 20, Medium = 10, High = 2
}

class NoiseGeneration:
	var NoiseGenerationAlgorithm: FastNoiseLite
	var RandomNumbers: RandomNumberGenerator

class Biome:
	var AtlasCoordinates: Array
	var AtlasSizes: Array 
	var TileSetSourceIndex: int

class CellInfo:
	var CellType: Cell
	var AtlasIndex: int

class MapData:
	var Cells: Array
	var Biomes: Dictionary
	var MapSize: Vector2i	
	var TilesSet: Array
	var SparseSet: Array
	
@export var MapSizeExport: Vector2i	
@export var m_Offset: Vector2i = Vector2i(21, 10)

var m_NoiseGeneration = NoiseGeneration.new()
var m_MapData = MapData.new()

const m_DarkLayerIndex = 0
const m_LightLayerIndex = 0

func GetMapPosition() -> Vector2i:
	return Vector2i(-m_MapData.MapSize.x / 2, -m_MapData.MapSize.y / 2)

func GetMapSize() -> Vector2i:
	return m_MapData.MapSize

func GetMapPositionInLocalSpace() -> Vector2:
	return $DarkTileMap.map_to_local(GetMapPosition()) + $DarkTileMap.map_to_local(m_Offset);	

func GetMapSizeInLocalSpace() -> Vector2:
	return $DarkTileMap.map_to_local(GetMapSize());

#position is in world space coordinates
func GetPositionInTileSpace(position: Vector2) -> Vector2i:
	return $DarkTileMap.local_to_map(position);

#tile is in tile space coordinates
func ChangeTileToLight(tile: Vector2i):
	var coords = Vector2i(clamp(tile.x, -m_MapData.MapSize.x / 2, m_MapData.MapSize.x / 2), 
						  clamp(tile.y, -m_MapData.MapSize.y / 2, m_MapData.MapSize.y / 2))

	tile = Vector2i(coords.x + m_MapData.MapSize.y / 2, coords.y + m_MapData.MapSize.y / 2)

	var index = tile.x + tile.y * m_MapData.MapSize.x

	var cellInfo = m_MapData.Cells[index]
	var biome = m_MapData.Biomes[cellInfo.CellType]

	var atlasCoords = biome.AtlasCoordinates[cellInfo.AtlasIndex]
	var atlasSize = biome.AtlasSizes[cellInfo.AtlasIndex]

	var xOffset = (atlasSize.x - 1) / 2
	var yOffset = (atlasSize.y - 1) / 2

	var tileCoordinate = Vector2i(coords.x + xOffset, coords.y + yOffset)

	$LightTileMap.set_cell(m_LightLayerIndex, tileCoordinate, biome.TileSetSourceIndex, atlasCoords);


#tile is in tile space coordinates
#radius is also in tile space coordinates
func ChangeTilesToLight(tile: Vector2i, radius: float):
	var minX = tile.x - ceili(radius)
	var maxX = tile.x + ceili(radius)

	var minY = tile.y - ceili(radius)
	var maxY = tile.y + ceili(radius)

	var floatTile = Vector2(tile.x, tile.y)

	for y in range(minY, maxY):
		for x in range(minX, maxX):
			var coordinate = Vector2(x, y)
			
			if coordinate.distance_to(floatTile) <= radius:
				var integerCoordinate = Vector2i(x, y)
				ChangeTileToLight(integerCoordinate)
	


#in tile space
func GetTile(tile: Vector2i) -> Cell:
	var coords = Vector2i(clamp(tile.x, -m_MapData.MapSize.x / 2, m_MapData.MapSize.x / 2),
						  clamp(tile.y, -m_MapData.MapSize.y / 2, m_MapData.MapSize.y / 2))

	tile = Vector2i(coords.x + m_MapData.MapSize.x / 2, coords.y + m_MapData.MapSize.y / 2);

	var index = tile.x + tile.y * m_MapData.MapSize.x;

	return m_MapData.Cells[index].CellType;


#in world space
func GetTileFromWorldSpace(coordinates: Vector2) -> Cell:
	return GetTile(GetPositionInTileSpace(coordinates));

func _ready():
	m_NoiseGeneration = NoiseGeneration.new()
	m_MapData = MapData.new()

	m_NoiseGeneration.RandomNumbers = RandomNumberGenerator.new()
	m_NoiseGeneration.NoiseGenerationAlgorithm = FastNoiseLite.new()

	m_NoiseGeneration.NoiseGenerationAlgorithm.seed = m_NoiseGeneration.RandomNumbers.randi()
	print("Seed: ", m_NoiseGeneration.NoiseGenerationAlgorithm.seed)

	m_MapData.MapSize = MapSizeExport
	m_MapData.Cells = Array()
	m_MapData.TilesSet = Array()
	m_MapData.SparseSet = Array()

	for i in range(0, m_MapData.MapSize.x * m_MapData.MapSize.y):
		m_MapData.Cells.append(CellInfo.new())
		m_MapData.TilesSet.append(false)

	for i in range(0, 5):
		m_MapData.SparseSet.append(i as Cell)

	var lambda = func RandomSort(a, b) -> Cell:
		if m_NoiseGeneration.RandomNumbers.randi() > m_NoiseGeneration.RandomNumbers.randi():
			return a
		else:
			return b

	m_MapData.SparseSet.sort_custom(lambda)

	InitalizeBiomes()
	GenerateWorld()
	UpdateTileMap()

	ChangeTilesToLight(Vector2i(0, 0), 25.0)


func GenerateWorld():
	for y in range(0, m_MapData.MapSize.y):
		for x in range(0, m_MapData.MapSize.x):
			var index = x + y * m_MapData.MapSize.x

			if m_MapData.TilesSet[index]:
				continue;

			var noiseValue = m_NoiseGeneration.NoiseGenerationAlgorithm.get_noise_2d(x, y)
			noiseValue = noiseValue * 0.5 + 0.5

			var cell = m_MapData.SparseSet[floori(noiseValue * m_MapData.SparseSet.size())]
			var biome = m_MapData.Biomes[cell]

			m_MapData.Cells[index] = ConstructCellFromBiome(cell, x, y);

			var atlasSize = biome.AtlasSizes[m_MapData.Cells[index].AtlasIndex];

			for dy in range(0, atlasSize.y):
				for dx in range(0, atlasSize.x):
					var maxIndex = min((dx + x) + (dy + y) * m_MapData.MapSize.x, m_MapData.TilesSet.size() - 1)

					m_MapData.TilesSet[maxIndex] = true
				
func UpdateTileMap():
	for index in range(0, m_MapData.TilesSet.size()):
		m_MapData.TilesSet[index] = false
	
	for y in range(0, m_MapData.MapSize.y):
		for x in range(0, m_MapData.MapSize.x):
			var index = x + y * m_MapData.MapSize.x

			if m_MapData.TilesSet[index]:
				continue

			var coordinate = Vector2i(x - m_MapData.MapSize.x / 2, y - m_MapData.MapSize.y / 2)
			
			var cellInfo = m_MapData.Cells[index]

			var biome = m_MapData.Biomes[cellInfo.CellType]

			var atlasCoords = biome.AtlasCoordinates[cellInfo.AtlasIndex];
			var atlasSize   = biome.AtlasSizes[cellInfo.AtlasIndex];
			
			for dy in range(0, atlasSize.y):
				for dx in range(0, atlasSize.x):
					var maxIndex = min((dx + x) + (dy + y) * m_MapData.MapSize.x, m_MapData.TilesSet.size() - 1)
					m_MapData.TilesSet[maxIndex] = true;

			var xOffset = (atlasSize.x - 1) / 2;
			var yOffset = (atlasSize.y - 1) / 2;

			var tileCoordinate = Vector2i(coordinate.x + xOffset, coordinate.y + yOffset);

			$DarkTileMap.set_cell(m_DarkLayerIndex, tileCoordinate, biome.TileSetSourceIndex, atlasCoords);

		
func InitalizeBiomes():
	var tileSet = $DarkTileMap.tile_set; 

	m_MapData.Biomes = {}

	m_MapData.Biomes[Cell.Tundra] =   CreateBiome(tileSet, 0)
	m_MapData.Biomes[Cell.Taiga]  =   CreateBiome(tileSet, 1)
	m_MapData.Biomes[Cell.Forest] =   CreateBiome(tileSet, 2)
	m_MapData.Biomes[Cell.Desert] =   CreateBiome(tileSet, 3)
	m_MapData.Biomes[Cell.Swamp]  =   CreateBiome(tileSet, 4)


func ConstructCellFromBiome(cell: Cell, x: int, y: int) -> CellInfo:
	var cellInfo = CellInfo.new();
	cellInfo.CellType = cell;

	var biome = m_MapData.Biomes[cellInfo.CellType];

	var noiseValue = m_NoiseGeneration.NoiseGenerationAlgorithm.get_noise_2d(x, y)
	noiseValue = noiseValue * 0.5 + 0.5

	cellInfo.AtlasIndex = floori(noiseValue * biome.AtlasCoordinates.size())

	return cellInfo;


func CreateBiome(tileSet: TileSet, tileSetSourceIndex: int) -> Biome:
	var biome = Biome.new()

	biome.TileSetSourceIndex = tileSetSourceIndex

	var source = tileSet.get_source(biome.TileSetSourceIndex)

	if source != null && source is TileSetAtlasSource:
		biome.AtlasCoordinates = GetAtlasCoordinatesFromSource(source)
		biome.AtlasSizes = GetAtlasSizesFromCoordinates(biome.AtlasCoordinates, source)
	
	return biome

static func GetAtlasCoordinatesFromSource(source: TileSetAtlasSource) -> Array:
	var atlasCoords = Array()

	var gridSize = source.get_atlas_grid_size()

	for x in range(0, gridSize.x):
		for y in range(0, gridSize.y):
			var tileCoordinate = Vector2i(x, y)
			
			if source.has_tile(tileCoordinate):
				atlasCoords.append(tileCoordinate)
			
	return atlasCoords


static func GetAtlasSizesFromCoordinates(coordinates: Array, tileSetSource: TileSetAtlasSource) -> Array:
	var atlasSizes = Array()

	for i in range(0, coordinates.size()):
		atlasSizes.append(tileSetSource.get_tile_size_in_atlas(coordinates[i]))
	
	return atlasSizes;

			
	
