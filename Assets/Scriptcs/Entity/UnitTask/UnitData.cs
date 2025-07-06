using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Units/Unit Data", order = 1)]
public class UnitData : ScriptableObject
{
    public int unitID;
    public UnitNameEnum unitName;
    public int health;
    public int attack;
    public float speed;
    public Sprite unitSprite;
    public GameObject unitPrefab;
    public float productionTime;
    [field: SerializeField] public List<ObjectPrices> objectPrices = new List<ObjectPrices>();
}
