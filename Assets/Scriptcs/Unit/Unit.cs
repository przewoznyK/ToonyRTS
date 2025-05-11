using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IActiveClickable
{
    Transform activator;
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

    public List<UnitNameEnum> GetUnitsToBuyList()
    {
        throw new System.NotImplementedException();
    }
}
