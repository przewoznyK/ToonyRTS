using Mirror;
using System;
using UnityEngine;

public class DefaultMapGenerator : MonoBehaviour
{
    public static DefaultMapGenerator Instance;
    [SerializeField]
    public GameObject[] objectsToSpawn;
    private void Awake()
    {
        Instance = this;
    }

}
