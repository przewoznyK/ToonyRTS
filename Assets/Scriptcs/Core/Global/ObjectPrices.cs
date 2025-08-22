using System;
using UnityEngine;

[Serializable]
public class ObjectPrices
{
    [field: SerializeField] public ResourceTypesEnum priceType { get; private set; }
    [field: SerializeField] public int priceValue { get; private set; }

    public ObjectPrices() { }

    public ObjectPrices(ResourceTypesEnum priceType, int priceValue)
    {
        this.priceType = priceType;
        this.priceValue = priceValue;
    }

    public void SetValue(int newValue)
    {
        priceValue = newValue;
    }

    public void AddValue(int delta)
    {
        priceValue += delta;
    }


    public ObjectPrices Clone()
    {
        return new ObjectPrices(priceType, priceValue);
    }

}