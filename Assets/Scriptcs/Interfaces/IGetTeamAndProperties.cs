using UnityEngine;

public interface IGetTeamAndProperties
{
    public TeamColorEnum GetTeam();
    public EntityTypeEnum GetEntityType();
    public T GetProperties<T>() where T : Component;
}
