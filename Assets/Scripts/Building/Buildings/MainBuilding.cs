using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainBuilding : Building, ILootContainer
{
	public Inventory LootCounter { get; set; } = new Inventory() { { LootType.Gold, 500 } };
	List<LootType> _content = new List<LootType>() { LootType.Gold };


	private new void Awake()
	{
		base.Awake();
		data.IsPlaced = true;
		data.Name = "MainBuilding";
		data.Size = new Vector2Int(13, 13);
		data.PanelName = UIManager.PanelNames.MainBuildingP;
	}

	private new void Start()
	{
		base.Start();
		HealthSys.SetHealth(1000);
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
