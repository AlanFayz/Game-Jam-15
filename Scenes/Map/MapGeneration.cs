using Godot;
using System;
using System.Collections.Generic;

public partial class MapGeneration : Node
{
	enum Cell
	{
		None = 0, Water, Grass, Stone //TODO: more to add
	}

	struct NoiseGeneration
	{
		public FastNoiseLite NoiseGenerationAlgorithm;
		public RandomNumberGenerator RandomNumberGenerator;
	}

	struct MapData
	{
		public List<Cell> Cells;
		public Dictionary<Cell, TileData> Tiles;
		public TileMap TileMap;
		public Vector2 MapSize;
	}

	private NoiseGeneration	m_NoiseGeneration;
	private MapData m_MapData;

	public override void _Ready()
	{
		m_NoiseGeneration = new NoiseGeneration();
		m_MapData = new MapData();

		m_NoiseGeneration.RandomNumberGenerator = new RandomNumberGenerator();
		m_NoiseGeneration.NoiseGenerationAlgorithm = new FastNoiseLite();

		m_MapData.TileMap = GetNode<TileMap>("TileMap");
		m_MapData.MapSize = new Vector2(50.0f, 50.0f);
		m_MapData.Tiles   = new Dictionary<Cell, TileData>();
		m_MapData.Cells   = new List<Cell>((int)(m_MapData.MapSize.X * m_MapData.MapSize.Y));

	}

	public override void _Process(double delta)
	{
	}

	public void GenerateWorld()
	{
		//TODO
	}

	public void GenerateItems()
	{
		//TODO
	}

	public void GenerateMobs()
	{
		//TODO: should reference a mob class which needs to be created.
	}

}
