using Godot;
using System;
using System.Collections.Generic;

public partial class MapGeneration : Node
{
	enum Cells
	{
		None = 0, Water, Grass, Stone //TODO: more to add
	}

	struct NoiseGeneration
	{
		public FastNoiseLite p_NoiseGenerationAlgorithm;
		public RandomNumberGenerator p_RandomNumberGenerator;
	}

	private NoiseGeneration	m_NoiseGeneration;
	private List<Cells>	m_Cells;
	private Dictionary<Cells, TileData> m_Tiles;

	public override void _Ready()
	{
		
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
