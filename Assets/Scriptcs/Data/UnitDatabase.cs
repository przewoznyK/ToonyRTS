using System.Collections.Generic;
using UnityEngine;

public class UnitDatabase : MonoBehaviour
{
    public static UnitDatabase Instance;
    [SerializeField]
    private List<UnitData> unitDataList = new List<UnitData>();
    private void Awake()
    {
        Instance = this;
    }
    public UnitData GetUnitDataByID(int ID)
    {
        return unitDataList.Find(unit => unit.unitID == ID);
    }

    public UnitData GetUnitDataByNameEnum(UnitNameEnum unitName)
    {
        return unitDataList.Find(unit => unit.unitName == unitName);
    }
    
}
