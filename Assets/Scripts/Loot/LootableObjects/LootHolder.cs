using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LootHolder : MonoBehaviour, ILootContainer
{
	public Inventory inventory { get; set; } = new Inventory();
	public bool IsEmpty { get; protected set; } = false;


	public void Update()
	{
		if (IsEmpty) Destroy(this);
	}


	public Inventory GetTeamInventory()
	{
		return inventory;
	}
}
