using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CombatState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST }

public class CombatSystem : MonoBehaviour
{
    public const float weaknessMult = 1.25f;
    public const float resistMult = 0.5f;
    public const float knockedDownMult = 1.2f;

    public Unit playerUnit;
    public Unit enemyUnit;
    EnemyAI enemyAI;

    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject enemyPrefab;

    [SerializeField] Transform playerCombatPosition;
    [SerializeField] Transform enemyCombatPosition;

    [SerializeField] CombatHUD playerHUD;
    [SerializeField] CombatHUD enemyHUD;

    [SerializeField] CombatState combatState;
    public CombatUI combatUI;
    bool mSkillButtonsWereinstantiated;

    [SerializeField] Camera mainCamera;
    [SerializeField] Camera combatCamera;

    public int[] mentalSkillsMPCost;
    float initArmorModifier = 1;

    //System.Random random = new System.Random();

    void Start()
    {
        combatState = CombatState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        GameObject playerCombat = Instantiate(playerPrefab, playerCombatPosition);
        playerUnit = playerCombat.GetComponent<Unit>();
        playerUnit.knockedTurnsCount = 0;
        playerUnit.knockedDownTimeout = 0;

        GameObject enemyCombat = Instantiate(enemyPrefab, enemyCombatPosition);
        enemyUnit = enemyCombat.GetComponent<Unit>();
        enemyAI = enemyCombat.GetComponent<EnemyAI>();
        enemyUnit.knockedTurnsCount = 0;
        enemyUnit.knockedDownTimeout = 0;

        combatUI.gameObject.SetActive(true);
        //playerHUD.SetHUD(playerUnit);
        //enemyHUD.SetHUD(enemyUnit);

        combatCamera.enabled = true;
        mainCamera.enabled = false;
        combatUI.combatDialogue.text = "����� ��������";

        yield return new WaitForSeconds(2f);

        combatState = CombatState.PLAYER_TURN;
        StartCoroutine(PlayerTurn());
    }


    //-----------------------------(�������� ������)-------------------------------------------------

    IEnumerator PlayerTurn()
    {
        if (playerUnit.armorModifier != initArmorModifier)
            playerUnit.armorModifier = initArmorModifier;
        if (playerUnit.knockedDownTimeout > 0)
            playerUnit.knockedDownTimeout--;
        if (playerUnit.isKnockedDown)
        {
            playerUnit.isKnockedDown = false;
            playerUnit.knockedTurnsCount = 0;
            playerUnit.knockedDownTimeout = 3;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "����� ����� �� ����. ��� ����������� �� ���������!";
            yield return new WaitForSeconds(1f);
        }
        combatUI.combatDialogue.text = "�������� ��������:";
    }

    IEnumerator PlayerAttack()
    {
        int totalDamage = CalcAffinityDamage(0, false, playerUnit, enemyUnit);
        enemyUnit.TakeDamage(totalDamage);
        combatState = CombatState.ENEMY_TURN;
        yield return new WaitForSeconds(1f);
        //enemyHUD.ChangeHP(enemyUnit.currentHP);
        combatUI.combatDialogue.text = "����� ������� " + totalDamage + " ����������� �����";
        yield return new WaitForSeconds(1f);
        if (enemyUnit.IsDead())
        {
            combatState = CombatState.WON;
            combatUI.combatDialogue.text = "����� ������� ������!";
            Invoke(nameof(FinishBattle), 2);
        }
        else if (enemyUnit.isKnockedDown && enemyUnit.knockedTurnsCount == 0)
        {
            combatState = CombatState.PLAYER_TURN;
            enemyUnit.knockedTurnsCount++;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "���� ���� � ���. ������ ��������������� ��� ���� ���!";
            yield return new WaitForSeconds(1f);
            StartCoroutine(PlayerTurn());
        }
        else
            StartCoroutine(EnemyTurn());
    }

    IEnumerator PlayerDefend()
    {
        initArmorModifier = playerUnit.armorModifier;
        playerUnit.armorModifier = 0.4f;
        combatState = CombatState.ENEMY_TURN;
        yield return new WaitForSeconds(1f);
        combatUI.combatDialogue.text = "����� ������� ������� � ������";
        yield return new WaitForSeconds(1f);
        StartCoroutine(EnemyTurn());
    }

    IEnumerator PlayerPsionaSkill()
    {
        int totalDamage = CalcAffinityDamage(1, true, playerUnit, enemyUnit);
        enemyUnit.TakeDamage(totalDamage);
        playerUnit.ReduceCurrentMP(mentalSkillsMPCost[0]);
        combatState = CombatState.ENEMY_TURN;
        yield return new WaitForSeconds(1f);
        //enemyHUD.ChangeHP(enemyUnit.currentHP);
        combatUI.combatDialogue.text = "����� ������� " + totalDamage + " ������������� �����";
        yield return new WaitForSeconds(1f);
        if (enemyUnit.IsDead())
        {
            combatState = CombatState.WON;
            combatUI.combatDialogue.text = "����� ������� ������!";
            Invoke(nameof(FinishBattle), 2);
        }
        else if (enemyUnit.isKnockedDown && enemyUnit.knockedTurnsCount == 0)
        {
            combatState = CombatState.PLAYER_TURN;
            enemyUnit.knockedTurnsCount++;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "���� ���� � ���. ������ ��������������� ��� ���� ���!";
            yield return new WaitForSeconds(1f);
            StartCoroutine(PlayerTurn());
        }
        else
            StartCoroutine(EnemyTurn());
    }

//-----------------------------(�������� �����)-------------------------------------------------

    IEnumerator EnemyTurn()
    {
        if (enemyUnit.knockedDownTimeout > 0)
            enemyUnit.knockedDownTimeout--;
        if (enemyUnit.isKnockedDown)
        {
            enemyUnit.isKnockedDown = false;
            enemyUnit.knockedTurnsCount = 0;
            enemyUnit.knockedDownTimeout = 3;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "���� ����� �� ����";
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(1f);
        enemyAI.CombatAI();
        //playerHUD.ChangeHP(playerUnit.currentHP);
        yield return new WaitForSeconds(1f);       
        if (playerUnit.IsDead())
        {
            combatState = CombatState.LOST;
            FinishBattle();
        }
        else if (playerUnit.isKnockedDown && playerUnit.knockedTurnsCount == 0)
        {            
            playerUnit.knockedTurnsCount++;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "����� ���� � ���. ����� ��� �����!";
            yield return new WaitForSeconds(1f);
            StartCoroutine(EnemyTurn());
        }
        else
        {
            combatState = CombatState.PLAYER_TURN;
            StartCoroutine(PlayerTurn());
        }
    }

//-----------------------------(������ ��� ������)-------------------------------------------------
    
    public void OnAttackButton()
    {
        if (combatState != CombatState.PLAYER_TURN)
            return;
        combatUI.combatDialogue.text = "����� �������� �����";
        StartCoroutine(PlayerAttack());
    }

    public void OnDefendButton()
    {
        if (combatState != CombatState.PLAYER_TURN)
            return;
        combatUI.combatDialogue.text = "����� ����������";
        StartCoroutine(PlayerDefend());
    }

    public void OnMentalButton()
    {
        if (combatState != CombatState.PLAYER_TURN)
            return;
        if (mSkillButtonsWereinstantiated)
            combatUI.HideOrShowMentalSkillButtons();
        else
        {            
            combatUI.SetMentalSkillButtons();
            mSkillButtonsWereinstantiated = true;
        }
    }

    public void OnPsionaButton()
    {
        if (playerUnit.currentMP >= mentalSkillsMPCost[0])
        {
            combatUI.combatDialogue.text = "����� ���������� ��� �����";
            combatUI.HideOrShowMentalSkillButtons();
            StartCoroutine(PlayerPsionaSkill());
        }
        else
        {
            combatUI.combatDialogue.text = "������������ MP ��� ������������� ������";
            combatUI.HideOrShowMentalSkillButtons();
        }
    }

//------------------------------------------------------------------------------------------------

    void FinishBattle()
    {
        if (combatState == CombatState.WON)
        {           
            mainCamera.enabled = true;
            combatCamera.enabled = false;
            combatUI.gameObject.SetActive(false);
            Destroy(playerCombatPosition.GetChild(0).gameObject);
            Destroy(enemyCombatPosition.GetChild(0).gameObject);
        }
        else if (combatState == CombatState.LOST)
        {
            combatUI.combatDialogue.text = "����� �������� ��������...";
            playerCombatPosition.GetChild(0).gameObject.GetComponent<Unit>().Death();
        }
    }

    public int CalcAffinityDamage(int damageTypeID, bool isMental, Unit attackingUnit, Unit defendingUnit)
    {        
        if (defendingUnit.nulls[damageTypeID])
            return 0;
        float attackStrength;
        if (isMental)
            attackStrength = attackingUnit.mentalAttackStrength * attackingUnit.damageTypeAffinities[damageTypeID];
        else
            attackStrength = attackingUnit.meleeAttackStrength;
        float totalDamage = attackStrength * defendingUnit.armorModifier;        
        if (defendingUnit.weaknesses[damageTypeID] && !defendingUnit.isKnockedDown && defendingUnit.knockedDownTimeout == 0)
        {
            totalDamage *= weaknessMult;
            defendingUnit.isKnockedDown = true;
            defendingUnit.knockedTurnsCount = 0;
            return (int)totalDamage;
        }
        if (defendingUnit.isKnockedDown)
            totalDamage *= knockedDownMult;
        if (defendingUnit.weaknesses[damageTypeID])
            totalDamage *= weaknessMult;
        else if (defendingUnit.resistances[damageTypeID])
            totalDamage *= resistMult;
        return (int)totalDamage;
    }
}
