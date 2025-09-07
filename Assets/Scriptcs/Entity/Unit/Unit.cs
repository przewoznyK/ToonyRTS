using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EntityHealth))]
public class Unit : NetworkBehaviour, IActiveClickable, IGetTeamAndProperties
{
    public UnitTaskManager unitTaskManager;
    public NavMeshAgent agent;
    public Animator animator;
    [SyncVar] public TeamColorEnum teamColor;
    public EntityTypeEnum entityType;
    protected Transform activator;
    [SerializeField] protected SphereCollider enemyDecetorCollider;
    private Transform bodyToDrop;
    public bool aggressiveApproach;

    [Header("Unit Stats")]
    public int damage;
    public float attackRange;
    public float attackCooldown;
    public float maxEnemySearchingDistance;
    public float rotationSpeed;
    public float defaultStoppingDistance;
    public float defaultMovementSpeed;
    public float enemyDetectionRadius;

    [Header("Go To Meeting Point")]
    public bool isGoingToMeetingPoint;
    public Vector3 meetingPoint;

    [Header("Ranged Properties")]
    public bool isRanged;
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float bulletForce;

    public static readonly int Speed = Animator.StringToHash("Speed");
    public static readonly int AttackAnimationTrigger  = Animator.StringToHash("Attack");


    private void Start()
    {
        InitUniversalFunction();
        if (isGoingToMeetingPoint)
            GoToMeetingPoint();

    }
    public void InitUniversalFunction()
    {
        bodyToDrop = transform.GetChild(2);
        activator = transform.GetChild(0);

        if (isServer)
            enemyDecetorCollider.enabled = true;

        enemyDecetorCollider.radius = enemyDetectionRadius;
        
        agent.stoppingDistance = defaultStoppingDistance;
        agent.speed = defaultMovementSpeed;

        EntityHealth entityHealth = GetComponent<EntityHealth>();
        entityHealth.onHurtAction += HurtUnit;
        entityHealth.onDeathActiom += () => RequestToServerToRemoveUnit();

    }

    void GoToMeetingPoint()
    {
        unitTaskManager.RequestToServerToCreateGoToPositionTask(meetingPoint);
        StartCoroutine(ChangeAgentQualityAfterDelay(ObstacleAvoidanceType.HighQualityObstacleAvoidance, 3f));
    }

    IEnumerator ChangeAgentQualityAfterDelay(ObstacleAvoidanceType obstacleAvoidanceType, float time)
    {

        yield return new WaitForSeconds(time);
        agent.obstacleAvoidanceType = obstacleAvoidanceType;
    }
    public ObjectTypeEnum CheckObjectType() => ObjectTypeEnum.unit;

    public void ActiveObject() 
    { 
        activator.gameObject.SetActive(true);
     //   unitTaskManager.taskVisualization.enabled = true;
    }

    public void DeActiveObject() 
    {
        activator.gameObject.SetActive(false);
      //  unitTaskManager.taskVisualization.enabled = false;
    } 

    public List<UnitNameEnum> GetUnitsCanBuyList() => throw new System.NotImplementedException();

    public virtual void PlayerRightMouseButtonCommand(RaycastHit hit, bool isShiftPressed) => Debug.Log("Override this method");
    public virtual void PlayerLeftMouseButtonCommand(RaycastHit hit, bool isShiftPressed) => Debug.Log("Override this method");


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
     //   animator.SetTrigger("Hurt");
        if(unitTaskManager.taskTransform == null)
            StartCoroutine(IncreaseEnemeyDetectionRadiusForAMomentAfterTakingDamage());
    }
    public void RequestToServerToRemoveUnit()
    {
        if(teamColor == PlayerController.LocalPlayer.teamColor)
            PlayerController.LocalPlayer.CmdRemoveUnit(this.netIdentity, this);
    }

    public void RespondFromServerToRemoveUnit()
    {
        PlayerController.LocalPlayer.controlledUnits.RemoveUnit(this);
        if (isServer)
            PlayerController.LocalPlayer.CmdRemoveGameObject(this.gameObject);
    }
    public void SetActiveEnemyDetector(bool value) =>  enemyDecetorCollider.enabled = value;

    IEnumerator IncreaseEnemeyDetectionRadiusForAMomentAfterTakingDamage()
    {
        enemyDecetorCollider.radius = enemyDetectionRadius * 10;

        yield return new WaitForSeconds(1f);
        enemyDecetorCollider.radius = enemyDetectionRadius;
    }
    internal void AttackDetectionTarget(IGetTeamAndProperties component)
    {
        if (isServer)
            Debug.Log(teamColor + " [SERVER] AttackDetectionTarget " + component.GetTeam());
        else
            Debug.Log(teamColor + " [CLIENT] AttackDetectionTarget " + component.GetTeam());

        unitTaskManager.RequestToServerToCreateAttackEntityTask(component.GetTeam(), component.GetProperties<Transform>());
    }
}