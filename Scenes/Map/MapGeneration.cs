using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public partial class MapGeneration : Node
{
	private enum Cell
	{
		None = 0, Water, Grass, Stone
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

	private struct MapData
	{
		public List<Cell> Cells;
		public Dictionary<Cell, Vector2I> TileCoordinates;
		public HashSet<Vector2I> OccupiedLootCoords;
		public TileMap TileMap;
		public Vector2I MapSize;
	}

	private NoiseGeneration m_NoiseGeneration;
	private MapData m_MapData;

	private const int m_LayerIndex = 0;

	public override void _Ready()
	{
		m_NoiseGeneration = new NoiseGeneration();
		m_MapData = new MapData();

		m_NoiseGeneration.RandomNumberGenerator = new RandomNumberGenerator();
		m_NoiseGeneration.NoiseGenerationAlgorithm = new FastNoiseLite();

		m_NoiseGeneration.NoiseGenerationAlgorithm.Seed = (int)m_NoiseGeneration.RandomNumberGenerator.Randi();

		m_MapData.OccupiedLootCoords = new HashSet<Vector2I>();
		m_MapData.TileMap = GetNode<TileMap>("TileMap");
		m_MapData.MapSize = new Vector2I(100, 100);
		m_MapData.Cells = new List<Cell>();

		for (int i = 0; i < m_MapData.MapSize.X * m_MapData.MapSize.Y; i++)
			m_MapData.Cells.Add(Cell.Water);

		CreateTileCoordinateDictionary();
		GenerateWorld();
		UpdateTileMap();
	}

	public override void _Process(double delta)
	{
	}

	public void GenerateWorld()
	{
		for (int x = 0; x < m_MapData.MapSize.X; x++)
		{
			for (int y = 0; y < m_MapData.MapSize.Y; y++)
			{
				float noiseValue = m_NoiseGeneration.NoiseGenerationAlgorithm.GetNoise2D((float)x, (float)y);
				noiseValue = (noiseValue + 1.0f) / 2.0f;

				Cell cell = (Cell)Mathf.FloorToInt(noiseValue * (float)m_MapData.TileCoordinates.Count);

				m_MapData.Cells[x + y * m_MapData.MapSize.X] = cell;

				SpawnLoot(x, y, cell);
			}
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
		for (int x = 0; x < m_MapData.MapSize.X; x++)
		{
			for (int y = 0; y < m_MapData.MapSize.Y; y++)
			{
				Vector2I coordinate = new Vector2I(x - m_MapData.MapSize.X / 2, y - m_MapData.MapSize.Y / 2);
				Cell cell = m_MapData.Cells[x + y * m_MapData.MapSize.X];
				Vector2I atlasCoords = m_MapData.TileCoordinates[cell];

				m_MapData.TileMap.SetCell(m_LayerIndex, coordinate, 0, atlasCoords);
			}
		}
	}

	//Might change to list so we can create some biases for different biomes
	private void CreateTileCoordinateDictionary()
	{
		m_MapData.TileCoordinates = new Dictionary<Cell, Vector2I>();

		//NOTE: due to change to actual tile map we want
		m_MapData.TileCoordinates.Add(Cell.None, new Vector2I(1, 5));
		m_MapData.TileCoordinates.Add(Cell.Water, new Vector2I(11, 1));
		m_MapData.TileCoordinates.Add(Cell.Grass, new Vector2I(9, 2));
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

	static private Tier GetTierFromCell(Cell cell)
	{
		switch (cell)
		{
			case Cell.Grass: return Tier.Medium;
			case Cell.Stone: return Tier.Low;
			case Cell.Water:
			case Cell.None:
			default: return Tier.None;
		}
	}
}
