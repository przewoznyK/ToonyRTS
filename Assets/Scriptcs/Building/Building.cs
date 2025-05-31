using System;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, IActiveClickable
{
    [SerializeField] private List<UnitNameEnum> unitsToBuy;
    [SerializeField] private Transform meetingPoint;
    public ObjectTypeEnum CheckObjectType()
    {
        return ObjectTypeEnum.building;
    }

    public List<UnitNameEnum> GetUnitsCanBuyList() => unitsToBuy;

    void IActiveClickable.ActiveObject()
    {
        meetingPoint.gameObject.SetActive(true);
    }
    public void DisableObject()
    {
        meetingPoint.gameObject.SetActive(false);
    }
    internal void SetMeetingPoint(Vector3 newMeetingPointPosition) => meetingPoint.transform.position = newMeetingPointPosition;

    public void SpawnUnit(int unitID, TeamColorEnum teamColorEnum)
    {
        GameObject unitPrefab = UnitDatabase.Instance.GetUnitDataByID(unitID).unitPrefab;
        GameObject unitInstantiate = Instantiate(unitPrefab, transform.position, Quaternion.identity);
        
        var unit = unitInstantiate.GetComponent<Unit>();
        unit.GoMeetingPosition(meetingPoint.transform.position);
        unit.teamColorEnum = teamColorEnum;
    }
}
