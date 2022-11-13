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
    
    public TextMeshProUGUI combatDialogue;
    [SerializeField] CombatState combatState;

    //Вынести в отдельный скрипт интерфейса боевки
    [SerializeField] Canvas combatUI;
    //[SerializeField] Button[] psiSkillButtons;

    [SerializeField] Camera mainCamera;
    [SerializeField] Camera combatCamera;

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

        //playerHUD.SetHUD(playerUnit);
        //enemyHUD.SetHUD(enemyUnit);

        combatCamera.enabled = true;
        mainCamera.enabled = false;
        combatDialogue.text = "Битва началась";

        yield return new WaitForSeconds(2f);

        combatState = CombatState.PLAYER_TURN;
        StartCoroutine(PlayerTurn());
    }


    //-----------------------------(Действия игрока)-------------------------------------------------

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
            combatDialogue.text = "Игрок встал на ноги. Так продолжайте же сражаться!";
            yield return new WaitForSeconds(1f);
        }
        combatDialogue.text = "Выберите действие:";
    }

    IEnumerator PlayerAttack()
    {
        int totalDamage = CalcAffinityDamage(0, playerUnit, enemyUnit);
        enemyUnit.TakeDamage(totalDamage);
        combatState = CombatState.ENEMY_TURN;
        yield return new WaitForSeconds(1f);
        //enemyHUD.ChangeHP(enemyUnit.currentHP);
        combatDialogue.text = "Игрок наносит " + totalDamage + " урона";
        yield return new WaitForSeconds(1f);
        if (enemyUnit.IsDead())
        {
            combatState = CombatState.WON;
            combatDialogue.text = "Игрок одержал победу!";
            Invoke(nameof(FinishBattle), 2);
        }
        else if (enemyUnit.isKnockedDown && enemyUnit.knockedTurnsCount == 0)
        {
            combatState = CombatState.PLAYER_TURN;
            enemyUnit.knockedTurnsCount++;
            yield return new WaitForSeconds(1f);
            combatDialogue.text = "Враг сбит с ног. Игроку предоставляется еще один ход!";
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
        combatDialogue.text = "Игрок успешно перешел в защиту";
        yield return new WaitForSeconds(1f);
        StartCoroutine(EnemyTurn());
    }

    IEnumerator PlayerPsiSkill()
    {
        yield return new WaitForSeconds(1f);

    }

//-----------------------------(Действия врага)-------------------------------------------------

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
            combatDialogue.text = "Враг встал на ноги";
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
            combatDialogue.text = "Игрок сбит с ног. Снова ход врага!";
            yield return new WaitForSeconds(1f);
            StartCoroutine(EnemyTurn());
        }
        else
        {
            combatState = CombatState.PLAYER_TURN;
            StartCoroutine(PlayerTurn());
        }
    }

//-----------------------------(Методы для кнопок)-------------------------------------------------
    
    public void OnAttackButton()
    {
        if (combatState != CombatState.PLAYER_TURN)
            return;
        combatDialogue.text = "Игрок начинает атаку";
        StartCoroutine(PlayerAttack());
    }

    public void OnDefendButton()
    {
        if (combatState != CombatState.PLAYER_TURN)
            return;
        combatDialogue.text = "Игрок защищается";
        StartCoroutine(PlayerDefend());
    }

    public void OnPsiButton()
    {
        if (combatState != CombatState.PLAYER_TURN)
            return;
        combatDialogue.text = "Игрок использует пси-навык";
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
            combatDialogue.text = "Игрок оказался повержен...";
            playerCombatPosition.GetChild(0).gameObject.GetComponent<Unit>().Death();
        }
    }

    public int CalcAffinityDamage(int damageTypeID, Unit attackingFighter, Unit defendingFighter)
    {        
        if (defendingFighter.nulls[damageTypeID])
            return 0;
        float totalDamage = attackingFighter.attackStrength * defendingFighter.armorModifier;        
        if (defendingFighter.weaknesses[damageTypeID] && !defendingFighter.isKnockedDown)
        {
            totalDamage *= weaknessMult;
            defendingFighter.isKnockedDown = true;
            defendingFighter.knockedTurnsCount = 0;
            return (int)totalDamage;
        }
        if (defendingFighter.isKnockedDown)
            totalDamage *= knockedDownMult;
        if (defendingFighter.weaknesses[damageTypeID])
            totalDamage *= weaknessMult;
        else if (defendingFighter.resistances[damageTypeID])
            totalDamage *= resistMult;
        return (int)totalDamage;
    }
}
