using UnityEngine;


public interface IReadOnlyTeam
{
	public string GetTeamName();
	public int GetTeamID();
	public Color GetTeamColor();
	public Vector3 GetTeamCenter();
	public float GetTeamBuildingRadius();
	public Inventory GetTeamInventory();

}

public interface ITeamMember
{
	public void SetTeam(int teamID);
}
