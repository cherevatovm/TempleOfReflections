using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum CombatState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST }

public class CombatSystem : MonoBehaviour
{
    private const float weaknessMult = 1.25f;
    private const float resistMult = 0.5f;
    private const float knockedDownMult = 1.2f;
    public const float effectProbability = 0.2f;
    
    public float reflectionProbability1;
    public float reflectionProbability2;

    [HideInInspector] public Unit playerUnit;
    [HideInInspector] public Unit enemyUnit;
    private EnemyAI enemyAI;

    [HideInInspector] public EnemyAI encounteredEnemy;

    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject[] enemyPrefabs;

    [SerializeField] Transform playerCombatPosition;
    [SerializeField] Transform[] enemyCombatPositions = new Transform[3];
    private Transform enemyCombatPosition;

    [SerializeField] CombatHUD playerHUD;
    public CombatHUD enemyHUD;

    [SerializeField] CombatState combatState;
    public CombatUI combatUI;

    [SerializeField] Camera mainCamera;
    [SerializeField] Camera combatCamera;

    private float initArmorModifier = 1;
    public int[] mentalSkillsMPCost;
    [HideInInspector] public bool isInCombat;
    [HideInInspector] public bool wasAnItemUsed;
    public static CombatSystem instance;
    private System.Random random = new System.Random();

    [HideInInspector] public bool playerAttackButtonWasPressed;
    [HideInInspector] public bool enemyAttackButtonWasPressed;
    [HideInInspector] public bool playerIsHurting;
    [HideInInspector] public bool enemyIsHurting;

    private void Start()
    {
        combatCamera.enabled = false;
        combatState = CombatState.START;
        instance = this;
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

        enemyCombatPosition = enemyCombatPositions[encounteredEnemy.enemyID];
        GameObject enemyCombat = Instantiate(enemyPrefabs[encounteredEnemy.enemyID], enemyCombatPosition);
        enemyUnit = enemyCombat.GetComponent<Unit>();
        enemyAI = enemyCombat.GetComponent<EnemyAI>();
        enemyUnit.knockedTurnsCount = 0;
        enemyUnit.knockedDownTimeout = 0;

        GameUI.instance.gameObject.SetActive(false);
        combatUI.gameObject.SetActive(true);
        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);
        combatUI.UpdateMentalSkillButtons();

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
            yield return new WaitForSeconds(1f);
        }
        wasAnItemUsed = false;
        playerUnit.UnitEffectUpdate();
        playerHUD.ChangeHP(playerUnit.currentHP);
        playerHUD.ChangeMP(playerUnit.currentMP);
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

    private IEnumerator PlayerDefend()
    {
        initArmorModifier = playerUnit.armorModifier;
        playerUnit.armorModifier *= 0.4f;
        combatState = CombatState.ENEMY_TURN;
        yield return new WaitForSeconds(1f);
        combatUI.combatDialogue.text = "Игрок успешно перешел в защиту";
        yield return new WaitForSeconds(1f);
        StartCoroutine(EnemyTurn());
    }

    public IEnumerator PlayerUsingItem()
    {
        combatUI.combatDialogue.text = "Игрок использует предмет";
        playerHUD.ChangeHP(playerUnit.currentHP);
        playerHUD.ChangeMP(playerUnit.currentMP);
        yield return new WaitForSeconds(1f);
        StartCoroutine(EnemyTurn());
    }

    private IEnumerator PlayerAttack(int damageTypeID, bool isMental)
    {
        playerAttackButtonWasPressed = true;
        switch (damageTypeID)
        {
            case 0:
                SoundManager.PlaySound(SoundManager.Sound.WeaponSwingWithHit);
                break;
            case 1:
                SoundManager.PlaySound(SoundManager.Sound.PsiSkill);
                break;
            case 2:
                SoundManager.PlaySound(SoundManager.Sound.ElectraSkill);
                break;
            case 3:
                SoundManager.PlaySound(SoundManager.Sound.FiraSkill);
                break;
        }
        string message;
        if (reflectionProbability2 > 0 && random.NextDouble() < reflectionProbability2)
        {            
            message = ReflectAction(playerUnit, damageTypeID - 1, -CalcAffinityDamage(damageTypeID, isMental, playerUnit, playerUnit));
            playerIsHurting = true;
        }
        else
        {
            int totalDamage = CalcAffinityDamage(damageTypeID, isMental, playerUnit, enemyUnit);
            enemyIsHurting = true;
            enemyUnit.TakeDamage(totalDamage);
            switch (damageTypeID)
            {
                case 0:
                    message = playerUnit.unitName + " наносит " + totalDamage + " физического урона";
                    break;
                case 1:
                    enemyUnit.PsionaEffect();
                    message = playerUnit.unitName + " наносит " + totalDamage + " псионического урона";
                    break;
                case 2:
                    enemyUnit.ElectraEffect();
                    message = playerUnit.unitName + " наносит " + totalDamage + " электрического урона";
                    break;
                case 3:
                    enemyUnit.FiraEffect();
                    message = playerUnit.unitName + " наносит " + totalDamage + " огненного урона";
                    break;
                default:
                    message = string.Empty;
                    break;
            }
        }
        if (isMental)
        {
            playerUnit.ReduceCurrentMP(mentalSkillsMPCost[0]);
            playerHUD.ChangeMP(playerUnit.currentMP);
        }
        combatState = CombatState.ENEMY_TURN;
        yield return new WaitForSeconds(1f);
        playerIsHurting = false;
        enemyIsHurting = false;
        playerAttackButtonWasPressed = false;
        playerHUD.ChangeHP(playerUnit.currentHP);
        enemyHUD.ChangeHP(enemyUnit.currentHP);
        combatUI.combatDialogue.text = message;
        yield return new WaitForSeconds(1.5f);
        if (playerUnit.IsDead())
        {
            combatState = CombatState.LOST;
            FinishBattle();
        }
        else if (enemyUnit.IsDead())
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

    private IEnumerator PlayerRegenaSkill()
    {
        playerAttackButtonWasPressed = true;
        string message;
        if (reflectionProbability2 > 0 && random.NextDouble() < reflectionProbability2)
            message = ReflectAction(enemyUnit, -1, (int)(enemyUnit.maxHP * 0.25));
        else
        {
            playerUnit.Heal((int)(playerUnit.maxHP * 0.25));
            message = playerUnit.unitName + " излечивает " + (int)(playerUnit.maxHP * 0.25) + " здоровья";
        }
        playerUnit.ReduceCurrentMP(mentalSkillsMPCost[0]);
        playerHUD.ChangeHP(playerUnit.currentHP);
        playerHUD.ChangeMP(playerUnit.currentMP);
        combatState = CombatState.ENEMY_TURN;
        yield return new WaitForSeconds(1f);
        playerAttackButtonWasPressed = false;
        enemyHUD.ChangeHP(enemyUnit.currentHP);
        combatUI.combatDialogue.text = message;
        yield return new WaitForSeconds(1.5f);
        if (enemyUnit.isKnockedDown && enemyUnit.knockedTurnsCount == 0)
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
        enemyHUD.ChangeHP(enemyUnit.currentHP);
        enemyHUD.ChangeMP(enemyUnit.currentMP);
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
        enemyAI.CombatAI(out string effectMessage);
        enemyAttackButtonWasPressed = true;
        yield return new WaitForSeconds(0.5f);
        playerIsHurting = false;
        enemyIsHurting = false;
        enemyAttackButtonWasPressed = false;
        if (!string.IsNullOrEmpty(effectMessage))
        {
            yield return new WaitForSeconds(1f);
            combatUI.combatDialogue.text = effectMessage;
        }
        playerHUD.ChangeHP(playerUnit.currentHP);
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
            combatUI.combatDialogue.text = "Игрок сбит с ног. Снова ход врага!";
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

    //-----------------------------(Методы для кнопок)-------------------------------------------------

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
        if (combatUI.skillButtonsWereinstantiated && combatUI.areButtonsShown)
            combatUI.HideOrShowMentalSkillButtons();
        combatUI.combatDialogue.text = "Игрок начинает атаку";
        StartCoroutine(PlayerAttack(0, false));
    }

    public void OnDefendButton()
    {
        if (combatState != CombatState.PLAYER_TURN)
            return;
        if (Inventory.instance.isOpened)
            Inventory.instance.Close();
        if (combatUI.skillButtonsWereinstantiated && combatUI.areButtonsShown)
            combatUI.HideOrShowMentalSkillButtons();
        combatUI.combatDialogue.text = "Игрок защищается";
        StartCoroutine(PlayerDefend());
    }

    public void OnMentalButton()
    {
        if (combatState != CombatState.PLAYER_TURN)
            return;
        if (Inventory.instance.isOpened)
            Inventory.instance.Close();
        if (combatUI.skillButtonsWereinstantiated)
            combatUI.HideOrShowMentalSkillButtons();
        else
        {
            combatUI.SetMentalSkillButtons();
            combatUI.skillButtonsWereinstantiated = true;
        }
    }

    public void OnPsionaButton()
    {
        if (playerUnit.currentMP >= mentalSkillsMPCost[0])
        {
            combatUI.combatDialogue.text = "Игрок использует пси навык";
            combatUI.HideOrShowMentalSkillButtons();
            combatUI.areButtonsShown = false;
            StartCoroutine(PlayerAttack(1, true));
        }
        else
        {
            combatUI.combatDialogue.text = "Недостаточно MP для использования навыка";
            combatUI.HideOrShowMentalSkillButtons();
            combatUI.areButtonsShown = false;
        }
    }

    public void OnElectraButton()
    {
        if (playerUnit.currentMP >= mentalSkillsMPCost[0])
        {
            combatUI.combatDialogue.text = "Игрок использует электрический навык";
            combatUI.HideOrShowMentalSkillButtons();
            combatUI.areButtonsShown = false;
            StartCoroutine(PlayerAttack(2, true));
        }
        else
        {
            combatUI.combatDialogue.text = "Недостаточно MP для использования навыка";
            combatUI.HideOrShowMentalSkillButtons();
            combatUI.areButtonsShown = false;
        }
    }

    public void OnFiraButton()
    {
        if (playerUnit.currentMP >= mentalSkillsMPCost[0])
        {
            combatUI.combatDialogue.text = "Игрок использует огненный навык";
            combatUI.HideOrShowMentalSkillButtons();
            combatUI.areButtonsShown = false;
            StartCoroutine(PlayerAttack(3, true));
        }
        else
        {
            combatUI.combatDialogue.text = "Недостаточно MP для использования навыка";
            combatUI.HideOrShowMentalSkillButtons();
            combatUI.areButtonsShown = false;
        }
    }

    public void OnRegenaButton()
    {
        if (playerUnit.currentMP >= mentalSkillsMPCost[0])
        {
            combatUI.combatDialogue.text = "Игрок использует целительный навык";
            combatUI.HideOrShowMentalSkillButtons();
            combatUI.areButtonsShown = false;
            StartCoroutine(PlayerRegenaSkill());
        }
        else
        {
            combatUI.combatDialogue.text = "Недостаточно MP для использования навыка";
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
            GameUI.instance.gameObject.SetActive(true);
            mainCamera.enabled = true;
            combatCamera.enabled = false;
            combatUI.gameObject.SetActive(false);
            Destroy(playerCombatPosition.GetChild(0).gameObject);
            Destroy(enemyCombatPosition.GetChild(0).gameObject);
            Destroy(encounteredEnemy.gameObject);
        }
        else if (combatState == CombatState.LOST)
        {
            combatUI.combatDialogue.text = "Игрок оказался повержен...";
            GameUI.instance.gameObject.SetActive(true);
            for (int i = 0; i < GameUI.instance.transform.childCount; i++)
                GameUI.instance.transform.GetChild(i).gameObject.SetActive(false);
            Inventory.instance.attachedUnit.Death();
        }
    }

    public string ReflectAction(Unit receiver, int effectIndex, int totalDamage)
    {
        string message;
        if (totalDamage < 0)
        {
            receiver.TakeDamage(-totalDamage);
            message = receiver.unitName + " атакует, но атака оказывается отражена обратно, " + receiver.unitName + " получает " + -totalDamage + " урона";
        }
        else
        {
            receiver.Heal(totalDamage);
            message = "Эффект оказывается отражен в " + receiver.unitName + ", что восстанавливает ему " + totalDamage + " здоровья";
        }
        switch (effectIndex)
        {
            case 0:
                receiver.PsionaEffect();
                break;
            case 1:
                receiver.ElectraEffect();
                break;
            case 2:
                receiver.FiraEffect();
                break;
            default:
                break;
        }
        return message;
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
