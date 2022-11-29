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
    //FireEnemyAI fireEnemyAI;
    //ElectraEnemyAI electraEnemyAI;
    BossEnemyAI bossEnemyAI;

    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject enemyPrefab;

    [SerializeField] Transform playerCombatPosition;
    [SerializeField] Transform enemyCombatPosition;

    [SerializeField] CombatHUD playerHUD;
    [SerializeField] CombatHUD enemyHUD;

    [SerializeField] CombatState combatState;
    public CombatUI combatUI;

    [SerializeField] Camera mainCamera;
    [SerializeField] Camera combatCamera;

    public int[] mentalSkillsMPCost;
    float initArmorModifier = 1;
    public static CombatSystem instance;
    
    void Start()
    {
        combatState = CombatState.START;
        instance = this;
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

        //fireEnemyAI = enemyCombat.GetComponent<FireEnemyAI>();
        //electraEnemyAI = enemyCombat.GetComponent<ElectraEnemyAI>();
        bossEnemyAI = enemyCombat.GetComponent<BossEnemyAI>();

        enemyUnit.knockedTurnsCount = 0;
        enemyUnit.knockedDownTimeout = 0;

        combatUI.gameObject.SetActive(true);
        //playerHUD.SetHUD(playerUnit);
        //enemyHUD.SetHUD(enemyUnit);

        combatCamera.enabled = true;
        mainCamera.enabled = false;
        combatUI.combatDialogue.text = "Битва началась";

        yield return new WaitForSeconds(1.5f);

        combatState = CombatState.PLAYER_TURN;
        StartCoroutine(PlayerTurn());
    }


    //-----------------------------(Действия игрока)-------------------------------------------------

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
            combatUI.combatDialogue.text = "Игрок встал на ноги. Так продолжайте же сражаться!";
            yield return new WaitForSeconds(1.5f);
        }
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
            combatUI.combatDialogue.text = "Выберите действие:";
    }

    IEnumerator PlayerAttack()
    {
        int totalDamage = CalcAffinityDamage(0, false, playerUnit, enemyUnit);
        enemyUnit.TakeDamage(totalDamage);
        combatState = CombatState.ENEMY_TURN;
        yield return new WaitForSeconds(1f);
        //enemyHUD.ChangeHP(enemyUnit.currentHP);
        combatUI.combatDialogue.text = "Игрок наносит " + totalDamage + " физического урона";
        yield return new WaitForSeconds(1.5f);
        if (enemyUnit.IsDead())
        {
            combatState = CombatState.WON;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "Игрок одержал победу!";
            yield return new WaitForSeconds(1.5f);
            FinishBattle();
        }
        else if (enemyUnit.isKnockedDown && enemyUnit.knockedTurnsCount == 0)
        {
            combatState = CombatState.PLAYER_TURN;
            enemyUnit.knockedTurnsCount++;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "Враг сбит с ног. Игроку предоставляется еще один ход!";
            yield return new WaitForSeconds(1.5f);
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
        combatUI.combatDialogue.text = "Игрок успешно перешел в защиту";
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(EnemyTurn());
    }

    IEnumerator PlayerPsionaSkill()
    {
        int totalDamage = CalcAffinityDamage(1, true, playerUnit, enemyUnit);
        enemyUnit.TakeDamage(totalDamage);
        playerUnit.ReduceCurrentMP(mentalSkillsMPCost[0]);
        enemyUnit.PsionaEffect();
        combatState = CombatState.ENEMY_TURN;
        yield return new WaitForSeconds(1f);
        //enemyHUD.ChangeHP(enemyUnit.currentHP);
        combatUI.combatDialogue.text = "Игрок наносит " + totalDamage + " псионического урона";
        yield return new WaitForSeconds(1.5f);
        if (enemyUnit.IsDead())
        {
            combatState = CombatState.WON;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "Игрок одержал победу!";
            yield return new WaitForSeconds(1.5f);
            FinishBattle();
        }
        else if (enemyUnit.isKnockedDown && enemyUnit.knockedTurnsCount == 0)
        {
            combatState = CombatState.PLAYER_TURN;
            enemyUnit.knockedTurnsCount++;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "Враг сбит с ног. Игроку предоставляется еще один ход!";
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(PlayerTurn());
        }
        else
            StartCoroutine(EnemyTurn());
    }

    IEnumerator PlayerElectraSkill()
    {
        int totalDamage = CalcAffinityDamage(2, true, playerUnit, enemyUnit);
        enemyUnit.TakeDamage(totalDamage);
        playerUnit.ReduceCurrentMP(mentalSkillsMPCost[1]);
        enemyUnit.ElectraEffect();
        combatState = CombatState.ENEMY_TURN;
        yield return new WaitForSeconds(1f);
        //enemyHUD.ChangeHP(enemyUnit.currentHP);
        combatUI.combatDialogue.text = "Игрок наносит " + totalDamage + " электрического урона";
        yield return new WaitForSeconds(1.5f);
        if (enemyUnit.IsDead())
        {
            combatState = CombatState.WON;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "Игрок одержал победу!";
            yield return new WaitForSeconds(1.5f);
            FinishBattle();
        }
        else if (enemyUnit.isKnockedDown && enemyUnit.knockedTurnsCount == 0)
        {
            combatState = CombatState.PLAYER_TURN;
            enemyUnit.knockedTurnsCount++;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "Враг сбит с ног. Игроку предоставляется еще один ход!";
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(PlayerTurn());
        }
        else
            StartCoroutine(EnemyTurn());
    }

    IEnumerator PlayerFiraSkill()
    {
        int totalDamage = CalcAffinityDamage(3, true, playerUnit, enemyUnit);
        enemyUnit.TakeDamage(totalDamage);
        playerUnit.ReduceCurrentMP(mentalSkillsMPCost[2]);
        enemyUnit.FiraEffect();
        combatState = CombatState.ENEMY_TURN;
        yield return new WaitForSeconds(1f);
        //enemyHUD.ChangeHP(enemyUnit.currentHP);
        combatUI.combatDialogue.text = "Игрок наносит " + totalDamage + " огненного урона";
        yield return new WaitForSeconds(1.5f);
        if (enemyUnit.IsDead())
        {
            combatState = CombatState.WON;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "Игрок одержал победу!";
            yield return new WaitForSeconds(1.5f);
            FinishBattle();
        }
        else if (enemyUnit.isKnockedDown && enemyUnit.knockedTurnsCount == 0)
        {
            combatState = CombatState.PLAYER_TURN;
            enemyUnit.knockedTurnsCount++;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "Враг сбит с ног. Игроку предоставляется еще один ход!";
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(PlayerTurn());
        }
        else
            StartCoroutine(EnemyTurn());
    }

    //-----------------------------(Действия врага)-------------------------------------------------

    public IEnumerator EnemyTurn()
    {
        enemyUnit.UnitEffectUpdate();
        //enemyHUD.ChangeHP(enemyUnit.currentHP);
        //enemyHUD.ChangeMP(enemyUnit.currentMP);
        if (enemyUnit.appliedEffect[1])
        {
            //StopCoroutine(EnemyTurn());
            combatState = CombatState.PLAYER_TURN;
            StartCoroutine(PlayerTurn());
            yield break;
        }
        if (enemyUnit.IsDead())
        {
            combatState = CombatState.WON;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "Игрок одержал победу!";
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
            combatUI.combatDialogue.text = "Враг встал на ноги";
            yield return new WaitForSeconds(1.5f);
        }
        yield return new WaitForSeconds(1f);
        //fireEnemyAI.CombatAI();
        //electraEnemyAI.CombatAI();
        bossEnemyAI.CombatAI();

        //playerHUD.ChangeHP(playerUnit.currentHP);
        if (playerUnit.IsDead())
        {
            combatState = CombatState.LOST;
            FinishBattle();
        }
        else if (playerUnit.isKnockedDown && playerUnit.knockedTurnsCount == 0)
        {            
            playerUnit.knockedTurnsCount++;
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = "Игрок сбит с ног. Снова ход врага!";
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(EnemyTurn());
        }
        else
        {
            yield return new WaitForSeconds(2f);
            combatState = CombatState.PLAYER_TURN;
            StartCoroutine(PlayerTurn());
        }
    }

//-----------------------------(Методы для кнопок)-------------------------------------------------
    
    public void OnAttackButton()
    {
        if (combatState != CombatState.PLAYER_TURN)
            return;
        if (combatUI.mSkillButtonsWereinstantiated && combatUI.buttonShowLock)
        {
            combatUI.buttonShowLock = false;
            combatUI.HideOrShowMentalSkillButtons();
        }
        combatUI.combatDialogue.text = "Игрок начинает атаку";
        StartCoroutine(PlayerAttack());
    }

    public void OnDefendButton()
    {
        if (combatState != CombatState.PLAYER_TURN)
            return;
        if (combatUI.mSkillButtonsWereinstantiated && combatUI.buttonShowLock)
        {
            combatUI.buttonShowLock = false;
            combatUI.HideOrShowMentalSkillButtons();
        }
        combatUI.combatDialogue.text = "Игрок защищается";
        StartCoroutine(PlayerDefend());
    }

    public void OnMentalButton()
    {
        if (combatState != CombatState.PLAYER_TURN)
            return;
        combatUI.buttonShowLock = true;
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
            combatUI.combatDialogue.text = "Игрок использует пси навык";
            combatUI.HideOrShowMentalSkillButtons();
            combatUI.buttonShowLock = false;
            StartCoroutine(PlayerPsionaSkill());
        }
        else
        {
            combatUI.combatDialogue.text = "Недостаточно MP для использования навыка";
            combatUI.HideOrShowMentalSkillButtons();
            combatUI.buttonShowLock = false;
        }
    }

    public void OnElectraButton()
    {
        if (playerUnit.currentMP >= mentalSkillsMPCost[0])
        {
            combatUI.combatDialogue.text = "Игрок использует электро навык";
            combatUI.HideOrShowMentalSkillButtons();
            combatUI.buttonShowLock = false;
            StartCoroutine(PlayerElectraSkill());
        }
        else
        {
            combatUI.combatDialogue.text = "Недостаточно MP для использования навыка";
            combatUI.HideOrShowMentalSkillButtons();
            combatUI.buttonShowLock = false;
        }
    }

    public void OnFiraButton()
    {
        if (playerUnit.currentMP >= mentalSkillsMPCost[0])
        {
            combatUI.combatDialogue.text = "Игрок использует огненный навык";
            combatUI.HideOrShowMentalSkillButtons();
            combatUI.buttonShowLock = false;
            StartCoroutine(PlayerFiraSkill());
        }
        else
        {
            combatUI.combatDialogue.text = "Недостаточно MP для использования навыка";
            combatUI.HideOrShowMentalSkillButtons();
            combatUI.buttonShowLock = false;
        }
    }

    //--------------------------------------------------------------------------------

    void FinishBattle()
    {
        StopAllCoroutines();
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
            combatUI.combatDialogue.text = "Игрок оказался повержен...";
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
