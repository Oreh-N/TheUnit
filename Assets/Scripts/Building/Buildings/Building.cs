using MapSpace;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public abstract class Building : MonoBehaviour, IConstructable, IHavePanel, ITeamMember
{
	protected BuildingData data = new BuildingData();
	public HealthSystem HealthSys { get; protected set; } = new HealthSystem();



	public void Awake()
	{
		HealthSys.SetHealth(100);
		data.RendererChildren = GetComponentsInChildren<Renderer>();
		if (data.RendererChildren == null) Debug.Log("No renderers in this building");
	}

	public void Start()
	{
	}

	public void Update()
	{
		if (HealthSys.GetHealth() <= 0)
		{ Destroy(gameObject); }

		if (data.BuildingSlider)
		{
			data.BuildingSlider.value += Time.deltaTime;
			if (data.BuildingSlider.value >= data.BuildingSlider.maxValue)
			{
				Destroy(data.Canvas.gameObject);
				data.BuildingSlider = null;
				data.Canvas = null;
			}
		}
	}


	IEnumerator StartBuilding()
	{
		SetupSlider();
		yield return new WaitUntil(() => data.BuildingSlider == null);
		data.IsPlaced = true;
		BuildingManager.AddBuilding(this, data.TeamID);
	}

	void SetupSlider()
	{
		data.Canvas = Creator.CreateCanvasWithSlider(transform);
		var slider = data.Canvas.GetComponentInChildren<Slider>();
		slider.maxValue = data.BuildingTime;
		slider.value = 0;
		data.BuildingSlider = slider;
	}

	public virtual void Construct()
	{
		Team t = MainController.Instance.GetTeam(data.TeamID);
		if (!t) { Debug.Log($"Team {data.TeamID} doesn't exist!"); return; }
		Destroy(gameObject.GetComponent<Movable>());
		ColorBuilding(t.GetTeamColor());

		Map.Layers.BuildingL.FillSquareArea(this, Map.WorldToMap(GetPos()), GetSize());
		if (this is not MainBuilding)
		{ StartCoroutine(StartBuilding()); }
	}


	public void ColorBuilding(Color color)
	{
		if (data.RendererChildren == null) return;
		foreach (Renderer rend in data.RendererChildren)
		{ rend.material.color = color; }
	}

	private void OnDestroy()
	{
		BuildingManager.RemoveBuilding(this, data.TeamID);
	}


	public void MouseDownAct()
	{
		if (EventSystem.current.IsPointerOverGameObject())
			return;

	}

	public virtual void Interact()
	{ BuildingManager.ShowMessage("Building class Interact shouldn't be called"); }

	#region Data transfering

	public bool IsPlaced() => data.IsPlaced;

	public int GetTeamID() => data.TeamID;

	public string GetName() => data.Name;

	public Vector2Int GetSize() => data.Size;

	public void SetTeam(int teamID) { data.TeamID = teamID; }

	public Vector3 GetPos() => transform.position;

	public Renderer[] GetRendererChildren() => data.RendererChildren;

	public UIManager.PanelNames GetPanelName() => data.PanelName;

	public string GetTeamName() => BuildingManager.GetTeamName(data.TeamID);


	#endregion
}
