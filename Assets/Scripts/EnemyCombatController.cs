using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatController : MonoBehaviour
{
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (CombatSystem.instance.enemyAttackButtonWasPressed)
            animator.SetTrigger("attack");
        if (CombatSystem.instance.enemyIsHurting)
            animator.SetTrigger("isHurting");
        if (CombatSystem.instance.enemyUnit.IsDead())
            animator.SetTrigger("isDead");
    }

}
