using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor.VersionControl;

public enum CombatState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST }

public class CombatSystem : MonoBehaviour
{
    private const float weaknessMult = 1.25f;
    private const float resistMult = 0.5f;
    private const float knockedDownMult = 1.2f;
    public const float effectProbability = 1f;
    
    [HideInInspector] public float reflectionProbability1;
    [HideInInspector] public float reflectionProbability2;

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

        StartCoroutine(PlayerTurn());
    }


    //-----------------------------(Player's actions)-------------------------------------------------

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
            playerUnit.knockedDownTimeout = 3;;
            combatUI.combatDialogue.text = playerUnit.unitName + " встал на ноги. Так продолжайте же сражаться!";
            yield return new WaitForSeconds(1.5f);
        }
        if (playerUnit.UnitEffectUpdate())
        {
            playerHUD.ChangeHP(playerUnit.currentHP);
            playerHUD.ChangeMP(playerUnit.currentMP);
            yield return new WaitForSeconds(1.5f);
        }
        if (playerUnit.underItemEffect)
        {
            if (playerUnit.affectingItem.doesHaveContinuousEffect)
            {
                playerUnit.affectingItem.ApplyEffect();
                yield return new WaitForSeconds(1f);
            }
            playerUnit.affectingItem.RemoveEffect();
            if (!playerUnit.underItemEffect)
            {
                combatUI.combatDialogue.text = "Эффект от предмета, наложенный на " + playerUnit.unitName + ", прошел";
                yield return new WaitForSeconds(1f);
            }
            else
                playerUnit.itemEffectTurnsCount++;
        }
        if (playerUnit.appliedEffect[1])
        {
            combatState = CombatState.ENEMY_TURN;
            StartCoroutine(EnemyTurn());
        }
        else if (playerUnit.IsDead())
        {
            combatState = CombatState.LOST;
            FinishBattle();
        }
        else
        {
            combatState = CombatState.PLAYER_TURN;
            combatUI.combatDialogue.text = "Выберите действие:";
        }
    }

    private IEnumerator PlayerDefend()
    {
        combatState = CombatState.ENEMY_TURN;
        initArmorModifier = playerUnit.armorModifier;
        playerUnit.armorModifier *= 0.4f;
        yield return new WaitForSeconds(1.5f);
        combatUI.combatDialogue.text = playerUnit.unitName + " успешно перешел в защиту";
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(EnemyTurn());
    }

    public IEnumerator PlayerUsingItem()
    {
        combatState = CombatState.ENEMY_TURN;
        playerHUD.ChangeHP(playerUnit.currentHP);
        playerHUD.ChangeMP(playerUnit.currentMP);
        enemyHUD.ChangeHP(enemyUnit.currentHP);
        enemyHUD.ChangeMP(enemyUnit.currentMP);
        yield return new WaitForSeconds(1.5f);
        if (enemyUnit.IsDead())
        {
            combatState = CombatState.WON;
            combatUI.combatDialogue.text = playerUnit.unitName + " одержал победу!";
            yield return new WaitForSeconds(1.5f);
            FinishBattle();
        }
        else if (enemyUnit.isKnockedDown && enemyUnit.knockedTurnsCount == 0)
        {
            enemyUnit.knockedTurnsCount++;
            combatUI.combatDialogue.text = "Враг сбит с ног. " + playerUnit.unitName + " предоставляется еще один ход!";
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(PlayerTurn());
        }
        else
            StartCoroutine(EnemyTurn());
    }

    private IEnumerator PlayerAttack(int damageTypeID, bool isMental)
    {
        combatState = CombatState.ENEMY_TURN;
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
        string effectMessage;
        if (reflectionProbability2 > 0 && random.NextDouble() < reflectionProbability2)
        {
            yield return new WaitForSeconds(0.5f);
            message = ReflectAction(playerUnit, damageTypeID - 1, -CalcAffinityDamage(damageTypeID, isMental, playerUnit, playerUnit), out effectMessage);
            playerIsHurting = true;
            yield return new WaitForSeconds(1f);
        }
        else
        {
            int totalDamage = CalcAffinityDamage(damageTypeID, isMental, playerUnit, enemyUnit);
            enemyIsHurting = true;
            yield return new WaitForSeconds(1.5f);
            enemyUnit.TakeDamage(totalDamage);
            effectMessage = enemyUnit.ApplyEffect(damageTypeID - 1);
            message = damageTypeID switch
            {
                0 => playerUnit.unitName + " наносит " + totalDamage + " физического урона",
                1 => playerUnit.unitName + " наносит " + totalDamage + " псионического урона",
                2 => playerUnit.unitName + " наносит " + totalDamage + " электрического урона",
                3 => playerUnit.unitName + " наносит " + totalDamage + " огненного урона",
                _ => string.Empty,
            };
        }
        if (isMental)
        {
            playerUnit.ReduceCurrentMP(mentalSkillsMPCost[0]);
            playerHUD.ChangeMP(playerUnit.currentMP);
        }
        if (!string.IsNullOrEmpty(effectMessage))
        {
            playerIsHurting = false;
            enemyIsHurting = false;
            combatUI.combatDialogue.text = effectMessage;
            yield return new WaitForSeconds(1.5f);
        }
        playerAttackButtonWasPressed = false;
        playerHUD.ChangeHP(playerUnit.currentHP);
        enemyHUD.ChangeHP(enemyUnit.currentHP);
        playerIsHurting = false;
        enemyIsHurting = false;
        combatUI.combatDialogue.text = message;
        yield return new WaitForSeconds(1.5f);
        if (playerUnit.IsDead())
        {
            combatState = CombatState.LOST;
            FinishBattle();
        }
        else if (playerUnit.isKnockedDown && playerUnit.knockedTurnsCount == 0)
        {
            combatUI.combatDialogue.text = playerUnit.unitName + " сбит с ног собственной атакой. Неловко вышло...";
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(EnemyTurn());
        }
        else if (enemyUnit.IsDead())
        {
            combatState = CombatState.WON;
            combatUI.combatDialogue.text = playerUnit.unitName + " одержал победу!";
            yield return new WaitForSeconds(1.5f);
            FinishBattle();
        }
        else if (enemyUnit.isKnockedDown && enemyUnit.knockedTurnsCount == 0)
        {
            enemyUnit.knockedTurnsCount++;
            combatUI.combatDialogue.text = "Враг сбит с ног. " + playerUnit.unitName + " предоставляется еще один ход!";
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(PlayerTurn());
        }
        else
            StartCoroutine(EnemyTurn());
    }

    private IEnumerator PlayerRegenaSkill()
    {
        combatState = CombatState.ENEMY_TURN;
        playerAttackButtonWasPressed = true;
        yield return new WaitForSeconds(1.5f);
        string message;
        if (reflectionProbability2 > 0 && random.NextDouble() < reflectionProbability2)
            message = ReflectAction(enemyUnit, -1, (int)(enemyUnit.maxHP * 0.25), out _);
        else
        {
            playerUnit.Heal((int)(playerUnit.maxHP * 0.25));
            message = playerUnit.unitName + " восстанавливает " + (int)(playerUnit.maxHP * 0.25) + " здоровья";
        }
        playerUnit.ReduceCurrentMP(mentalSkillsMPCost[0]);
        playerHUD.ChangeHP(playerUnit.currentHP);
        playerHUD.ChangeMP(playerUnit.currentMP);
        enemyHUD.ChangeHP(enemyUnit.currentHP);
        playerAttackButtonWasPressed = false;
        combatUI.combatDialogue.text = message;
        yield return new WaitForSeconds(1.5f);
        if (enemyUnit.isKnockedDown && enemyUnit.knockedTurnsCount == 0)
        {
            enemyUnit.knockedTurnsCount++;
            combatUI.combatDialogue.text = "Враг сбит с ног. " + playerUnit.unitName + " предоставляется еще один ход!";
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(PlayerTurn());
        }
        else
            StartCoroutine(EnemyTurn());
    }

    //-----------------------------(Enemy's actions)-------------------------------------------------

    public IEnumerator EnemyTurn()
    {
        if (enemyUnit.UnitEffectUpdate())
        {
            enemyHUD.ChangeHP(enemyUnit.currentHP);
            enemyHUD.ChangeMP(enemyUnit.currentMP);
            yield return new WaitForSeconds(1.5f);
        }
        if (enemyUnit.underItemEffect)
        {
            if (enemyUnit.affectingItem.doesHaveContinuousEffect)
            {
                enemyUnit.affectingItem.ApplyEffect();
                yield return new WaitForSeconds(1f);
            }
            enemyUnit.affectingItem.RemoveEffect();
            if (!enemyUnit.underItemEffect)
            {
                combatUI.combatDialogue.text = "Эффект от предмета, наложенный на " + enemyUnit.unitName + ", прошел";
                yield return new WaitForSeconds(1f);
            }
            else
                enemyUnit.itemEffectTurnsCount++;
        }
        if (enemyUnit.appliedEffect[1])
        {
            StartCoroutine(PlayerTurn());
            yield break;
        }
        if (enemyUnit.IsDead())
        {
            combatState = CombatState.WON;
            combatUI.combatDialogue.text = playerUnit.unitName + " одержал победу!";
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
            combatUI.combatDialogue.text = enemyUnit.unitName + " встал на ноги";
            yield return new WaitForSeconds(1.5f);
        }
        enemyAttackButtonWasPressed = true;
        List<string> messages = enemyAI.CombatAI(out int soundID);
        for (int i = 0; i < messages.Count; i++)
        {
            if (enemyAI.enemyID == 0 && i == messages.Count - 1 && soundID != -1)
                SoundManager.PlaySound((SoundManager.Sound)soundID);
            if (!string.IsNullOrEmpty(messages[i]))
            {
                combatUI.combatDialogue.text = messages[i];
                yield return new WaitForSeconds(1.5f);
                playerIsHurting = false;
                enemyIsHurting = false;
            }
        }
        enemyAttackButtonWasPressed = false;
        playerHUD.ChangeHP(playerUnit.currentHP);
        enemyHUD.ChangeHP(enemyUnit.currentHP);
        if (enemyUnit.IsDead())
        {
            combatState = CombatState.WON;
            combatUI.combatDialogue.text = playerUnit.unitName + " одержал победу!";
            yield return new WaitForSeconds(1.5f);
            FinishBattle();
        }
        else if (enemyUnit.isKnockedDown && enemyUnit.knockedTurnsCount == 0)
        {
            combatUI.combatDialogue.text = enemyUnit.unitName + " сбит с ног собственной атакой. Какая неудача!";
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(PlayerTurn());
        }
        else if (playerUnit.IsDead())
        {
            combatState = CombatState.LOST;
            FinishBattle();
        }
        else if (playerUnit.isKnockedDown && playerUnit.knockedTurnsCount == 0)
        {            
            playerUnit.knockedTurnsCount++;
            combatUI.combatDialogue.text = playerUnit.unitName + " сбит с ног. Снова ход врага!";
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(EnemyTurn());
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(PlayerTurn());
        }
    }

    //-----------------------------(Methods for buttons)-------------------------------------------------

    public void OnItemButton()
    {
        if (combatState != CombatState.PLAYER_TURN)
            return;
        if (Inventory.instance.isOpen)
            Inventory.instance.Close();
        else
            Inventory.instance.Open();
        if (combatUI.skillButtonsWereInstantiated && combatUI.areButtonsShown)
            combatUI.HideOrShowMentalSkillButtons();
    }

    public void OnAttackButton()
    {
        if (combatState != CombatState.PLAYER_TURN)
            return;
        if (Inventory.instance.isOpen)
            Inventory.instance.Close();
        if (combatUI.skillButtonsWereInstantiated && combatUI.areButtonsShown)
            combatUI.HideOrShowMentalSkillButtons();
        combatUI.combatDialogue.text = playerUnit.unitName + " начинает атаку";
        StartCoroutine(PlayerAttack(0, false));
    }

    public void OnDefendButton()
    {
        if (combatState != CombatState.PLAYER_TURN)
            return;
        if (Inventory.instance.isOpen)
            Inventory.instance.Close();
        if (combatUI.skillButtonsWereInstantiated && combatUI.areButtonsShown)
            combatUI.HideOrShowMentalSkillButtons();
        combatUI.combatDialogue.text = playerUnit.unitName + " защищается";
        StartCoroutine(PlayerDefend());
    }

    public void OnMentalButton()
    {
        if (combatState != CombatState.PLAYER_TURN)
            return;
        if (Inventory.instance.isOpen)
            Inventory.instance.Close();
        if (combatUI.skillButtonsWereInstantiated)
            combatUI.HideOrShowMentalSkillButtons();
        else
        {
            combatUI.SetMentalSkillButtons();
            combatUI.skillButtonsWereInstantiated = true;
        }
    }

    public void OnPsionaButton()
    {
        if (playerUnit.currentMP >= mentalSkillsMPCost[0])
        {
            combatUI.combatDialogue.text = playerUnit.unitName + " использует пси навык";
            combatUI.HideOrShowMentalSkillButtons();
            StartCoroutine(PlayerAttack(1, true));
        }
        else
        {
            combatUI.combatDialogue.text = "Недостаточно MP для использования навыка";
            combatUI.HideOrShowMentalSkillButtons();
        }
    }

    public void OnElectraButton()
    {
        if (playerUnit.currentMP >= mentalSkillsMPCost[0])
        {
            combatUI.combatDialogue.text = playerUnit.unitName + " использует электрический навык";
            combatUI.HideOrShowMentalSkillButtons();
            StartCoroutine(PlayerAttack(2, true));
        }
        else
        {
            combatUI.combatDialogue.text = "Недостаточно MP для использования навыка";
            combatUI.HideOrShowMentalSkillButtons();
        }
    }

    public void OnFiraButton()
    {
        if (playerUnit.currentMP >= mentalSkillsMPCost[0])
        {
            combatUI.combatDialogue.text = playerUnit.unitName + " использует огненный навык";
            combatUI.HideOrShowMentalSkillButtons();
            StartCoroutine(PlayerAttack(3, true));
        }
        else
        {
            combatUI.combatDialogue.text = "Недостаточно MP для использования навыка";
            combatUI.HideOrShowMentalSkillButtons();
        }
    }

    public void OnRegenaButton()
    {
        if (playerUnit.currentMP >= mentalSkillsMPCost[0])
        {
            combatUI.combatDialogue.text = playerUnit.unitName + " использует целительный навык";
            combatUI.HideOrShowMentalSkillButtons();
            StartCoroutine(PlayerRegenaSkill());
        }
        else
        {
            combatUI.combatDialogue.text = "Недостаточно MP для использования навыка";
            combatUI.HideOrShowMentalSkillButtons();
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
            GameUI.instance.gameObject.SetActive(true);
            for (int i = 0; i < GameUI.instance.transform.childCount; i++)
                GameUI.instance.transform.GetChild(i).gameObject.SetActive(false);
            Inventory.instance.attachedUnit.Death();
        }
    }

    public string ReflectAction(Unit receiver, int effectIndex, int totalDamage, out string effectMessage)
    {
        string message;
        if (totalDamage < 0)
        {
            receiver.TakeDamage(-totalDamage);
            message = "Атака оказывается отражена в нападающего, " + receiver.unitName + " получает " + -totalDamage + " урона";
        }
        else
        {
            receiver.Heal(totalDamage);
            message = "Навык оказывается отражен в " + receiver.unitName + ", что восстанавливает ему " + totalDamage + " здоровья";
        }
        effectMessage = enemyUnit.ApplyEffect(effectIndex);
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
