using System;
using UnityEngine;

[Serializable]
public class ObjectPrices
{
    [field: SerializeField] public ResourceTypesEnum priceType { get; private set; }
    [field: SerializeField] public int priceValue { get; private set; }
}