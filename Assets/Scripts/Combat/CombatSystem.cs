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

    public float reflectionProbability1;
    public float reflectionProbability2;

    [HideInInspector] public Player playerUnit;
    [HideInInspector] public int curEnemyID;
    [HideInInspector] public List<Enemy> enemyUnits = new();
    [HideInInspector] public List<EnemyCombatController> enemyCombatControllers = new();
    private List<EnemyAI> enemyAIs = new();

    [SerializeField] GameObject playerPrefab;
    [HideInInspector] public Enemy encounteredEnemy;

    [SerializeField] Transform playerCombatPosition;
    [SerializeField] Transform[] possibleCombatPositions = new Transform[3];
    //private Transform[] enemyCombatPositions = new Transform[4];

    [SerializeField] CombatHUD playerHUD;
    public CombatHUD[] enemyHUDs;

    [SerializeField] CombatState combatState;
    public CombatUI combatUI;

    [SerializeField] Camera mainCamera;
    [SerializeField] Camera combatCamera;

    private float initArmorModifier = 1;
    public int[] mentalSkillsMPCost;
    [HideInInspector] public bool isInCombat;
    [HideInInspector] public bool isChoosingEnemy;
    [HideInInspector] public bool wasAnItemUsed;
    public static CombatSystem instance;
    private System.Random random = new System.Random();

    [HideInInspector] public bool playerAttackButtonWasPressed;
    [HideInInspector] public bool playerIsHurting;

    [HideInInspector] public int damageTypeID;
    [HideInInspector] public bool isMental;

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
        playerUnit = playerCombat.GetComponent<Player>();
        playerUnit.CopyStats(Inventory.instance.attachedUnit);
        playerUnit.knockedTurnsCount = 0;
        playerUnit.knockedDownTimeout = 0;
        for (int i = 0; i < encounteredEnemy.enemyPrefabsForCombat.Length; i++)
        {
            GameObject enemyCombat = Instantiate(encounteredEnemy.enemyPrefabsForCombat[i], possibleCombatPositions[i]);
            enemyUnits.Add(enemyCombat.GetComponent<Enemy>());
            enemyAIs.Add(enemyCombat.GetComponent<EnemyAI>());
            enemyCombatControllers.Add(enemyCombat.GetComponent<EnemyCombatController>());
            enemyUnits[i].knockedTurnsCount = 0;
            enemyUnits[i].knockedDownTimeout = 0;
            enemyHUDs[i].gameObject.SetActive(true);
            enemyHUDs[i].SetHUD(enemyUnits[i]);
            enemyUnits[i].combatHUD = enemyHUDs[i];
        }
        curEnemyID = 0;
        GameUI.instance.gameObject.SetActive(false);
        combatUI.gameObject.SetActive(true);
        playerHUD.SetHUD(playerUnit);
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
            combatUI.combatDialogue.text = playerUnit.unitName + " встал на ноги. Так продолжайте же сражаться!";
            yield return new WaitForSeconds(1.5f);
        }
        wasAnItemUsed = false;
        if (playerUnit.UnitEffectUpdate())
        {
            playerHUD.ChangeHP(playerUnit.currentHP);
            playerHUD.ChangeMP(playerUnit.currentMP);
            yield return new WaitForSeconds(1.5f);
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
            combatUI.combatDialogue.text = "Выберите действие";
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
        combatUI.combatDialogue.text = playerUnit.unitName + " использует предмет";
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(EnemyTurn());
    }

    public IEnumerator PlayerAttack(int damageTypeID, bool isMental)
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
            int totalDamage = CalcAffinityDamage(damageTypeID, isMental, playerUnit, enemyUnits[curEnemyID]);
            enemyCombatControllers[curEnemyID].isHurting = true;
            yield return new WaitForSeconds(0.5f);
            enemyUnits[curEnemyID].TakeDamage(totalDamage);
            if (enemyUnits[curEnemyID].IsDead())
            {
                enemyCombatControllers[curEnemyID].isHurting = false;
                enemyCombatControllers[curEnemyID].isDying = true;
            }
            yield return new WaitForSeconds(1f);
            effectMessage = enemyUnits[curEnemyID].ApplyEffect(damageTypeID - 1);
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
            enemyCombatControllers[curEnemyID].isHurting = false;
            combatUI.combatDialogue.text = effectMessage;
            yield return new WaitForSeconds(1.5f);
        }
        playerAttackButtonWasPressed = false;
        playerHUD.ChangeHP(playerUnit.currentHP);
        enemyHUDs[curEnemyID].ChangeHP(enemyUnits[curEnemyID].currentHP);
        playerIsHurting = false;
        enemyCombatControllers[curEnemyID].isHurting = false;
        combatUI.combatDialogue.text = message;
        yield return new WaitForSeconds(1.5f);
        if (playerUnit.IsDead())
        {
            combatState = CombatState.LOST;
            FinishBattle();
        }
        else if (playerUnit.isKnockedDown && playerUnit.knockedTurnsCount == 0)
        {
            combatUI.combatDialogue.text = playerUnit.unitName + " сбил себя с ног своей же атакой. Неловко вышло...";
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(EnemyTurn());
        }
        else if (!enemyUnits[curEnemyID].IsDead() && enemyUnits[curEnemyID].isKnockedDown && enemyUnits[curEnemyID].knockedTurnsCount == 0)
        {
            enemyUnits[curEnemyID].knockedTurnsCount++;
            combatUI.combatDialogue.text = "Враг сбит с ног. " + playerUnit.unitName + " предоставляется еще один ход!";
            yield return new WaitForSeconds(1.5f);
            combatState = CombatState.PLAYER_TURN;
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
        {
            curEnemyID = random.Next(0, enemyUnits.Count);
            message = ReflectAction(enemyUnits[curEnemyID], -1, (int)(enemyUnits[curEnemyID].maxHP * 0.25), out _);
        }
        else
        {
            playerUnit.Heal((int)(playerUnit.maxHP * 0.25));
            message = playerUnit.unitName + " излечивает " + (int)(playerUnit.maxHP * 0.25) + " здоровья";
        }
        playerUnit.ReduceCurrentMP(mentalSkillsMPCost[0]);
        playerHUD.ChangeHP(playerUnit.currentHP);
        playerHUD.ChangeMP(playerUnit.currentMP);
        enemyUnits[curEnemyID].combatHUD.ChangeHP(enemyUnits[curEnemyID].currentHP);
        playerAttackButtonWasPressed = false;
        combatUI.combatDialogue.text = message;
        yield return new WaitForSeconds(1.5f);
        if (enemyUnits[curEnemyID].isKnockedDown && enemyUnits[curEnemyID].knockedTurnsCount == 0)
        {
            enemyUnits[curEnemyID].knockedTurnsCount++;
            combatUI.combatDialogue.text = "Враг сбит с ног. " + playerUnit.unitName + " предоставляется еще один ход!";
            yield return new WaitForSeconds(1.5f);
            combatState = CombatState.PLAYER_TURN;
            StartCoroutine(PlayerTurn());
        }
        else
            StartCoroutine(EnemyTurn());
    }

    //-----------------------------(Действия врага)-------------------------------------------------

    public IEnumerator EnemyTurn()
    {
        if (enemyUnits[curEnemyID].IsDead())
        {
            RemoveEnemy(curEnemyID);
            yield return new WaitForSeconds(1.5f);
        }
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            curEnemyID = i;
            if (enemyUnits[i].UnitEffectUpdate())
            {
                enemyUnits[i].combatHUD.ChangeHP(enemyUnits[i].currentHP);
                enemyUnits[i].combatHUD.ChangeMP(enemyUnits[i].currentMP);
                yield return new WaitForSeconds(1.5f);
            }
            if (enemyUnits[i].appliedEffect[1])
                continue;
            else if (enemyUnits[i].IsDead())
            {
                RemoveEnemy(i);
                i--;
                yield return new WaitForSeconds(1.5f);
                continue;
            }
            if (enemyUnits[i].isKnockedDown && enemyUnits[i].knockedTurnsCount == 0)
            {
                enemyUnits[i].knockedTurnsCount++;
                continue;
            }
            if (enemyUnits[i].knockedDownTimeout > 0)
                enemyUnits[i].knockedDownTimeout--;
            if (enemyUnits[i].isKnockedDown)
            {
                enemyUnits[i].isKnockedDown = false;
                enemyUnits[i].knockedTurnsCount = 0;
                enemyUnits[i].knockedDownTimeout = 3;
                combatUI.combatDialogue.text = enemyUnits[i].unitName + " встал на ноги";
                yield return new WaitForSeconds(1.5f);
            }
            enemyCombatControllers[i].attackButtonWasPressed = true;
            List<string> messages = enemyAIs[i].CombatAI(out int soundID);
            for (int j = 0; j < messages.Count; j++)
            {
                if (enemyAIs[i].enemyID == 0 && j == messages.Count - 1 && soundID != -1)
                    SoundManager.PlaySound((SoundManager.Sound)soundID);
                if (!string.IsNullOrEmpty(messages[j]))
                {
                    combatUI.combatDialogue.text = messages[j];
                    yield return new WaitForSeconds(1.5f);
                    playerIsHurting = false;
                    enemyCombatControllers[i].isHurting = false;
                }
            }
            enemyCombatControllers[i].attackButtonWasPressed = false;
            playerHUD.ChangeHP(playerUnit.currentHP);
            enemyUnits[i].combatHUD.ChangeHP(enemyUnits[i].currentHP);
            if (enemyUnits[i].IsDead())
            {               
                RemoveEnemy(i);
                i--;
                yield return new WaitForSeconds(1.5f);
            }
            else if (enemyUnits[i].isKnockedDown)
            {
                combatUI.combatDialogue.text = enemyUnits[i].unitName + " сбил себя с ног своей же атакой. Какая неудача!";
                yield return new WaitForSeconds(1.5f);
            }
            else if (playerUnit.IsDead())
            {
                combatState = CombatState.LOST;
                FinishBattle();
                yield break;
            }
        }
        if (enemyUnits.Count == 0)
        {
            combatState = CombatState.WON;
            combatUI.combatDialogue.text = playerUnit.unitName + " одержал победу!";
            yield return new WaitForSeconds(1.5f);
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
            combatState = CombatState.PLAYER_TURN;
            StartCoroutine(PlayerTurn());
        }
    }

    private void RemoveEnemy(int enemyID)
    {
        combatUI.combatDialogue.text = enemyUnits[enemyID].unitName + " повержен";
        enemyUnits[enemyID].combatHUD.gameObject.SetActive(false);
        Destroy(enemyUnits[enemyID].gameObject);
        enemyCombatControllers.RemoveAt(enemyID);
        enemyUnits.RemoveAt(enemyID);
        enemyAIs.RemoveAt(enemyID);
    }

    //-----------------------------(Методы для кнопок)-------------------------------------------------

    public void OnItemButton()
    {
        if (combatState != CombatState.PLAYER_TURN || wasAnItemUsed)
            return;
        isChoosingEnemy = false;
        if (Inventory.instance.isOpened)
        {
            Inventory.instance.Close();
            combatUI.combatDialogue.text = "Выберите действие";
        }
        else
        {
            Inventory.instance.Open();
            combatUI.combatDialogue.text = "Выберите предмет, который хотите использовать";
        }
        if (combatUI.skillButtonsWereInstantiated && combatUI.areButtonsShown)
            combatUI.HideOrShowMentalSkillButtons();
    }

    public void OnAttackButton()
    {
        if (combatState != CombatState.PLAYER_TURN)
            return;
        if (Inventory.instance.isOpened)
            Inventory.instance.Close();
        if (combatUI.skillButtonsWereInstantiated && combatUI.areButtonsShown)
            combatUI.HideOrShowMentalSkillButtons();
        isChoosingEnemy = true;
        isMental = false;
        damageTypeID = 0;
        combatUI.combatDialogue.text = "Выберите цель для атаки";
    }

    public void OnDefendButton()
    {
        if (combatState != CombatState.PLAYER_TURN)
            return;
        isChoosingEnemy = false;
        if (Inventory.instance.isOpened)
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
        if (Inventory.instance.isOpened)
            Inventory.instance.Close();
        if (combatUI.skillButtonsWereInstantiated)
            combatUI.HideOrShowMentalSkillButtons();
        else
        {
            combatUI.SetMentalSkillButtons();
            combatUI.skillButtonsWereInstantiated = true;
        }
        if (combatUI.areButtonsShown)
            combatUI.combatDialogue.text = "Выберите навык, который хотите применить";
        else
            combatUI.combatDialogue.text = "Выберите действие";
    }

    public void OnPsionaButton()
    {
        if (playerUnit.currentMP >= mentalSkillsMPCost[0])
        {
            combatUI.HideOrShowMentalSkillButtons();
            isChoosingEnemy = true;
            isMental = true;
            damageTypeID = 1;
            combatUI.combatDialogue.text = "Выберите цель для атаки";
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
            combatUI.HideOrShowMentalSkillButtons();
            isChoosingEnemy = true;
            isMental = true;
            damageTypeID = 2;
            combatUI.combatDialogue.text = "Выберите цель для атаки";
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
            combatUI.HideOrShowMentalSkillButtons();
            isChoosingEnemy = true;
            isMental = true;
            damageTypeID = 3;
            combatUI.combatDialogue.text = "Выберите цель для атаки";
        }
        else
        {
            combatUI.combatDialogue.text = "Недостаточно MP для использования навыка";
            combatUI.HideOrShowMentalSkillButtons();
        }
    }

    public void OnRegenaButton()
    {
        isChoosingEnemy = false;
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

    private void FinishBattle()
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
            message = "Эффект оказывается отражен в " + receiver.unitName + ", что восстанавливает ему " + totalDamage + " здоровья";
        }
        effectMessage = receiver.ApplyEffect(effectIndex);
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
