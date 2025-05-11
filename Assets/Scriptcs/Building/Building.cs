using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, IActiveClickable
{
    [SerializeField] private List<UnitNameEnum> unitsToBuy;
    public ObjectTypeEnum CheckObjectType()
    {
        return ObjectTypeEnum.building;
    }

    public List<UnitNameEnum> GetUnitsToBuyList() => unitsToBuy;

    void IActiveClickable.ActiveObject()
    {
        //throw new System.NotImplementedException();
    }
}
