using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EntityHealth))]
public class Unit : MonoBehaviour, IActiveClickable, IGetTeamAndProperties
{
    public UnitTaskManager unitTaskManager;
    public NavMeshAgent agent;
    public Animator animator;
    public TeamColorEnum teamColor;
    public EntityTypeEnum entityType;
    protected Transform activator;
    [SerializeField] protected SphereCollider enemyDecetorCollider;
    private Transform bodyToDrop;
    [Header("Unit Stats")]
    public int damage;
    public float attackRange;
    public float attackCooldown;
    public float maxEnemySearchingDistance;
    public float rotationSpeed;
    public float defaultStoppingDistance;
    public float defaultMovementSpeed;
    public float enemyDetectionRadius;

    [Header("Ranged Properties")]
    public bool isRanged;
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float bulletForce;

    public static readonly int Speed = Animator.StringToHash("Speed");
    public static readonly int AttackAnimationTrigger  = Animator.StringToHash("Attack");



    [Header("Go To Meeting Point")]
    public bool isGoingToMeetingPoint;
    public Vector3 meetingPoint;
    private void Start()
    {
        InitUniversalFunction();
        if (isGoingToMeetingPoint)
            unitTaskManager.GoToPosition(meetingPoint);

    }
    public void InitUniversalFunction()
    {
        bodyToDrop = transform.GetChild(2);
        activator = transform.GetChild(0);
        enemyDecetorCollider.radius = enemyDetectionRadius;
   
        AccessToClassByTeamColor.Instance.GetControlledUnitsByTeamColor(teamColor).AddToAllUnits(this);
        agent.stoppingDistance = defaultStoppingDistance;
        agent.speed = defaultMovementSpeed;

        EntityHealth entityHealth = GetComponent<EntityHealth>();
        entityHealth.onHurtAction += HurtUnit;
        entityHealth.onDeathActiom += () => DeleteUnit();

    }
    public ObjectTypeEnum CheckObjectType() => ObjectTypeEnum.unit;

    public void ActiveObject() 
    { 
        activator.gameObject.SetActive(true);
        unitTaskManager.taskVisualization.enabled = true;
    }

    public void DeActiveObject() 
    {
        activator.gameObject.SetActive(false);
        unitTaskManager.taskVisualization.enabled = false;
    } 

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
        if(unitTaskManager.taskTransform == null)
            StartCoroutine(IncreaseEnemeyDetectionRadiusForAMomentAfterTakingDamage());
    }
    public void DeleteUnit()
    {
        AccessToClassByTeamColor.Instance.GetControlledUnitsByTeamColor(teamColor).RemoveUnit(this);
        unitTaskManager.enabled = false;
        animator.SetTrigger("Death");

        bodyToDrop.SetParent(null, true);
        Destroy(gameObject);
    }

    public void SetActiveEnemyDetector(bool value)
    {
        enemyDecetorCollider.enabled = value;
    }

    IEnumerator IncreaseEnemeyDetectionRadiusForAMomentAfterTakingDamage()
    {
        enemyDecetorCollider.radius = enemyDetectionRadius * 10;

        yield return new WaitForSeconds(1f);
        enemyDecetorCollider.radius = enemyDetectionRadius;

    }
    internal void AttackDetectionTarget(IGetTeamAndProperties component)
    {
        unitTaskManager.AttackTarget(component.GetProperties<Transform>(), component.GetTeam());
        SetActiveEnemyDetector(false);
    }
}