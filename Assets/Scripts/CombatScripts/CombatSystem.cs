using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum CombatState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST }

public class CombatSystem : MonoBehaviour
{
    const float weaknessMult = 1.25f;
    const float resistMult = 0.5f;
    const float knockedDownMult = 1.2f;
    public const float effectProbability = 0.2f;

    public Unit playerUnit;
    public Unit enemyUnit;
    EnemyAI enemyAI;

    public EnemyAI encounteredEnemy;

    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject[] enemyPrefabs;

    [SerializeField] Transform playerCombatPosition;
    [SerializeField] Transform enemyCombatPosition;

    [SerializeField] CombatHUD playerHUD;
    [SerializeField] CombatHUD enemyHUD;

    [SerializeField] CombatState combatState;
    public CombatUI combatUI;

    [SerializeField] Camera mainCamera;
    [SerializeField] Camera combatCamera;

    float initArmorModifier = 1;
    public int[] mentalSkillsMPCost;
    public bool isInCombat;
    public bool wasAnItemUsed;
    public static CombatSystem instance;

    void Start()
    {
        combatCamera.enabled = false;
        combatState = CombatState.START;
        instance = this;
        //StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        SoundManager.StopLoopedSound();
        SoundManager.PlaySound(SoundManager.Sound.EnterCombat);
        SoundManager.PlaySound(SoundManager.Sound.MentalBattle);

        encounteredEnemy.gameObject.GetComponent<Collider2D>().enabled = false;
        encounteredEnemy.gameObject.GetComponent<EnemyMovement>().enabled = false;
        Inventory.instance.attachedUnit.GetComponent<PlayerMovement>().enabled = false;

        isInCombat = true;
        GameObject playerCombat = Instantiate(playerPrefab, playerCombatPosition);
        playerUnit = playerCombat.GetComponent<Unit>();
        playerUnit.CopyStats(Inventory.instance.attachedUnit);
        playerUnit.knockedTurnsCount = 0;
        playerUnit.knockedDownTimeout = 0;


        GameObject enemyCombat = Instantiate(enemyPrefabs[encounteredEnemy.enemyID], enemyCombatPosition);
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

        yield return new WaitForSeconds(1.5f);

        combatState = CombatState.PLAYER_TURN;
        StartCoroutine(PlayerTurn());
    }


    //-----------------------------(�������� ������)-------------------------------------------------

    public IEnumerator PlayerTurn()
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
        wasAnItemUsed = false;
        playerUnit.UnitEffectUpdate();
        //playerHUD.ChangeHP(playerUnit.currentHP);
        //playerHUD.ChangeMP(playerUnit.currentMP);
        yield return new WaitForSeconds(1f);
        if (playerUnit.appliedEffect[1])
        {
            yield return new WaitForSeconds(1f);
            combatState = CombatState.ENEMY_TURN;
            StartCoroutine(EnemyTurn());
        }
        else if (playerUnit.IsDead())
        {
            combatState = CombatState.LOST;
            FinishBattle();
        }
        else
            combatUI.combatDialogue.text = "�������� ��������:";
    }

    IEnumerator PlayerAttack()
    {
        SoundManager.PlaySound(SoundManager.Sound.WeaponSwingWithHit);
        int totalDamage = CalcAffinityDamage(0, false, playerUnit, enemyUnit);
        enemyUnit.TakeDamage(totalDamage);
        combatState = CombatState.ENEMY_TURN;
        yield return new WaitForSeconds(1f);
        //enemyHUD.ChangeHP(enemyUnit.currentHP);
        combatUI.combatDialogue.text = "����� ������� " + totalDamage + " ����������� �����";
        yield return new WaitForSeconds(1.5f);
        if (enemyUnit.IsDead())
        {
            combatState = CombatState.WON;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "����� ������� ������!";
            yield return new WaitForSeconds(1.5f);
            FinishBattle();
        }
        else if (enemyUnit.isKnockedDown && enemyUnit.knockedTurnsCount == 0)
        {
            combatState = CombatState.PLAYER_TURN;
            enemyUnit.knockedTurnsCount++;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "���� ���� � ���. ������ ��������������� ��� ���� ���!";
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(PlayerTurn());
        } 
        else
            StartCoroutine(EnemyTurn());
    }

    IEnumerator PlayerDefend()
    {
        initArmorModifier = playerUnit.armorModifier;
        playerUnit.armorModifier *= 0.4f;
        combatState = CombatState.ENEMY_TURN;
        yield return new WaitForSeconds(1f);
        combatUI.combatDialogue.text = "����� ������� ������� � ������";
        yield return new WaitForSeconds(1f);
        StartCoroutine(EnemyTurn());
    }

    public IEnumerator PlayerUsingItem()
    {
        combatUI.combatDialogue.text = "����� ���������� �������";
        yield return new WaitForSeconds(1f);
        StartCoroutine(EnemyTurn());
    }

    IEnumerator PlayerPsionaSkill()
    {
        SoundManager.PlaySound(SoundManager.Sound.PsiSkill);
        int totalDamage = CalcAffinityDamage(1, true, playerUnit, enemyUnit);
        enemyUnit.TakeDamage(totalDamage);
        playerUnit.ReduceCurrentMP(mentalSkillsMPCost[0]);
        enemyUnit.PsionaEffect();
        combatState = CombatState.ENEMY_TURN;
        yield return new WaitForSeconds(1f);
        //enemyHUD.ChangeHP(enemyUnit.currentHP);
        combatUI.combatDialogue.text = "����� ������� " + totalDamage + " ������������� �����";
        yield return new WaitForSeconds(1.5f);
        if (enemyUnit.IsDead())
        {
            combatState = CombatState.WON;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "����� ������� ������!";
            yield return new WaitForSeconds(1.5f);
            FinishBattle();
        }
        else if (enemyUnit.isKnockedDown && enemyUnit.knockedTurnsCount == 0)
        {
            combatState = CombatState.PLAYER_TURN;
            enemyUnit.knockedTurnsCount++;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "���� ���� � ���. ������ ��������������� ��� ���� ���!";
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(PlayerTurn());
        }
        else
            StartCoroutine(EnemyTurn());
    }

    IEnumerator PlayerElectraSkill()
    {
        SoundManager.PlaySound(SoundManager.Sound.ElectraSkill);
        int totalDamage = CalcAffinityDamage(2, true, playerUnit, enemyUnit);
        enemyUnit.TakeDamage(totalDamage);
        playerUnit.ReduceCurrentMP(mentalSkillsMPCost[1]);
        enemyUnit.ElectraEffect();
        combatState = CombatState.ENEMY_TURN;
        yield return new WaitForSeconds(1f);
        //enemyHUD.ChangeHP(enemyUnit.currentHP);
        combatUI.combatDialogue.text = "����� ������� " + totalDamage + " �������������� �����";
        yield return new WaitForSeconds(1.5f);
        if (enemyUnit.IsDead())
        {
            combatState = CombatState.WON;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "����� ������� ������!";
            yield return new WaitForSeconds(1.5f);
            FinishBattle();
        }
        else if (enemyUnit.isKnockedDown && enemyUnit.knockedTurnsCount == 0)
        {
            combatState = CombatState.PLAYER_TURN;
            enemyUnit.knockedTurnsCount++;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "���� ���� � ���. ������ ��������������� ��� ���� ���!";
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(PlayerTurn());
        }
        else
            StartCoroutine(EnemyTurn());
    }

    IEnumerator PlayerFiraSkill()
    {
        SoundManager.PlaySound(SoundManager.Sound.FiraSkill);
        int totalDamage = CalcAffinityDamage(3, true, playerUnit, enemyUnit);
        enemyUnit.TakeDamage(totalDamage);
        playerUnit.ReduceCurrentMP(mentalSkillsMPCost[2]);
        enemyUnit.FiraEffect();
        combatState = CombatState.ENEMY_TURN;
        yield return new WaitForSeconds(1f);
        //enemyHUD.ChangeHP(enemyUnit.currentHP);
        combatUI.combatDialogue.text = "����� ������� " + totalDamage + " ��������� �����";
        yield return new WaitForSeconds(1.5f);
        if (enemyUnit.IsDead())
        {
            combatState = CombatState.WON;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "����� ������� ������!";
            yield return new WaitForSeconds(1.5f);
            FinishBattle();
        }
        else if (enemyUnit.isKnockedDown && enemyUnit.knockedTurnsCount == 0)
        {
            combatState = CombatState.PLAYER_TURN;
            enemyUnit.knockedTurnsCount++;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "���� ���� � ���. ������ ��������������� ��� ���� ���!";
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(PlayerTurn());
        }
        else
            StartCoroutine(EnemyTurn());
    }

    //-----------------------------(�������� �����)-------------------------------------------------

    public IEnumerator EnemyTurn()
    {
        enemyUnit.UnitEffectUpdate();
        //enemyHUD.ChangeHP(enemyUnit.currentHP);
        //enemyHUD.ChangeMP(enemyUnit.currentMP);
        if (enemyUnit.appliedEffect[1])
        {
            combatState = CombatState.PLAYER_TURN;
            yield return new WaitForSeconds(1f);
            StartCoroutine(PlayerTurn());
            yield break;
        }
        if (enemyUnit.IsDead())
        {
            combatState = CombatState.WON;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "����� ������� ������!";
            yield return new WaitForSeconds(1.5f);
            FinishBattle();
        }
        if (enemyUnit.knockedDownTimeout > 0)
            enemyUnit.knockedDownTimeout--;
        if (enemyUnit.isKnockedDown)
        {
            enemyUnit.isKnockedDown = false;
            enemyUnit.knockedTurnsCount = 0;
            enemyUnit.knockedDownTimeout = 3;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "���� ����� �� ����";
            yield return new WaitForSeconds(1.5f);
        }
        yield return new WaitForSeconds(1f);
        enemyAI.CombatAI(out string effectMessage);
        if (!string.IsNullOrEmpty(effectMessage))
        {
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = effectMessage;
        }
        //playerHUD.ChangeHP(playerUnit.currentHP);
        yield return new WaitForSeconds(1.5f);
        if (playerUnit.IsDead())
        {
            combatState = CombatState.LOST;
            yield return new WaitForSeconds(1f);
            FinishBattle();
        }
        else if (playerUnit.isKnockedDown && playerUnit.knockedTurnsCount == 0)
        {            
            playerUnit.knockedTurnsCount++;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "����� ���� � ���. ����� ��� �����!";
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(EnemyTurn());
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
            combatState = CombatState.PLAYER_TURN;
            StartCoroutine(PlayerTurn());
        }
    }

    //-----------------------------(������ ��� ������)-------------------------------------------------

    public void OnItemButton()
    {
        if (combatState != CombatState.PLAYER_TURN || wasAnItemUsed)
            return;
        if (Inventory.instance.isOpened)
            Inventory.instance.Close();
        else
            Inventory.instance.Open();
    }

    public void OnAttackButton()
    {
        if (combatState != CombatState.PLAYER_TURN)
            return;
        if (Inventory.instance.isOpened)
            Inventory.instance.Close();
        if (combatUI.mSkillButtonsWereinstantiated && combatUI.areButtonsShown)
            combatUI.HideOrShowMentalSkillButtons();
        combatUI.combatDialogue.text = "����� �������� �����";
        StartCoroutine(PlayerAttack());
    }

    public void OnDefendButton()
    {
        if (combatState != CombatState.PLAYER_TURN)
            return;
        if (Inventory.instance.isOpened)
            Inventory.instance.Close();
        if (combatUI.mSkillButtonsWereinstantiated && combatUI.areButtonsShown)
            combatUI.HideOrShowMentalSkillButtons();
        combatUI.combatDialogue.text = "����� ����������";
        StartCoroutine(PlayerDefend());
    }

    public void OnMentalButton()
    {
        if (combatState != CombatState.PLAYER_TURN)
            return;
        if (Inventory.instance.isOpened)
            Inventory.instance.Close();
        if (combatUI.mSkillButtonsWereinstantiated)
            combatUI.HideOrShowMentalSkillButtons();
        else
        {
            combatUI.SetMentalSkillButtons();
            combatUI.mSkillButtonsWereinstantiated = true;
        }
    }

    public void OnPsionaButton()
    {
        if (playerUnit.currentMP >= mentalSkillsMPCost[0])
        {
            combatUI.combatDialogue.text = "����� ���������� ��� �����";
            combatUI.HideOrShowMentalSkillButtons();
            combatUI.areButtonsShown = false;
            StartCoroutine(PlayerPsionaSkill());
        }
        else
        {
            combatUI.combatDialogue.text = "������������ MP ��� ������������� ������";
            combatUI.HideOrShowMentalSkillButtons();
            combatUI.areButtonsShown = false;
        }
    }

    public void OnElectraButton()
    {
        if (playerUnit.currentMP >= mentalSkillsMPCost[0])
        {
            combatUI.combatDialogue.text = "����� ���������� ������������� �����";
            combatUI.HideOrShowMentalSkillButtons();
            combatUI.areButtonsShown = false;
            StartCoroutine(PlayerElectraSkill());
        }
        else
        {
            combatUI.combatDialogue.text = "������������ MP ��� ������������� ������";
            combatUI.HideOrShowMentalSkillButtons();
            combatUI.areButtonsShown = false;
        }
    }

    public void OnFiraButton()
    {
        if (playerUnit.currentMP >= mentalSkillsMPCost[0])
        {
            combatUI.combatDialogue.text = "����� ���������� �������� �����";
            combatUI.HideOrShowMentalSkillButtons();
            combatUI.areButtonsShown = false;
            StartCoroutine(PlayerFiraSkill());
        }
        else
        {
            combatUI.combatDialogue.text = "������������ MP ��� ������������� ������";
            combatUI.HideOrShowMentalSkillButtons();
            combatUI.areButtonsShown = false;
        }
    }

    //--------------------------------------------------------------------------------

    void FinishBattle()
    {
        StopAllCoroutines();
        if (combatState == CombatState.WON)
        {
            SoundManager.StopLoopedSound();
            SoundManager.PlaySound(SoundManager.Sound.EnterCombat);
            SoundManager.PlaySound(SoundManager.Sound.Mystery);
            wasAnItemUsed = false;
            Inventory.instance.attachedUnit.CopyStats(playerUnit);
            Inventory.instance.attachedUnit.GetComponent<PlayerMovement>().enabled = true;
            isInCombat = false;
            mainCamera.enabled = true;
            combatCamera.enabled = false;
            combatUI.gameObject.SetActive(false);
            Destroy(playerCombatPosition.GetChild(0).gameObject);
            Destroy(enemyCombatPosition.GetChild(0).gameObject);
            Destroy(encounteredEnemy.gameObject);
        }
        else if (combatState == CombatState.LOST)
        {
            combatUI.combatDialogue.text = "����� �������� ��������...";
            Inventory.instance.attachedUnit.Death();
        }
    }

    public int CalcAffinityDamage(int damageTypeID, bool isMental, Unit attackingUnit, Unit defendingUnit)
    {        
        if (defendingUnit.nulls[damageTypeID])
            return 0;
        float attackStrength;
        if (isMental)
            attackStrength = attackingUnit.mentalAttackStrength * attackingUnit.elementAffinities[damageTypeID];
        else
            attackStrength = attackingUnit.meleeAttackStrength;
        float totalDamage = attackStrength * defendingUnit.armorModifier;        
        if (!defendingUnit.appliedEffect[1] && defendingUnit.weaknesses[damageTypeID] && !defendingUnit.isKnockedDown && defendingUnit.knockedDownTimeout == 0)
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
