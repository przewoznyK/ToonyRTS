using Mirror;
using UnityEngine;

public struct TaskDataForVisualization
{
    public bool followTarget;
    public Vector3 position;
    public NetworkIdentity targetIdentity;

    public TaskDataForVisualization(Vector3 position)
    {
        followTarget = false;
        this.position = position;
        targetIdentity = null;
    }

    public TaskDataForVisualization(NetworkIdentity targetIdentity)
    {
        followTarget = true;
        this.position = Vector3.zero;
        this.targetIdentity = targetIdentity;
    }
}
