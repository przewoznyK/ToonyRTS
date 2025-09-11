using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskVisualization : NetworkBehaviour
{
    public static TaskVisualization Instance;

    public ControlledUnits controlledUnits;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject taskVizualizationPrefab;
    [SerializeField] private Transform taskVizualizationContainer;
    List<MovingTargetTaskVisualizationData> updatingPositionTasksData = new();

    UnitTaskManager unitTaskManager;
    private bool callbackRegistered = false;

    Transform targetTransform;
    
    Dictionary<GameObject, Transform> followFlags = new();
    private void Update()
    {
        if(followFlags.Count > 0)
        {
            foreach (var item in followFlags)
            {
                if (item.Value == null) return;
                item.Key.transform.position = item.Value.transform.position;
            }

        }

        if (lineRenderer.positionCount > 1 && unitTaskManager)
        {
            lineRenderer.SetPosition(0, unitTaskManager.transform.position);
        }
    }
    public void Init(ControlledUnits controlledUnits)
    {
        Instance = this;
        this.controlledUnits = controlledUnits;
        controlledUnits.OnSelectedUnitsChanged += ShowCurrentTask;
 

    }
    internal void ShowCurrentTask()
    {
        if (controlledUnits.selectedUnits.Count <= 0)
        {
            ClearVisualization();
            return;
        }
     
        this.unitTaskManager = controlledUnits.selectedUnits[0].unitTaskManager;
        if(callbackRegistered == false)
            unitTaskManager.taskDataForVisualizationList.Callback += OnTaskListChanged;

        if(unitTaskManager.taskDataForVisualizationList.Count > 1)
            lineRenderer.enabled = true;
        else
            lineRenderer.enabled = false;

        lineRenderer.positionCount = unitTaskManager.taskDataForVisualizationList.Count + 1;
        int lrIndex = 0;
        lineRenderer.SetPosition(lrIndex, unitTaskManager.transform.position);
        callbackRegistered = true;
        foreach (var taskData in unitTaskManager.taskDataForVisualizationList)
        {
            Vector3 destinationPosition = Vector3.zero;
            if (taskData.followTarget)
            {
                if (taskData.targetIdentity == null) return;
                targetTransform = taskData.targetIdentity.GetComponent<Transform>();

                destinationPosition = targetTransform.transform.position;
                GameObject newFollowFlag = Instantiate(taskVizualizationPrefab, destinationPosition, Quaternion.identity, taskVizualizationContainer);
                followFlags.Add(newFollowFlag, targetTransform);
            }
            else
            {
                destinationPosition = taskData.position;
                Instantiate(taskVizualizationPrefab, destinationPosition, Quaternion.identity, taskVizualizationContainer);
            }
            lrIndex++;
            lineRenderer.SetPosition(lrIndex, destinationPosition);
        }
    }

    public void OnTaskListChanged(SyncList<TaskDataForVisualization>.Operation op, int index, TaskDataForVisualization oldItem, TaskDataForVisualization newItem)
    {
        followFlags.Clear();

        foreach (Transform child in taskVizualizationContainer)
            Destroy(child.gameObject);

        lineRenderer.positionCount = 0;
        if (unitTaskManager.taskDataForVisualizationList.Count <= 1) return;
        if (unitTaskManager.taskDataForVisualizationList.Count > 1)
            lineRenderer.enabled = true;
        else
            lineRenderer.enabled = false;
        lineRenderer.positionCount = unitTaskManager.taskDataForVisualizationList.Count + 1;
        int lrIndex = 0;
        lineRenderer.SetPosition(lrIndex, unitTaskManager.transform.position);
        foreach (var taskData in unitTaskManager.taskDataForVisualizationList)
        {
            
            Vector3 destinationPosition = Vector3.zero;
            if (taskData.followTarget)
            {
                if (taskData.targetIdentity == null) break;
                targetTransform = taskData.targetIdentity.GetComponent<Transform>();
                destinationPosition = targetTransform.transform.position;
                GameObject newFollowFlag = Instantiate(taskVizualizationPrefab, destinationPosition, Quaternion.identity, taskVizualizationContainer);
                followFlags.Add(newFollowFlag, targetTransform);

            }
            else
            {
                destinationPosition = taskData.position;
                Instantiate(taskVizualizationPrefab, destinationPosition, Quaternion.identity, taskVizualizationContainer);
            }
            lrIndex++;
            lineRenderer.SetPosition(lrIndex, destinationPosition);

        }
    }

    internal void ClearVisualization()
    {
        if (unitTaskManager == null) return;

        callbackRegistered = false;
        unitTaskManager.taskDataForVisualizationList.Callback -= OnTaskListChanged;
        this.unitTaskManager = null;

        followFlags.Clear();
        foreach (Transform child in taskVizualizationContainer)
            Destroy(child.gameObject);

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
            lineRenderer.enabled = false;
        }
    }
}
