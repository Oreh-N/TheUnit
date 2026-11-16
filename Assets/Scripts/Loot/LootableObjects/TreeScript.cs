using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TreeScript : LootHolder
{
	private void Awake()
	{
		inventory.FillFully(LootType.Wood);
	}

	private new void Update()
	{
		base.Update();
	}

}
