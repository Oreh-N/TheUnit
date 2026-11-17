using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rnd = UnityEngine.Random;


namespace MapSpace
{
	public static class ForestGenerator
	{

		static float treeGenFrequency = 5;

		public static IEnumerator GenVirtForest()
		{
			if (!MainController.Instance.Ready) yield return null;

			Dictionary<Vector3, float> areasInfo = EnvManager.Instance.GetBaseAreaInfo();

			for (int x = 0; x < Map.GetSize().x; x++)
			{
				for (int z = 0; z < Map.GetSize().y; z++)
				{
					var pos = new Vector2Int(x, z);
					if (Rnd.Range(0, 100) < treeGenFrequency && Map.Layers.CellEmptyInAllLayers(pos))
					{ Map.Layers.TreeL.TrySetCell(pos, true); }
				}
				yield return null;
			}
		}

		public static void GenWholeMapForest(GameObject treePrefab)
		{
			if (treePrefab == null)
			{
				Debug.Log("Not initialized tree prefab");
				return;
			}

			for (int x = 0; x < Map.GetSize().x; x++)
			{
				for (int z = 0; z < Map.GetSize().y; z++)
				{
					if (Map.Layers.TreeL.Get(new Vector2Int(x, z)) == true)
					{ Creator.CreateTree(Map.MapToWorld(x, z)); }
				}
			}
		}
	}
}

// 03.07. Trying to optimize tree spawning.Right now might cause spikes in fps and take ~30 ms
// 