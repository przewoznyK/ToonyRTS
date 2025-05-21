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
        Debug.Log("ODPALAM");
        //throw new System.NotImplementedException();
        meetingPoint.gameObject.SetActive(true);
    }
    public void DisableObject()
    {
        Debug.Log("WYLACZAM");
        meetingPoint.gameObject.SetActive(false);
    }
    internal void SetMeetingPoint(Vector3 newMeetingPointPosition) => meetingPoint.transform.position = newMeetingPointPosition;

    public void SpawnUnit(int unitID)
    {
        GameObject unitPrefab = UnitDatabase.Instance.GetUnitDataByID(unitID).unitPrefab;
        GameObject unitInstantiate = Instantiate(unitPrefab, transform.position, Quaternion.identity);
        unitInstantiate.GetComponent<Unit>().GoMeetingPosition(meetingPoint.transform.position);
    }
}
