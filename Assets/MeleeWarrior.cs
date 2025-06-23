using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeWarrior : Unit
{
    bool isOnTask;
    public LinkedList<UnitTask> requestedTasks = new();
    UnitTask currentTask;
    Transform taskTransform;
    Vector3 taskVector;
    private static readonly int AttackAnimationTrigger = Animator.StringToHash("Attack");
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;

    public override void PlayerRightMouseButtonCommand(RaycastHit hit, bool isShiftPressed)
    {
        if (isShiftPressed == false)
            ResetTasks();  
        //else
        //    Instantiate(taskFlagPrefab, hit.point, Quaternion.identity);
        if (hit.collider.CompareTag("Ground"))
        {
            GoToPositionTask newTask = new(hit.point);
            requestedTasks.AddLast(newTask);
        }
        else if(hit.collider.TryGetComponent<IGetTeamAndProperties>(out IGetTeamAndProperties component))
        {
            if ((component.GetTeam() & teamColor) != teamColor)
            {
                Transform targetTransform = component.GetProperties<Transform>();
                AttackTargetTask newTask = new(targetTransform);
                requestedTasks.AddLast(newTask);
                    

            }
        }
     

        DoTask();
    }
    private void Update()
    {
        if(isOnTask)
        {
            if (currentTask.unitTaskType == UnitTaskTypeEnum.GoToPosition)
            {
                if (Vector3.Distance(taskVector, transform.position) <= 2f)
                {
                    GoToNextTask();
                }
            }

            if (currentTask.unitTaskType == UnitTaskTypeEnum.AttackTarget)
            {
                agent.SetDestination(taskTransform.position);

                if (Vector3.Distance(taskTransform.position, transform.position) <= attackRange)
                {
                    StartCoroutine(AttackCycle());
                    isOnTask = false;
                }
            }
        }
    }
    public void DoTask()
    {
        if (requestedTasks.Count > 0 && isOnTask == false)
        {
            currentTask = requestedTasks.First.Value;
            requestedTasks.RemoveFirst();
            if (currentTask is GoToPositionTask goToPositionTask)
            {
                Vector3 pos = goToPositionTask.destinatedPosition;

                agent.SetDestination(pos);
                taskVector = pos;
            }
            else if (currentTask is AttackTargetTask attackTarget)
            {   
                taskTransform = attackTarget.targetTransform;
            }
            animator.SetFloat(Speed, 1f);
            isOnTask = true;
        }

    }

    void GoToNextTask()
    {
        isOnTask = false;
        animator.SetFloat(Speed, 0f);
        DoTask();
     
    }
    
    void ResetTasks()
    {
        requestedTasks.Clear();
        isOnTask = false;
        currentTask = null;
        StopAllCoroutines();
    }
    IEnumerator AttackCycle()
    {
        animator.SetFloat(Speed, 0f);
        animator.SetTrigger(AttackAnimationTrigger);
        yield return new WaitForSeconds(1.15f);
        if (Vector3.Distance(taskTransform.position, transform.position) > attackRange)
        {
            AttackTargetTask newTask = new(taskTransform);
            requestedTasks.AddFirst(newTask);
            DoTask();
            yield break;
        }
        yield return new WaitForSeconds(attackCooldown);
        StartCoroutine(AttackCycle());
    }
}
