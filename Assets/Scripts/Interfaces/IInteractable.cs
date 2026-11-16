using UnityEngine;



public interface IInteractable
{
	void Interact();

	virtual void MouseDownAct() { Debug.Log("Shouldn't call MouseDownAct in interface"); }

}
