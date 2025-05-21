using System.Collections.Generic;
using UnityEngine;

public class BuildingProductionData
{
    public Queue<Product> productQueue = new();
    public Coroutine activeCoroutine = null;
}