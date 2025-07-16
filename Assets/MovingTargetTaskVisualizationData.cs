using UnityEngine;

public class MovingTargetTaskVisualizationData
{
    public GameObject taskVizualizationGameObject;
    public Transform taskTargetTransform;
    public int taskLineRendererIndex;

    public MovingTargetTaskVisualizationData(GameObject taskVizualizationGameObject, Transform taskTargetTransform, int taskLineRendererIndex)
    {
        this.taskVizualizationGameObject = taskVizualizationGameObject;
        this.taskTargetTransform = taskTargetTransform;
        this.taskLineRendererIndex = taskLineRendererIndex;
    }
}