using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        Debug.Log(unitData);
        unitProductionQueue.Enqueue(unitData);
    }


}
