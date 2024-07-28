using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
public partial class MapGeneration : Node
{
	public enum Cell
	{
		Tundra = 0, Taiga, Forest, Desert, Swamp, None
	}

	private enum Tier
	{
		None = 0, Low = 20, Medium = 10, High = 2
	}

	private struct NoiseGeneration
	{
		public FastNoiseLite NoiseGenerationAlgorithm;
		public RandomNumberGenerator RandomNumberGenerator;
	}

	private struct Biome
	{
		public List<Vector2I> AtlastCoordinates; 
		public List<Vector2I> AtlasSizes;
		public int TileSetSource;
	}

	private struct CellInfo
	{
		public Cell Cell;
		public int AtlasIndex;
	}

	private struct MapData
	{
		public List<CellInfo> Cells;
		public Dictionary<Cell, Biome> Biomes;
		public HashSet<Vector2I> OccupiedLootCoords;
		public TileMap LightTileMap;
		public TileMap DarkTileMap;
		public Vector2I MapSize;
		public List<bool> TilesSet;
		public List<Cell> SparseSet;
	}

	[Export]
	public Vector2I MapSizeExport;

	private Vector2I m_Offset = new Vector2I(21, 10);
	private NoiseGeneration m_NoiseGeneration;
	private MapData m_MapData;

	private const int m_DarkLayerIndex = 0;
	private const int m_LightLayerIndex = 0;

	public Vector2I GetMapPosition()
	{
		return new Vector2I(-m_MapData.MapSize.X / 2, -m_MapData.MapSize.Y / 2);
	}

	public Vector2I GetMapSize()
	{
		return m_MapData.MapSize;
	}

	public Vector2 GetMapPositionInLocalSpace()
	{
		return m_MapData.DarkTileMap.MapToLocal(GetMapPosition()) + m_MapData.DarkTileMap.MapToLocal(m_Offset);	
	}

	public Vector2 GetMapSizeInLocalSpace()
	{
		return m_MapData.DarkTileMap.MapToLocal(GetMapSize());
	}

	/*
		position is in world space coordinates
	*/
	public Vector2I GetPositionInTileSpace(Vector2 position)
	{
		return m_MapData.DarkTileMap.LocalToMap(position);
	}

	/*
		tile is in tile space coordinates
	*/

	public void ChangeTileToLight(Vector2I tile)
	{
		Vector2I coords = new Vector2I(Mathf.Clamp(tile.X, -m_MapData.MapSize.X / 2, m_MapData.MapSize.X / 2),
									   Mathf.Clamp(tile.Y, -m_MapData.MapSize.Y / 2, m_MapData.MapSize.Y / 2));

		tile = new Vector2I(coords.X + m_MapData.MapSize.X / 2, coords.Y + m_MapData.MapSize.Y / 2);

		int index = tile.X + tile.Y * m_MapData.MapSize.X;

		CellInfo cellInfo = m_MapData.Cells[index];
		Biome biome = m_MapData.Biomes[cellInfo.Cell];

		Vector2I atlasCoords = biome.AtlastCoordinates[cellInfo.AtlasIndex];
		Vector2I atlasSize = biome.AtlasSizes[cellInfo.AtlasIndex];

		int xOffset = (atlasSize.X - 1) / 2;
		int yOffset = (atlasSize.Y - 1) / 2;

		Vector2I tileCoordinate = new Vector2I(coords.X + xOffset, coords.Y + yOffset);

		m_MapData.LightTileMap.SetCell(m_LightLayerIndex, tileCoordinate, biome.TileSetSource, atlasCoords);
	}


	/*
		tile is in tile space coordinates
		radius is also in tile space coordinates
	*/
	public void ChangeTilesToLight(Vector2I tile, float radius)
	{
		int minX = tile.X - Mathf.CeilToInt(radius);
		int maxX = tile.X + Mathf.CeilToInt(radius);

		int minY = tile.Y - Mathf.CeilToInt(radius);
		int maxY = tile.Y + Mathf.CeilToInt(radius);

		Vector2 integerTile = new Vector2((float)tile.X, (float)tile.Y);

		for(int y = minY; y < maxY; y++)
		{
			for(int x = minX; x < maxX; x++)
			{
				Vector2 coordinate = new Vector2((float)x, (float)y);

				if (coordinate.DistanceTo(integerTile) <= radius)
				{
					Vector2I integerCoordinate = new Vector2I(x, y);
					ChangeTileToLight(integerCoordinate);
				}
			}
		}

	}

	/*
		in tile space
	*/
	public Cell GetTile(Vector2I tile)
	{
        Vector2I coords = new Vector2I(Mathf.Clamp(tile.X, -m_MapData.MapSize.X / 2, m_MapData.MapSize.X / 2),
                                       Mathf.Clamp(tile.Y, -m_MapData.MapSize.Y / 2, m_MapData.MapSize.Y / 2));

        tile = new Vector2I(coords.X + m_MapData.MapSize.X / 2, coords.Y + m_MapData.MapSize.Y / 2);

		int index = tile.X + tile.Y * m_MapData.MapSize.X;

		return m_MapData.Cells[index].Cell;
	}

	/*
		in world space
	*/
	public Cell GetTile(Vector2 coordinates)
	{
		return GetTile(GetPositionInTileSpace(coordinates));
	}

	public override void _Ready()
	{
		m_NoiseGeneration = new NoiseGeneration();
		m_MapData = new MapData();

		m_NoiseGeneration.RandomNumberGenerator    = new RandomNumberGenerator();
		m_NoiseGeneration.NoiseGenerationAlgorithm = new FastNoiseLite();

		m_NoiseGeneration.NoiseGenerationAlgorithm.Seed = (int)m_NoiseGeneration.RandomNumberGenerator.Randi();
		GD.Print("Seed: ", m_NoiseGeneration.NoiseGenerationAlgorithm.Seed);

		m_MapData.OccupiedLootCoords = new HashSet<Vector2I>();
		m_MapData.MapSize = MapSizeExport;
		m_MapData.Cells = new List<CellInfo>();
		m_MapData.TilesSet = new List<bool>();
		m_MapData.DarkTileMap  = GetNode<TileMap>("DarkTileMap");
		m_MapData.LightTileMap = GetNode<TileMap>("LightTileMap");
		m_MapData.SparseSet = new List<Cell>();

		for (int i = 0; i < m_MapData.MapSize.X * m_MapData.MapSize.Y; i++)
		{
			m_MapData.Cells.Add(new CellInfo());
			m_MapData.TilesSet.Add(false);
		}

		for (int i = 0; i < 5; i++)
			m_MapData.SparseSet.Add((Cell)i);

		m_MapData.SparseSet = m_MapData.SparseSet.OrderBy(_ => m_NoiseGeneration.RandomNumberGenerator.Randi()).ToList();

		InitalizeBiomes();
		GenerateWorld();
		UpdateTileMap();

		ChangeTilesToLight(new Vector2I(0, 0), 25.0f);

	}

	public override void _Process(double delta)
	{
	}

	public void GenerateWorld()
	{
		for (int y = 0; y < m_MapData.MapSize.Y; y++)
		{
			for (int x = 0; x < m_MapData.MapSize.X; x++)
			{
				int index = x + y * m_MapData.MapSize.X;

				if (m_MapData.TilesSet[index])
					continue;

				float noiseValue = m_NoiseGeneration.NoiseGenerationAlgorithm.GetNoise2D((float)x, (float)y);
				noiseValue = noiseValue * 0.5f + 0.5f;

				Cell cell = m_MapData.SparseSet[Mathf.FloorToInt(noiseValue * (float)m_MapData.SparseSet.Count)];
				Biome biome = m_MapData.Biomes[cell];

				m_MapData.Cells[index] = ConstructCellFromBiome(cell, x, y);

				Vector2I atlasSize = biome.AtlasSizes[m_MapData.Cells[index].AtlasIndex];

				for (int dy = 0; dy < atlasSize.Y; dy++)
				{
					for (int dx = 0; dx < atlasSize.X; dx++)
					{
						int maxIndex = Math.Min((dx + x) + (dy + y) * m_MapData.MapSize.X, m_MapData.TilesSet.Count - 1);

						m_MapData.TilesSet[maxIndex] = true;
					}
				}
			}
		}
	}

	private void UpdateTileMap()
	{
		Parallel.For(0, m_MapData.TilesSet.Count - 1, (int index) => 
		{
			m_MapData.TilesSet[index] = false;
		});

		for (int y = 0; y < m_MapData.MapSize.Y; y++)
		{
			for (int x = 0; x < m_MapData.MapSize.X; x++)
			{
				int index = x + y * m_MapData.MapSize.X;

				if (m_MapData.TilesSet[index])
					continue;

				Vector2I coordinate = new Vector2I(x - m_MapData.MapSize.X / 2, y - m_MapData.MapSize.Y / 2);
				
				CellInfo cellInfo = m_MapData.Cells[index];

				Biome biome = m_MapData.Biomes[cellInfo.Cell];

				Vector2I atlasCoords = biome.AtlastCoordinates[cellInfo.AtlasIndex];
				Vector2I atlasSize   = biome.AtlasSizes[cellInfo.AtlasIndex];

				
				for (int dy = 0; dy < atlasSize.Y; dy++)
				{
					for (int dx = 0; dx < atlasSize.X; dx++)
					{
						int maxIndex = Math.Min((dx + x) + (dy + y) * m_MapData.MapSize.X, m_MapData.TilesSet.Count - 1);
						
						m_MapData.TilesSet[maxIndex] = true;
					}
				}

				int xOffset = (atlasSize.X - 1) / 2;
				int yOffset = (atlasSize.Y - 1) / 2;

				Vector2I tileCoordinate = new Vector2I(coordinate.X + xOffset, coordinate.Y + yOffset);

				m_MapData.DarkTileMap.SetCell(m_DarkLayerIndex, tileCoordinate, biome.TileSetSource, atlasCoords);

			}
		}
	}
	private void InitalizeBiomes()
	{
		TileSet tileSet = m_MapData.DarkTileMap.TileSet; //doesn't matter which one both are identical apart from material used

		m_MapData.Biomes = new Dictionary<Cell, Biome>(12);

		m_MapData.Biomes.Add(Cell.Tundra, CreateBiome(tileSet, 0));
		m_MapData.Biomes.Add(Cell.Taiga,  CreateBiome(tileSet, 1));
		m_MapData.Biomes.Add(Cell.Forest, CreateBiome(tileSet, 2));
		m_MapData.Biomes.Add(Cell.Desert, CreateBiome(tileSet, 3));
		m_MapData.Biomes.Add(Cell.Swamp,  CreateBiome(tileSet, 4));
	}

	private void SpawnLoot(int x, int y, Cell cell)
	{
		Vector2I currentCoordinate = new Vector2I(x, y);

		if (m_MapData.OccupiedLootCoords.Contains(currentCoordinate))
			return;

		Tier tier = GetTierFromCell(cell);

		int choice = m_NoiseGeneration.RandomNumberGenerator.RandiRange(0, 100);

		if ((int)tier <= choice)
		{
			//Spawn the loot
			m_MapData.OccupiedLootCoords.Add(currentCoordinate);

			//TODO: need to get some sort of loot system before implementing
		}

	}

	private CellInfo ConstructCellFromBiome(Cell cell, int x, int y)
	{
		CellInfo cellInfo = new CellInfo();
		cellInfo.Cell = cell;

		Biome biome = m_MapData.Biomes[cellInfo.Cell];

		float noiseValue = m_NoiseGeneration.NoiseGenerationAlgorithm.GetNoise2D((float)x, (float)y);
		noiseValue = noiseValue * 0.5f + 0.5f;

		cellInfo.AtlasIndex = Mathf.FloorToInt(noiseValue * (float)biome.AtlastCoordinates.Count);

		return cellInfo;
	}

	private Biome CreateBiome(TileSet tileSet, int tileSetSourceIndex)
	{
		Biome biome = new Biome();

		biome.AtlastCoordinates = new List<Vector2I>();
		biome.TileSetSource = tileSetSourceIndex;

		TileSetSource source = tileSet.GetSource(biome.TileSetSource);

		if (source != null && source is TileSetAtlasSource)
		{
			TileSetAtlasSource atlasSource = (TileSetAtlasSource)source;

			biome.AtlastCoordinates.AddRange(GetAtlasCoordinatesFromSource(atlasSource));
			biome.AtlasSizes = GetAtlasSizesFromCoordinates(ref biome.AtlastCoordinates, ref atlasSource);
		}

		return biome;
	}

	static private Tier GetTierFromCell(Cell cell)
	{
		switch (cell)
		{
			case Cell.Taiga:   return Tier.High;
			case Cell.Tundra:  return Tier.Medium;
			case Cell.Desert:  return Tier.Low;
			case Cell.Swamp:
			case Cell.None:
			default:		   return Tier.None;
		}
	}

	static private List<Vector2I> GetAtlasCoordinatesFromSource(TileSetAtlasSource source)
	{
		List<Vector2I> atlasCoords = new List<Vector2I>();

		Vector2I gridSize = source.GetAtlasGridSize();

		for(int x = 0; x < gridSize.X; x++)
		{
			for(int y = 0; y < gridSize.Y; y++)
			{
				Vector2I tileCoordinate = new Vector2I(x, y);
				
				if(source.HasTile(tileCoordinate))
				{
					atlasCoords.Add(tileCoordinate);
				}
			}
		}

		return atlasCoords;
	}

	static private List<Vector2I> GetAtlasSizesFromCoordinates(ref List<Vector2I> coordinates, ref TileSetAtlasSource tileSetSource)
	{
		List<Vector2I> atlasSizes = new List<Vector2I>();

		for (int i = 0; i < coordinates.Count; i++)
		{
			atlasSizes.Add(tileSetSource.GetTileSizeInAtlas(coordinates[i]));
		}
		
		return atlasSizes;
	}

}
