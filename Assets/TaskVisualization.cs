using System.Collections.Generic;
using UnityEngine;

public class TaskVisualization : MonoBehaviour
{

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject taskVizualizationPrefab;
    [SerializeField] private Transform taskVizualizationContainer;
    List<MovingTargetTaskVisualizationData> updatingPositionTasksData = new();
    int requestedTaskCount;
    private Vector3 worldPosition;
    private Quaternion worldRotation = Quaternion.identity;
    private void OnEnable()
    {
        taskVizualizationContainer.position = worldPosition;
        taskVizualizationContainer.rotation = worldRotation;

        lineRenderer.enabled = true;
        taskVizualizationContainer.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        lineRenderer.enabled = false;
        taskVizualizationContainer.gameObject.SetActive(false);
    }
    private void Update()
    {
        if(requestedTaskCount > 0)
            lineRenderer.SetPosition(0, transform.position);

        foreach (var taskData in updatingPositionTasksData)
        {
            if(taskData != null)
            {
                if (taskData.taskTargetTransform == null) return;
                taskData.taskVizualizationGameObject.transform.position = taskData.taskTargetTransform.position;

                lineRenderer.SetPosition(taskData.taskLineRendererIndex, taskData.taskTargetTransform.position);
            }
        }
    }
    private void LateUpdate()
    {
        taskVizualizationContainer.position = worldPosition;
        taskVizualizationContainer.rotation = worldRotation;
    }

    public GameObject AddNewTaskAndRefreshLineRenderer(LinkedList<UnitTask> requestedTasks)
    {
        ClearVisulalizationFlags();
        updatingPositionTasksData.Clear();
        GameObject vizualizationGameObject = null;
        lineRenderer.positionCount = 1;
        requestedTaskCount = requestedTasks.Count;
        if (requestedTasks.Count >= 1)
        {
            int renderLineIndex = 0;
            lineRenderer.positionCount = requestedTasks.Count + 1;
            foreach (var task in requestedTasks)
            {
                vizualizationGameObject = Instantiate(taskVizualizationPrefab, task.taskPosition, Quaternion.identity, taskVizualizationContainer);
                if (task.unitTaskType == UnitTaskTypeEnum.GoToPosition)
                {
                    renderLineIndex++;
                }
                else if(task.unitTaskType == UnitTaskTypeEnum.AttackTarget)
                {
                    renderLineIndex++;
                    MovingTargetTaskVisualizationData movingTaskTarget = new(vizualizationGameObject, task.targetTransform, renderLineIndex);
                    updatingPositionTasksData.Add(movingTaskTarget);
                }
                else if((task.unitTaskType == UnitTaskTypeEnum.GatherResource )|| task.unitTaskType == UnitTaskTypeEnum.BuildingConstruction)
                {
                    renderLineIndex++;

                }
                lineRenderer.SetPosition(renderLineIndex, task.taskPosition);
              
            }

        }
        return vizualizationGameObject;
    }

    public void ClearVisulalizationFlags()
    {
        foreach (Transform child in taskVizualizationContainer)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
