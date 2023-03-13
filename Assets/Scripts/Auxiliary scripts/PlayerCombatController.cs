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
        if (CombatSystem.instance.playerAttackButtonWasPressed)
            animator.SetTrigger("attack");
        if (CombatSystem.instance.playerIsHurting)
            animator.SetTrigger("isHurting");
    }
}
