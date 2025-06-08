using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour, IActiveClickable
{
    public TeamColorEnum teamColor;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Animator animator;
    public Vector3 TargetPosition;
    public bool isGoingToPosition;
    public float TimeStuck;
    Transform activator;
    private void Start()
    {
        activator = transform.GetChild(0);
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
        Debug.Log("111");
        // Domyœlna logika (lub pusta jeœli tylko do nadpisania)
    }
}
