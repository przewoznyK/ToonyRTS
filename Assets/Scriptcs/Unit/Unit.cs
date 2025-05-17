using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour, IActiveClickable
{
    [SerializeField] private NavMeshAgent agent;
    Transform activator;
    Vector3 meetingPosition;
    private void Start()
    {
        activator = transform.GetChild(0);
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
}
