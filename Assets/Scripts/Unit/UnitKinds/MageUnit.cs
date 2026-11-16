using UnityEngine;
using System;


public class MageUnit : UnitSelf<MageUnit>
{
	private new void Awake()
	{
		base.Awake();
		data.Name = "MageUnit";
	}

	new void Start()
	{
		base.Start();
	}

	new void Update()
	{
		base.Update();
	}
	public override void Interact()
	{
		Debug.Log("Interact not ready yet. MageUNIT");
	}

}