using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingProductionData : MonoBehaviour
{
    public Queue<Product> productQueue = new();
    public Coroutine activeCoroutine = null;
    public float timeProduction;
    public float currentTimeProduction;
    public bool endProduction;

   public IEnumerator StartProduction()
    {
        currentTimeProduction = timeProduction;

        while (currentTimeProduction > 0f)
        {
            currentTimeProduction -= Time.deltaTime;
            yield return null;
        }
        endProduction = true;
    }
}