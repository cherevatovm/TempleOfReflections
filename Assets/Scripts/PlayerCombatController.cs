using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetFloat("moveX", -1);
        SetAnimationCombat();
    }

    void SetAnimationCombat()
    {
        if (CombatSystem.instance.attackButtonWasPressed)
            animator.SetBool("isAttacking", true);
        else
            animator.SetBool("isAttacking", false);

        if (CombatSystem.instance.playerUnit.isHurting)
            animator.SetBool("isHurting", true);
        else
            animator.SetBool("isHurting", false);

        if (CombatSystem.instance.playerUnit.IsDead())
            animator.SetBool("isDeath", true);
        else
            animator.SetBool("isDeath", false);
    }

    //if (CombatSystem.instance.attackButtonWasPressed)
    //        animator.SetBool("isAttacking", true);
    //    else
    //        animator.SetBool("isAttacking", false);
}
