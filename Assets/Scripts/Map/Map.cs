using System;
using System.Collections.Generic;
using UnityEngine;
using MapCoord = UnityEngine.Vector2Int;


namespace MapSpace
{
	public static class Map
	{
		public static readonly MapLayers Layers = new MapLayers();
		static MapData data = new MapData();

		static Map()
		{
			//for (int i = 0; i < MapData.Layers.Count; i++)
			//{
			//	MapData.Layers[i] = new CellType[MapData.MapSize[0], MapData.MapSize[1]];
			//	ResetMap((LayersNames)i);
			//}
			MapData.Ready = true;
		}

		public static IInteractable GetGameObjectOnMap(MapCoord mapPos)
		{// If player didn't hit needed cell preciesly then we will look if this cell was near the click position and return it
			int search_radius = 4;

			if (Layers.CellEmptyInAllLayers(mapPos))	
			{ mapPos = Layers.BuildingL.FindNearestCell(mapPos, search_radius); }

			 return Layers.GetInteractableOnPos(mapPos);
		}

		public static bool Ready() { return MapData.Ready; }


		#region Debug
		public static void LogList<T>(List<T> list)
		{
			Debug.Log("List______________________");
			foreach (var i in list)
			{
				Debug.Log($"Item {i.GetType()}");
			}
			Debug.Log("__________________________");

		}
		#endregion


		public static List<MapCoord> GetSquareAreaPoints(MapCoord center, Vector2Int size)
		{   // all coordinates starts at (0,0), endCellPos can't be negative
			// as the positioning correctness should be checked beforehand
			List<MapCoord> points = new List<MapCoord>();
			var halfSize = size / 2;
			var startCellPos = center - halfSize; // min corner
			var endCellPos = center + halfSize;   // max corner

			for (int x = startCellPos.x; x < endCellPos.x; x++)
				for (int z = startCellPos.y; z < endCellPos.y; z++)
				{ points.Add(new MapCoord(x, z)); }
			return points;
		}


		private static void AddNearbyCellsToQueue(ref Queue<MapCoord> queue, MapCoord currCoords, List<MapCoord> visited = null)
		{
			List<int> range = new List<int>() { -1, 0, 1 };
			foreach (int i in range)
			{
				foreach (int j in range)
				{
					if (i == 0 && j == 0) continue;
					MapCoord newCoords = new MapCoord(currCoords.x + i, currCoords.y + j);
					if (IsOutOfMap(newCoords) || (visited != null && visited.Contains(newCoords))) continue;
					queue.Enqueue(newCoords);
				}
			}
		}

		public static bool IsOutOfMap(MapCoord mapCoord)
		=> !(mapCoord.x < MapData.MapSize[0] && mapCoord.x >= 0
			&& mapCoord.y < MapData.MapSize[1] && mapCoord.y >= 0);

		public static bool IsOutOfMap(Vector3 coord) => IsOutOfMap(WorldToMap(new Vector3(coord.x, 0, coord.z)));

		/// <summary>
		/// Convert map index to world position (vertex of the cell)
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>World position</returns>
		public static Vector3 MapToWorld(int x, int y) => data.MapStart + new Vector3(x, 0, y);

		public static Vector3 MapToWorld(MapCoord map_pos) => data.MapStart + new Vector3(map_pos.x, 0, map_pos.y);

		public static Vector3 GetCellSize() => data.CellSize;

		public static Vector2Int GetSize() => MapData.MapSize;

		/// <summary>
		/// Convert world position to map index
		/// </summary>
		/// <param name="pos"></param>
		/// <returns>Map position (indicies)</returns>
		public static MapCoord WorldToMap(Vector3 pos)
		{
			int x = Mathf.FloorToInt(pos.x - data.MapStart.x);
			int z = Mathf.FloorToInt(pos.z - data.MapStart.z);


			if (IsOutOfMap(new MapCoord(x, z)))
			{
				return WorldToMapWithCut(pos);
			}

			return new MapCoord(x, z);
		}

		/// <summary>
		/// Convert world position to map index. If given position is out of map, 
		/// then the position will be cutted to fit into the map.
		/// </summary>
		/// <param name="pos"></param>
		/// <returns>Map position (indicies)</returns>
		public static MapCoord WorldToMapWithCut(Vector3 pos)
		{
			int x = Mathf.FloorToInt(pos.x - data.MapStart.x);
			int z = Mathf.FloorToInt(pos.z - data.MapStart.z);

			if (x < 0) { x = 0; }
			else if (x >= MapData.MapSize[0]) { x = MapData.MapSize[0] - 1; }
			if (z < 0) { z = 0; }
			else if (z >= MapData.MapSize[1]) { z = MapData.MapSize[1] - 1; }

			return new MapCoord(x, z);
		}

	}
}
