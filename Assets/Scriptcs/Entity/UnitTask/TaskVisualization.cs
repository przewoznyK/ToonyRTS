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
        }
    }

    public void OnTaskListChanged(SyncList<TaskDataForVisualization>.Operation op, int index, TaskDataForVisualization oldItem, TaskDataForVisualization newItem)
    {
        followFlags.Clear();

        foreach (Transform child in taskVizualizationContainer)
            Destroy(child.gameObject);

        if (unitTaskManager.taskDataForVisualizationList.Count <= 1) return;

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
    }

    //int requestedTaskCount;
    //private Vector3 worldPosition;
    //private Quaternion worldRotation = Quaternion.identity;
    //private void OnEnable()
    //{
    //    taskVizualizationContainer.position = worldPosition;
    //    taskVizualizationContainer.rotation = worldRotation;

    //    lineRenderer.enabled = true;
    //    taskVizualizationContainer.gameObject.SetActive(true);
    //}

    //private void OnDisable()
    //{
    //    lineRenderer.enabled = false;
    //    taskVizualizationContainer.gameObject.SetActive(false);
    //}
    //private void Update()
    //{
    //    if(requestedTaskCount > 0)
    //        lineRenderer.SetPosition(0, transform.position);

    //    foreach (var taskData in updatingPositionTasksData)
    //    {
    //        if(taskData != null)
    //        {
    //            if (taskData.taskTargetTransform == null) return;
    //            taskData.taskVizualizationGameObject.transform.position = taskData.taskTargetTransform.position;

    //            lineRenderer.SetPosition(taskData.taskLineRendererIndex, taskData.taskTargetTransform.position);
    //        }
    //    }
    //}
    //private void LateUpdate()
    //{
    //    taskVizualizationContainer.position = worldPosition;
    //    taskVizualizationContainer.rotation = worldRotation;
    //}

    //public GameObject AddNewTaskAndRefreshLineRenderer(LinkedList<UnitTask> requestedTasks)
    //{

    //    ClearVisulalizationFlags();
    //    updatingPositionTasksData.Clear();
    //    GameObject vizualizationGameObject = null;
    //    lineRenderer.positionCount = 1;
    //    requestedTaskCount = requestedTasks.Count;
    //    if (requestedTasks.Count >= 1)
    //    {
    //        int renderLineIndex = 0;
    //        lineRenderer.positionCount = requestedTasks.Count + 1;
    //        foreach (var task in requestedTasks)
    //        {
    //            vizualizationGameObject = Instantiate(taskVizualizationPrefab, task.taskPosition, Quaternion.identity, taskVizualizationContainer);
    //            PlayerController.LocalPlayer.CmdSpawnTaskVizualization(vizualizationGameObject);
    //            if (task.unitTaskType == UnitTaskTypeEnum.GoToPosition)
    //            {
    //                renderLineIndex++;
    //            }
    //            else if(task.unitTaskType == UnitTaskTypeEnum.AttackTarget)
    //            {
    //                renderLineIndex++;
    //                MovingTargetTaskVisualizationData movingTaskTarget = new(vizualizationGameObject, task.targetTransform, renderLineIndex);
    //                updatingPositionTasksData.Add(movingTaskTarget);
    //            }
    //            else if((task.unitTaskType == UnitTaskTypeEnum.GatherResource )|| task.unitTaskType == UnitTaskTypeEnum.BuildingConstruction)
    //            {
    //                renderLineIndex++;

    //            }
    //            lineRenderer.SetPosition(renderLineIndex, task.taskPosition);

    //        }

    //    }
    //    return vizualizationGameObject;
    //}

    //public void ClearVisulalizationFlags()
    //{
    //    foreach (Transform child in taskVizualizationContainer)
    //    {
    //        GameObject.Destroy(child.gameObject);
    //    }
    //}
}
