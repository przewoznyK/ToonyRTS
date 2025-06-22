using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour, IActiveClickable, IGetTeamAndProperties
{
    public TeamColorEnum teamColor;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Animator animator;
    public Vector3 TargetPosition;
    public bool isGoingToPosition;
    public float TimeStuck;
    Transform activator;

    protected static readonly int Speed = Animator.StringToHash("Speed");
    private void Start()
    {
        activator = transform.GetChild(0);
        if(teamColor == TeamColorEnum.Blue)
            AccessToClassByTeamColor.instance.GetControlledUnitsByTeamColor(teamColor).AddToAllUnits(this);
    }
    public ObjectTypeEnum CheckObjectType() => ObjectTypeEnum.unit;

    public void ActiveObject()
    {
        activator.gameObject.SetActive(true);
    }
    public void DeActiveObject()
    {
        activator.gameObject.SetActive(false);
    }

    public List<UnitNameEnum> GetUnitsCanBuyList()
    {
        throw new System.NotImplementedException();
    }

    public void GoMeetingPosition(Vector3 position) => agent.SetDestination(position);


    public virtual void PlayerRightMouseButtonCommand(RaycastHit hit)
    {
        Debug.Log("Override this method");
    }

    public TeamColorEnum GetTeam()
    {
        return teamColor;
    }

    public EntityTypeEnum GetEntityType()
    {
        throw new NotImplementedException();
    }

    public T GetProperties<T>() where T : Component
    {
        if (typeof(T) == typeof(Transform))
            return transform as T;
        else
            Debug.Log("You can only take Transform from this");
        return null;
    }
}