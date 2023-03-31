using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    protected Animator animator;
    [SerializeField] private bool isPlayer;
    [HideInInspector] public bool attackButtonWasPressed;
    [HideInInspector] public bool isHurting;
    [HideInInspector] public bool isDying;

    private void Start() => animator = GetComponent<Animator>();

    private void Update()
    {
        if (isPlayer)
            animator.SetFloat("moveX", -1);
        SetAnimationCombat();
    }

    private void SetAnimationCombat()
    {
        if (attackButtonWasPressed)
            animator.SetTrigger("attack");
        else if (isHurting)
            animator.SetTrigger("isHurting");
        else if (isDying)
            animator.SetTrigger("isDead");
    }
}
