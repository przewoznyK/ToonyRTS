using UnityEngine;

public interface IGetTeamAndProperties
{
    public TeamColorEnum GetTeam();
    public EntityTypeEnum GetEntityType();
    public BuildingTypeEnum GetBuildingType();
    public T GetProperties<T>() where T : Component;
}
