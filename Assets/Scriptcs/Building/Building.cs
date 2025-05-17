using System;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, IActiveClickable
{
    [SerializeField] private List<UnitNameEnum> unitsToBuy;
    [SerializeField] private Transform meetingPoint;

    public Queue<UnitData> unitProductionQueue;
    private void Awake()
    {
        unitProductionQueue = new Queue<UnitData>();
    }
    public ObjectTypeEnum CheckObjectType()
    {
        return ObjectTypeEnum.building;
    }

    public List<UnitNameEnum> GetUnitsCanBuyList() => unitsToBuy;

    void IActiveClickable.ActiveObject()
    {
        //throw new System.NotImplementedException();
    }

    public void AddToProductionQueue(UnitData unitData)
    {
        unitProductionQueue.Enqueue(unitData);
    }

    internal void SetMeetingPoint(Vector3 newMeetingPointPosition) => meetingPoint.transform.position = newMeetingPointPosition;

    public void SpawnUnit(int unitID)
    {
        GameObject unitPrefab = UnitDatabase.Instance.GetUnitDataByID(unitID).unitPrefab;
        GameObject unitInstantiate = Instantiate(unitPrefab, transform.position, Quaternion.identity);
        unitInstantiate.GetComponent<Unit>().GoMeetingPosition(meetingPoint.transform.position);
    }
}
