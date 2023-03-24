using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : Unit
{
    //[HideInInspector] public CombatHUD combatHUD;
    public CombatHUD combatHUD;
    public int allyID;
    public bool isChoosingEnemyForAttack;
    //Animator animator;

    private void Start()
    {
        allyID = 0;
        //animator = GetComponent<Animator>();
    }

    private void Update()
    {
        //animator.SetBool("reverse", true);
    }

    private void OnMouseDown()
    {
        if (CombatSystem.instance.isInCombat && CombatSystem.instance.isChoosingAllyForItem)
        {
            CombatSystem.instance.curAllyID = CombatSystem.instance.allyUnits.IndexOf(this);
            CombatSystem.instance.isChoosingAllyForItem = false;
            CombatSystem.instance.activeSlot.UseItemInSlot();
        }
    }
}
