using System;           // dla typeof(), itp.
using UnityEngine;

public interface IGetTeamAndProperties
{
    //[SerializeField] private TeamColorEnum teamColor;
    //[SerializeField] private EntityTypeEnum entityType;
    public TeamColorEnum GetTeam();
    public EntityTypeEnum GetEntityType();
    public T GetProperties<T>() where T : Component;
}
