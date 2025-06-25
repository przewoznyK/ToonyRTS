using System.Collections;
using UnityEngine;

public class UnitAttack : MonoBehaviour
{
    Unit unit;
    Animator animator;
    private void Start()
    {
        unit = GetComponent<Unit>();
        animator = GetComponent<Animator>();
    }
    public void Attack()
    {

    }


}
