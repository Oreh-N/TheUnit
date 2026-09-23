using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class Chest : LootHolder, IMyPlaceableOnMap
{
	public Vector3 GetPos()
	{
		return transform.position;
	}

	public Vector2Int GetSize()
	{
		var box = transform.GetComponent<BoxCollider>();
		Vector2Int takeAreaSize = new Vector2Int((int)box.size.x, (int)box.size.z+1);
		return takeAreaSize;
	}

	private void Awake()
	{
        inventory._maxLootCount = 200;
	}
	void Start()
    {
         inventory.FillFully(LootType.Gold);
    }

    private new void Update()
    {
        base.Update();
    }
}
