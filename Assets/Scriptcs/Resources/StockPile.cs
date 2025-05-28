using System;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class StockPile : MonoBehaviour
{

    private int _gathered;

    private void OnEnable()
    {
        _gathered = 0;
        Add();
    }

    public void Add()
    {
        _gathered++;

        Debug.Log("TUTAJ BEDZIE PRZEKAZYWANIE ITEMOW BEZPOSRDENIO DO RESOURCE MANAGER");
    }

}