using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EntityHealth))]
public class Unit : MonoBehaviour, IActiveClickable, IGetTeamAndProperties
{
    protected UnitTaskManager unitTaskManager;
    protected UnitAttack unitAttack;
    public TeamColorEnum teamColor;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Animator animator;
    public Vector3 TargetPosition;
    public bool isGoingToPosition;
    public float TimeStuck;
    protected Transform activator;
    [SerializeField] protected GameObject taskFlagPrefab;

    public float attackRange;
    public float attackCooldown;

    public static readonly int Speed = Animator.StringToHash("Speed");
    public static readonly int AttackAnimationTrigger  = Animator.StringToHash("Attack");

    
    private void Start()
    {
        unitTaskManager = GetComponent<UnitTaskManager>();
        unitAttack = GetComponent<UnitAttack>();

        activator = transform.GetChild(0);
        if(teamColor == TeamColorEnum.Blue)
            AccessToClassByTeamColor.instance.GetControlledUnitsByTeamColor(teamColor).AddToAllUnits(this);

        EntityHealth entityHealth = GetComponent<EntityHealth>();
        entityHealth.onDeathActiom += () => DeleteUnit();
    }
    public ObjectTypeEnum CheckObjectType() => ObjectTypeEnum.unit;

    public void ActiveObject() => activator.gameObject.SetActive(true);

    public void DeActiveObject() => activator.gameObject.SetActive(false);

    public List<UnitNameEnum> GetUnitsCanBuyList() => throw new System.NotImplementedException();

    public void GoMeetingPosition(Vector3 position) => agent.SetDestination(position);

    public virtual void PlayerRightMouseButtonCommand(RaycastHit hit, bool isShiftPressed) => Debug.Log("Override this method");

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

    public void DeleteUnit()
    {
        //  AccessToClassByTeamColor.instance.GetControlledUnitsByTeamColor(teamColor).RemoveUnit(this);
        unitTaskManager.enabled = false;
        //Destroy(gameObject);
    }

}