using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapSpace
{
	public struct ChunkData
	{
		public static readonly Vector2Int Size = new Vector2Int(64, 64);
		public List<GameObject> Trees;
		public Vector2Int MapPos;
		public bool Initialized;
		public bool IsEnabled;
	}
}

