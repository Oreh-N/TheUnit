using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warehouse : Building, ILootContainer
{
	public List<LootType> _content { get; protected set; } = new List<LootType>() { LootType.Wood };
	public Inventory LootCounter { get; set; } = new Inventory() { { LootType.Wood, 0 } };


	private new void Start()
	{
		base.Start();
		//data.Panel = UIManager.Instance.GetPanelWithTag(PubNames.WarehousePanelTag);
		data.Name = "Warehouse0";
		data.Size = new Vector2Int(7, 7);
	}

	private new void Update()
	{
		base.Update();
	}



	public Inventory GetTeamInventory()
	{
		return LootCounter;
	}
}
