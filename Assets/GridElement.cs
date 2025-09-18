using System.Collections.Generic;
using UnityEngine;

public class GridElement : MonoBehaviour
{
    [SerializeField] private List<Vector3Int> positionToOccupy;

    public void SetPositionToOccupy(List<Vector3Int> positions)
    {
        positionToOccupy = positions;
    }

    public void RemoveGridData()
    {
        GridDataNetwork.Instance.RespondFromServerToRemoveObjectFromGridData(positionToOccupy);
    }
}
