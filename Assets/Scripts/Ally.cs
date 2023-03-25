using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : Unit
{
    //[HideInInspector] public CombatHUD combatHUD;
    public CombatHUD combatHUD;
    public int allyID;
    public bool isChoosingEnemyForAttack;

    private void Start()
    {
        allyID = 0;
        Transform transform = GetComponent<Transform>();
    }

    private void Update()
    {
        Vector3 scale = transform.localScale;
        scale.x = -1f;
        transform.localScale = scale;
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
