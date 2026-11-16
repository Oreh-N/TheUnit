using System;
using System.Collections;
using System.Collections.Generic;


public class UnitData
{
	public UIManager.PanelNames PanelName = UIManager.PanelNames.UnitP;
	public Inventory LootCounter = new Inventory();
	public int TeamID;
	
	public bool NowInteracting;
	public string Name;

	public int HolderCapacity = 2;
	public float Health;

}

