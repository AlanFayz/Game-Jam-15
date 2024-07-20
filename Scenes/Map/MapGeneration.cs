using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class MapGeneration : Node
{
	private enum Cell
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
		public TileMap TileMap;
		public Vector2I MapSize;
		public List<bool> TileSet;
		public List<Cell> SparseSet;
	}

	private NoiseGeneration m_NoiseGeneration;
	private MapData m_MapData;

	private const int m_LayerIndex = 0;

	public override void _Ready()
	{
		m_NoiseGeneration = new NoiseGeneration();
		m_MapData = new MapData();

		m_NoiseGeneration.RandomNumberGenerator    = new RandomNumberGenerator();
		m_NoiseGeneration.NoiseGenerationAlgorithm = new FastNoiseLite();

		m_NoiseGeneration.NoiseGenerationAlgorithm.Seed = (int)m_NoiseGeneration.RandomNumberGenerator.Randi();
		GD.Print("Seed: ", m_NoiseGeneration.NoiseGenerationAlgorithm.Seed);

		m_MapData.OccupiedLootCoords = new HashSet<Vector2I>();
		m_MapData.MapSize = new Vector2I(1000, 1000);
		m_MapData.Cells   = new List<CellInfo>();
		m_MapData.TileSet = new List<bool>();
		m_MapData.TileMap = GetNode<TileMap>("MainTileMap");
		m_MapData.SparseSet = new List<Cell>();

		for (int i = 0; i < m_MapData.MapSize.X * m_MapData.MapSize.Y; i++)
		{
			m_MapData.Cells.Add(new CellInfo());
			m_MapData.TileSet.Add(false);
		}

		for (int i = 0; i < 5; i++)
			m_MapData.SparseSet.Add((Cell)i);

		m_MapData.SparseSet = m_MapData.SparseSet.OrderBy(_ => m_NoiseGeneration.RandomNumberGenerator.Randi()).ToList();

		InitalizeBiomes();
		GenerateWorld();
		UpdateTileMap();
	}

	public override void _Process(double delta)
	{
	}

	public void GenerateWorld()
	{
		int yCount = 0;
		int xCount = 0;

		for (int y = 0; y < m_MapData.MapSize.Y; y++)
		{
			yCount++;

			for (int x = 0; x < m_MapData.MapSize.X; x++)
			{
				xCount++;
				int index = x + y * m_MapData.MapSize.X;

				if (m_MapData.TileSet[index])
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
						int maxIndex = Math.Min((dx + x) + (dy + y) * m_MapData.MapSize.X, m_MapData.TileSet.Count - 1);

						m_MapData.TileSet[maxIndex] = true;
					}
				}

				if (atlasSize.X > 1 || atlasSize.Y > 1)
				{
					m_NoiseGeneration.NoiseGenerationAlgorithm.Seed = (int)noiseValue;
				}
			}
		}

		for (int i = 0; i < m_MapData.TileSet.Count; i++)
		{
			m_MapData.TileSet[i] = false;
		}
	}

	public void GenerateItems()
	{
		//TODO
	}

	public void GenerateMobs()
	{
		//TODO: should reference a mob class which needs to be created.
	}

	private void UpdateTileMap()
	{
		for (int y = 0; y < m_MapData.MapSize.Y; y++)
		{
			for (int x = 0; x < m_MapData.MapSize.X; x++)
			{
				if (m_MapData.TileSet[x + y * m_MapData.MapSize.X])
					continue;

				Vector2I coordinate = new Vector2I(x - m_MapData.MapSize.X / 2, y - m_MapData.MapSize.Y / 2);
				
				CellInfo cellInfo = m_MapData.Cells[x + y * m_MapData.MapSize.X];

				Biome biome = m_MapData.Biomes[cellInfo.Cell];

				Vector2I atlasCoords = biome.AtlastCoordinates[cellInfo.AtlasIndex];
				Vector2I atlasSize   = biome.AtlasSizes[cellInfo.AtlasIndex];

				
				for (int dy = 0; dy < atlasSize.Y; dy++)
				{
					for (int dx = 0; dx < atlasSize.X; dx++)
					{
						m_MapData.TileSet[(dx + x) + (dy + y) * m_MapData.MapSize.X] = true;
					}
				}

				int xOffset = (atlasSize.X - 1) / 2;
				int yOffset = (atlasSize.Y - 1) / 2;

				Vector2I tileCoordinate = new Vector2I(coordinate.X + xOffset, coordinate.Y + yOffset);

				m_MapData.TileMap.SetCell(m_LayerIndex, tileCoordinate, biome.TileSetSource, atlasCoords);

			}
		}
	}
	private void InitalizeBiomes()
	{
		TileSet tileSet = m_MapData.TileMap.TileSet;

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
