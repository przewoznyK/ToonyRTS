using UnityEngine;

public class TaskVisualization : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject taskVizualizationPrefab;
    public GameObject VisualizeTask(Vector3 taskPosition)
    {
        return Instantiate(taskVizualizationPrefab, taskPosition, Quaternion.identity);
    }
}
