using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EntityHealth))]
public class Unit : MonoBehaviour, IActiveClickable, IGetTeamAndProperties
{
    protected UnitTaskManager unitTaskManager;
    public TeamColorEnum teamColor;
    public EntityTypeEnum entityType;
    public NavMeshAgent agent;
    public Animator animator;
    public Vector3 TargetPosition;
    public bool isGoingToPosition;
    public float TimeStuck;
    protected Transform activator;
    protected SphereCollider enemyDecetorCollider;
    [SerializeField] protected GameObject taskFlagPrefab;
    public GameObject attackArea;

    public int damage;
    public float attackRange;
    public float attackCooldown;
    public float maxEnemySearchingDistance;
    public float rotationSpeed;
    public float defaultStoppingDistance;
    public float defaultMovementSpeed;
    public float enemyDetectionRadius;
    [SerializeField] protected Transform bodyToDrop;
    public static readonly int Speed = Animator.StringToHash("Speed");
    public static readonly int AttackAnimationTrigger  = Animator.StringToHash("Attack");

    
    private void Start()
    {
        agent.stoppingDistance = defaultStoppingDistance;
        agent.speed = defaultMovementSpeed;
        InitUniversalFunction();
    }
    public void InitUniversalFunction()
    {
        unitTaskManager = GetComponent<UnitTaskManager>();
        agent = GetComponent<NavMeshAgent>();

        activator = transform.GetChild(0);
        enemyDecetorCollider = transform.GetChild(1).GetComponent<SphereCollider>();
        enemyDecetorCollider.radius = enemyDetectionRadius;
        AccessToClassByTeamColor.instance.GetControlledUnitsByTeamColor(teamColor).AddToAllUnits(this);

        EntityHealth entityHealth = GetComponent<EntityHealth>();
        entityHealth.onHurtAction += HurtUnit;
        entityHealth.onDeathActiom += () => DeleteUnit();

    }
    public ObjectTypeEnum CheckObjectType() => ObjectTypeEnum.unit;

    public void ActiveObject() => activator.gameObject.SetActive(true);

    public void DeActiveObject() => activator.gameObject.SetActive(false);

    public List<UnitNameEnum> GetUnitsCanBuyList() => throw new System.NotImplementedException();

    public void GoMeetingPosition(Vector3 position)
    {
        agent.SetDestination(position);
    }
    public virtual void PlayerRightMouseButtonCommand(RaycastHit hit, bool isShiftPressed) => Debug.Log("Override this method");

    public TeamColorEnum GetTeam()
    {
        return teamColor;
    }

    public EntityTypeEnum GetEntityType()
    {
        return entityType;
    }
    public BuildingTypeEnum GetBuildingType()
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

    public void HurtUnit(Unit fromUnit)
    {
        animator.SetTrigger("Hurt");
        unitTaskManager.DefenseFromAttack(fromUnit);
    }
    public void DeleteUnit()
    {
        AccessToClassByTeamColor.instance.GetControlledUnitsByTeamColor(teamColor).RemoveUnit(this);
        unitTaskManager.enabled = false;
        animator.SetTrigger("Death");
        bodyToDrop.SetParent(null, true);
        Destroy(gameObject);
    }

    public void SetActiveEnemyDetector(bool value)
    {
        enemyDecetorCollider.enabled = value;
    }


}