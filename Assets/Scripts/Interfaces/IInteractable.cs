using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
	void Interact();

	virtual void MouseDownAct() { Debug.Log("Shouldn't call MouseDownAct in interface"); }
}
