using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatController : MonoBehaviour
{
    private Animator animator;
    public bool attackButtonWasPressed;
    public bool isHurting;
    public bool isDying;

    private void Start() => animator = GetComponent<Animator>();

    private void Update()
    {
        if (attackButtonWasPressed)
            animator.SetTrigger("attack");
        else if (isHurting)
            animator.SetTrigger("isHurting");
        else if (isDying)
            animator.SetTrigger("isDead");
    }

}
