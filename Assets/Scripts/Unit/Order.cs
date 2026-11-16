using UnityEngine;


public enum ExeState { Running, Succeeded, Failed }

public class Order
{
	public bool Completed { get; private set; } = false;


	public Order(IInteractable obj)
	{  }

	public void Update()
	{

	}

}

