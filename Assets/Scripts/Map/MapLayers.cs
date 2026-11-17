using MapSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapSpace.Map;
using static UnityEditor.PlayerSettings;


namespace MapSpace
{
	public enum LayersNames { _LootL, _BuildingL, _InitL, _TreeL, _UnitL, _Invalid = 505 }   // Corresponds to _Maps to access them correctly

	public class MapLayers
	{
		

		public Layer<IInteractable> LootL = new Layer<IInteractable>();
		public Layer<Building>	BuildingL { get; private set; } = new Layer<Building>();
		public Layer<bool> EnvL { get; private set; } = new Layer<bool>();   // Removed after initialization
		public Layer<bool> TreeL { get; private set; } = new Layer<bool>();
		public Layer<Unit> UnitL { get; private set; } = new Layer<Unit>();

		public static readonly int Count = 5;

		
		public void RemoveEnvLayer() { EnvL = null; }

		public bool CellEmptyInAllLayers(Vector2Int pos)
		{
			return	LootL.Get(pos) == Layer<ILootContainer>.EmptyCell ||
					BuildingL.Get(pos) == Layer<Building>.EmptyCell ||
					TreeL.Get(pos) == Layer<bool>.EmptyCell ||
					UnitL.Get(pos) == Layer<Unit>.EmptyCell;
		}

		public static List<Vector2Int> GetSquareAreaPoints(Vector2Int center, Vector2Int size)
		{   // all coordinates starts at (0,0), endCellPos can't be negative
			// as the positioning correctness should be checked beforehand
			List<Vector2Int> points = new List<Vector2Int>();
			var halfSize = size / 2;
			var startCellPos = center - halfSize; // min corner
			var endCellPos = center + halfSize;   // max corner

			for (int x = startCellPos.x; x < endCellPos.x; x++)
				for (int z = startCellPos.y; z < endCellPos.y; z++)
				{ points.Add(new Vector2Int(x, z)); }
			return points;
		}

		public IInteractable GetInteractableOnPos(Vector2Int pos)
		{
			if (!Layers.BuildingL.IsEmpty(pos))
			{ return Layers.BuildingL.Get(pos); }
			else if (!Layers.UnitL.IsEmpty(pos))
			{ return Layers.UnitL.Get(pos); }
			else if (!Layers.LootL.IsEmpty(pos))
			{ return Layers.LootL.Get(pos); }
			else if (!Layers.TreeL.IsEmpty(pos))
			{ throw new NotImplementedException(); }
			return null;
		}
	}
}

public class Layer<T>
{
	T[,] l = new T[GetSize()[0], GetSize()[1]];
	public static readonly T EmptyCell = default;


	public T Get(Vector2Int pos) { return l[pos.x, pos.y]; }

	public bool IsEmpty(Vector2Int pos)
	{ return l[pos.x, pos.y].Equals(EmptyCell); }

	public bool TrySetCell(Vector2Int pos, T newCell)
	{
		if (IsOutOfMap(pos) || l[pos.x, pos.y].Equals(EmptyCell))
			return false;
		if (l[pos.x, pos.y].Equals(EmptyCell))
		{
			l[pos.x, pos.y] = newCell;
			return true;
		}
		return false;
	}
	public void CleanCell(Vector2Int pos)
	{ l[pos.x, pos.y] = EmptyCell; }

	public void Reset() { l = new T[Map.GetSize()[0], Map.GetSize()[1]]; }

	public bool SquareAreaTypeIs(T cell, Vector2Int pos, Vector2Int size)
	{
		var points = MapLayers.GetSquareAreaPoints(pos, size);
		foreach (var p in points)
		{
			if (Map.IsOutOfMap(p) || !Map.Layers.CellEmptyInAllLayers(p))
				return false;
		}
		return true;
	}

	public void FillSquareArea(T filling, Vector2Int center, Vector2Int size)
	{
		var points = MapLayers.GetSquareAreaPoints(center, size);

		foreach (var p in points)
		{ l[p.x, p.y] = filling; }
	}

	void AddNearbyCellsToQueue(ref Queue<Vector2Int> queue, Vector2Int currCoords, List<Vector2Int> visited)
	{
		List<int> range = new List<int>() { -1, 0, 1 };
		foreach (int i in range)
		{
			foreach (int j in range)
			{
				if (i == 0 && j == 0) continue;	// skip current position

				Vector2Int newCoords = new Vector2Int(currCoords.x + i, currCoords.y + j);
				if (visited.Contains(newCoords) ||
					queue.Contains(newCoords) ||
					IsOutOfMap(newCoords)) 
					continue;
				queue.Enqueue(newCoords);
			}
		}
	}

	/// <summary>
	/// Fill the area on map (circular filling)
	/// </summary>
	/// <param name="centerCoords"> - coordinates of the center of the area that need to be filled</param>
	/// <param name="radius"> - translate world radius (radius, 0, 0) to map map radius (mapRadius, 0) with WorldToMap method</param>
	/// <param name="filling"> - cell type which will fill the area</param>
	public IEnumerator FillCircleArea(T filling, Vector2Int centerCoords, int radius)
	{
		Queue<Vector2Int> toFill = new Queue<Vector2Int>();
		toFill.Enqueue(centerCoords);
		List<Vector2Int> visited = new List<Vector2Int>();
		AddNearbyCellsToQueue(ref toFill, centerCoords, visited);

		while (toFill.Count > 0)
		{
			var currCell = toFill.Dequeue();
			int dx = currCell.x - centerCoords.x;
			int dy = currCell.y - centerCoords.y;

			if (dx * dx + dy * dy > radius * radius ||
			!l[currCell.x, currCell.y].Equals(EmptyCell) ) 
				continue;
			l[currCell.x, currCell.y] = filling;
			visited.Add(currCell);
			AddNearbyCellsToQueue(ref toFill, currCell, visited);
			yield return null;
		}
	}

	public delegate bool CheckIfDesiredCell(Vector2Int nxtCellPos);

	public Vector2Int FindNearestCell(Vector2Int startCellPos, CheckIfDesiredCell check, 
		Func<List<Vector2Int>, List<Vector2Int>> DirSortFunc)
	{
		var dirs = new List<Vector2Int>() {
				new Vector2Int(1, 0),
				new	Vector2Int	(0, 1),
				new Vector2Int(-1, 0),
				new Vector2Int(0, -1),
				new Vector2Int(1, 1),
				new Vector2Int(1, -1),
				new Vector2Int(-1, 1),
				new Vector2Int(-1, -1),

			};
		dirs = DirSortFunc(dirs);   // Used for sufficient pathfinding.
									// Firtly tries dir which closest to target cell and after this
									// tries others dirs (for example when obsticle on the way)
		// LogList(dirs);
		HashSet<Vector2Int> visited = new HashSet<Vector2Int>() { startCellPos };
		Queue<Vector2Int> queue = new Queue<Vector2Int>();
		queue.Enqueue(startCellPos);

		while (queue.Count > 0)
		{
			var curCell = queue.Dequeue();

			for (int i = 0; i < dirs.Count; i++)
			{
				var nxtCellPos = curCell + dirs[i];

				if (!IsOutOfMap(nxtCellPos) && visited.Add(nxtCellPos))
				{
					if (check(nxtCellPos))
						return nxtCellPos;
					queue.Enqueue(nxtCellPos);
				}
			}
		}
		return new Vector2Int(MapData.MapSize[0], MapData.MapSize[1]);    // Out of map
	}

	/// <summary>
	/// Looks for nearest non-empty cell in the specific radius
	/// </summary>
	/// <param name="centerCoords"></param>
	/// <param name="radius"></param>
	/// <returns></returns>
	public Vector2Int FindNearestCell(Vector2Int centerCoords, int radius)
	{
		Queue<Vector2Int> queue = new Queue<Vector2Int>();
		List<Vector2Int> visited = new List<Vector2Int>();
		AddNearbyCellsToQueue(ref queue, centerCoords, visited);

		while (queue.Count > 0)
		{
			var currCell = queue.Dequeue();
			visited.Add(currCell);
			int dx = currCell.x - centerCoords.x;
			int dy = currCell.y - centerCoords.y;
			if (dx * dx + dy * dy > radius * radius) continue;

			if (l != null && !l[currCell.x, currCell.y].Equals(EmptyCell))
				return currCell;

			AddNearbyCellsToQueue(ref queue, currCell, visited);
		}
		return centerCoords;
	}
}