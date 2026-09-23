using System.Collections.Generic;
using UnityEngine;

public class Inventory : Dictionary<LootType, int>
{
	public int _maxLootCount = 20;


	public void FillFully(LootType lootT) { Add(lootT, _maxLootCount); }

	public void MoveLootTo(Inventory goalInv)
	{
		foreach(var pair in this)
		{ goalInv.ExtAdd(pair.Key, pair.Value); }
		Clear();
	}

	public void MoveSpecificLootTo(Inventory goalInv, IEnumerable<LootType> lootToTrans)
	{
		foreach (var pair in this)
		{ goalInv.ExtAdd(pair.Key, pair.Value); }
	}

	void ExtAdd(LootType key, int value) 
	{
		if (ContainsKey(key)) this[key] += value;
		else this[key] = value;
	}

	public void InitInventory()
	{
		Add(LootType.Gold, 0);
		Add(LootType.Wood, 0);
	}

	public override string ToString() 
	{
		string strInv = "";
		foreach (var loot in this)
		{ strInv += $"{loot.Key}: {loot.Value}\n"; }
		return strInv;
	}
}
