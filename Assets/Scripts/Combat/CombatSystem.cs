using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum CombatState { START, ALLY_TURN, ENEMY_TURN, WON, LOST }

public class CombatSystem : MonoBehaviour
{
    private const float weaknessMult = 1.25f;
    private const float resistMult = 0.5f;
    private const float knockedDownMult = 1.2f;
    public const float effectProbability = 0.2f;

    public float reflectionProbability1;
    public float reflectionProbability2;

    [HideInInspector] public Player playerUnit;
    [HideInInspector] public int curEnemyID;
    [HideInInspector] public List<Enemy> enemyUnits = new();
    [HideInInspector] public int curAllyID;
    [HideInInspector] public List<Unit> allyUnits = new();
    [HideInInspector] public List<EnemyCombatController> enemyCombatControllers = new();
    //[HideInInspector] public List<PlayerCombatController> allyCombatControllers = new();
    private List<EnemyAI> enemyAIs = new();
    [SerializeField] private GameObject[] allyPrefabsForCombat;

    [HideInInspector] public Enemy encounteredEnemy;

    [SerializeField] List<Transform> enemyCombatPositions;
    [SerializeField] List<Transform> allyCombatPositions;

    public CombatHUD[] enemyHUDs;
    public CombatHUD[] allyHUDs;

    [SerializeField] CombatState combatState;
    public CombatUI combatUI;

    [SerializeField] Camera mainCamera;
    [SerializeField] Camera combatCamera;

    private float initArmorModifier = 1;
    public int[] mentalSkillsMPCost;
    [HideInInspector] public bool isInCombat;
    [HideInInspector] public bool isChoosingEnemyForAttack;
    [HideInInspector] public bool isChoosingEnemyForItem;
    [HideInInspector] public bool isChoosingAllyForItem;
    public static CombatSystem instance;
    private System.Random random = new System.Random();

    [HideInInspector] public bool playerAttackButtonWasPressed;
    [HideInInspector] public bool playerIsHurting;

    [HideInInspector] public int damageTypeID;
    [HideInInspector] public bool isMental;
    [HideInInspector] public InventorySlot activeSlot;
    public bool isHitHimself;

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
        for (int i = 0; i < allyPrefabsForCombat.Length; i++)
        {
            GameObject allyCombat = Instantiate(allyPrefabsForCombat[i], allyCombatPositions[i]);
            //allyCombat.transform.Rotate(-allyCombat.transform.rotation.eulerAngles, Space.Self);
            allyUnits.Add(allyCombat.GetComponent<Unit>());
            //allyCombatControllers.Add(allyCombat.GetComponent<PlayerCombatController>());
            if (i == 0)
                (allyUnits[0] as Player).CopyStats(Inventory.instance.attachedUnit);
            allyUnits[i].knockedTurnsCount = 0;
            allyUnits[i].knockedDownTimeout = 0;
            allyHUDs[i].gameObject.SetActive(true);
            allyHUDs[i].SetHUD(allyUnits[i]);
            allyUnits[i].combatHUD = allyHUDs[i];
        }
        curAllyID = 0;
        for (int i = 0; i < encounteredEnemy.enemyPrefabsForCombat.Length; i++)
        {
            GameObject enemyCombat = Instantiate(encounteredEnemy.enemyPrefabsForCombat[i], enemyCombatPositions[i]);
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
        combatUI.UpdateMentalSkillButtons();

        combatCamera.enabled = true;
        mainCamera.enabled = false;
        combatUI.combatDialogue.text = "Битва началась";

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(AllyTurn());
    }


    //-----------------------------(Player's actions)-------------------------------------------------

    public IEnumerator AllyTurn()
    {
        if (curAllyID >= allyUnits.Count)
        {
            curAllyID = 0;
            StartCoroutine(EnemyTurn());
            yield break;
        }
        if (enemyUnits[curEnemyID].IsDead())
        {
            RemoveEnemy(curEnemyID);
            yield return new WaitForSeconds(1.5f);
            if (enemyUnits.Count == 0)
            {
                combatState = CombatState.WON;
                combatUI.combatDialogue.text = "Вы одержали победу!";
                yield return new WaitForSeconds(1.5f);
                FinishBattle();
                yield break;
            }
        }
        if (allyUnits[curAllyID].armorModifier != initArmorModifier)
            allyUnits[curAllyID].armorModifier = initArmorModifier;
        if (allyUnits[curAllyID].knockedDownTimeout > 0)
            allyUnits[curAllyID].knockedDownTimeout--;
        if (allyUnits[curAllyID].isKnockedDown)
        {
            allyUnits[curAllyID].isKnockedDown = false;
            allyUnits[curAllyID].knockedTurnsCount = 0;
            allyUnits[curAllyID].knockedDownTimeout = 3;
            combatUI.combatDialogue.text = allyUnits[curAllyID].unitName + " встал на ноги. Так продолжайте же сражаться!";
            yield return new WaitForSeconds(1.5f);
        }
        if (allyUnits[curAllyID].UnitEffectUpdate())
        {
            allyUnits[curAllyID].combatHUD.ChangeHP(allyUnits[curAllyID].currentHP);
            allyUnits[curAllyID].combatHUD.ChangeMP(allyUnits[curAllyID].currentMP);
            yield return new WaitForSeconds(1.5f);
        }
        if (allyUnits[curAllyID].underItemEffect)
        {
            if (allyUnits[curAllyID].affectingItem.doesHaveContinuousEffect)
            {
                allyUnits[curAllyID].affectingItem.ApplyEffect();
                yield return new WaitForSeconds(1f);
            }
            allyUnits[curAllyID].affectingItem.RemoveEffect();
            if (!allyUnits[curAllyID].underItemEffect)
            {
                combatUI.combatDialogue.text = "Эффект от предмета, наложенный на " + allyUnits[curAllyID].unitName + ", прошел";
                yield return new WaitForSeconds(1f);
            }
            else
                allyUnits[curAllyID].itemEffectTurnsCount++;
        }
        if (allyUnits[curAllyID].appliedEffect[1])
        {
            if (curAllyID + 1 < allyUnits.Count)
            {
                curAllyID++;
                StartCoroutine(AllyTurn());
            }
            else
            {
                curAllyID = 0;
                combatState = CombatState.ENEMY_TURN;
                StartCoroutine(EnemyTurn());
            }
        }
        else if (allyUnits[curAllyID].IsDead())
        {
            RemoveAlly(curAllyID);
            if (allyUnits.Count == 0)
            {
                combatState = CombatState.LOST;
                FinishBattle();
            }
            else
                StartCoroutine(AllyTurn());
        }
        else
        {
            combatState = CombatState.ALLY_TURN;
            combatUI.combatDialogue.text = "Выберите действие";
        }
    }

    private IEnumerator AllyDefend()
    {
        combatState = CombatState.ENEMY_TURN;
        initArmorModifier = allyUnits[curAllyID].armorModifier;
        allyUnits[curAllyID].armorModifier *= 0.4f;
        yield return new WaitForSeconds(1.5f);
        combatUI.combatDialogue.text = allyUnits[curAllyID].unitName + " успешно перешел в защиту";
        yield return new WaitForSeconds(1.5f);
        curAllyID++;
        StartCoroutine(AllyTurn());
    }

    public IEnumerator AllyUsingItem()
    {
        combatState = CombatState.ENEMY_TURN;
        allyUnits[curAllyID].combatHUD.ChangeHP(allyUnits[curAllyID].currentHP);
        allyUnits[curAllyID].combatHUD.ChangeMP(allyUnits[curAllyID].currentMP);
        enemyUnits[curEnemyID].combatHUD.ChangeHP(enemyUnits[curEnemyID].currentHP);
        enemyUnits[curEnemyID].combatHUD.ChangeMP(enemyUnits[curEnemyID].currentMP);
        activeSlot = null;
        yield return new WaitForSeconds(1.5f);
        if (!enemyUnits[curEnemyID].IsDead() && enemyUnits[curEnemyID].isKnockedDown && enemyUnits[curEnemyID].knockedTurnsCount == 0)
        {
            enemyUnits[curEnemyID].knockedTurnsCount++;
            combatUI.combatDialogue.text = "Враг сбит с ног. " + allyUnits[curAllyID].unitName + " предоставляется еще один ход!";
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(AllyTurn());
        }
        else
        {
            curAllyID++;
            StartCoroutine(AllyTurn());
        }
    }

    public IEnumerator AllyAttack(int damageTypeID, bool isMental)
    {
        combatState = CombatState.ENEMY_TURN;
        //playerAttackButtonWasPressed = true;
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
            message = ReflectAction(allyUnits[curAllyID], damageTypeID - 1, -CalcAffinityDamage(damageTypeID, isMental, allyUnits[curAllyID], allyUnits[curAllyID]), out effectMessage);
            //playerIsHurting = true;
            yield return new WaitForSeconds(1f);
        }
        else
        {
            int totalDamage = CalcAffinityDamage(damageTypeID, isMental, allyUnits[curAllyID], enemyUnits[curEnemyID]);
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
                0 => allyUnits[curAllyID].unitName + " наносит " + totalDamage + " физического урона",
                1 => allyUnits[curAllyID].unitName + " наносит " + totalDamage + " псионического урона",
                2 => allyUnits[curAllyID].unitName + " наносит " + totalDamage + " электрического урона",
                3 => allyUnits[curAllyID].unitName + " наносит " + totalDamage + " огненного урона",
                _ => string.Empty,
            };
        }
        if (isMental)
        {
            allyUnits[curAllyID].ReduceCurrentMP(mentalSkillsMPCost[0]);
            allyUnits[curAllyID].combatHUD.ChangeMP(allyUnits[curAllyID].currentMP);
        }
        if (!string.IsNullOrEmpty(effectMessage))
        {
            //playerIsHurting = false;
            enemyCombatControllers[curEnemyID].isHurting = false;
            combatUI.combatDialogue.text = effectMessage;
            yield return new WaitForSeconds(1.5f);
        }
        //playerAttackButtonWasPressed = false;
        allyUnits[curAllyID].combatHUD.ChangeHP(allyUnits[curAllyID].currentHP);
        enemyHUDs[curEnemyID].ChangeHP(enemyUnits[curEnemyID].currentHP);
        //playerIsHurting = false;
        enemyCombatControllers[curEnemyID].isHurting = false;
        combatUI.combatDialogue.text = message;
        yield return new WaitForSeconds(1.5f);
        if (allyUnits[curAllyID].IsDead())
        {
            RemoveAlly(curAllyID);
            yield return new WaitForSeconds(1.5f);
            if (allyUnits.Count == 0)
            {
                combatState = CombatState.LOST;
                FinishBattle();
            }
            else
                StartCoroutine(AllyTurn());
        }
        else if (!enemyUnits[curEnemyID].IsDead() && enemyUnits[curEnemyID].isKnockedDown && enemyUnits[curEnemyID].knockedTurnsCount == 0)
        {
            enemyUnits[curEnemyID].knockedTurnsCount++;
            combatUI.combatDialogue.text = "Враг сбит с ног. " + allyUnits[curAllyID].unitName + " предоставляется еще один ход!";
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(AllyTurn());
        }
        else
        {
            if (allyUnits[curAllyID].isKnockedDown && allyUnits[curAllyID].knockedTurnsCount == 0)
            {
                combatUI.combatDialogue.text = allyUnits[curAllyID].unitName + " сбит с ног собственной атакой. Неловко вышло...";
                yield return new WaitForSeconds(1.5f);
            }
            curAllyID++;
            StartCoroutine(AllyTurn());
        }
    }

    private IEnumerator AllyRegenaSkill()
    {
        throw new System.NotImplementedException();
        /*
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
            message = playerUnit.unitName + " восстанавливает " + (int)(playerUnit.maxHP * 0.25) + " здоровья";
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
            StartCoroutine(AllyTurn());
        }
        else
            StartCoroutine(AllyTurn());
        */
    }

    private void RemoveAlly(int allyID)
    {
        combatUI.combatDialogue.text = allyUnits[allyID].unitName + " повержен";
        allyUnits[allyID].combatHUD.gameObject.SetActive(false);
        Destroy(allyUnits[allyID].gameObject);
        //allyCombatControllers.RemoveAt(allyID);
        allyUnits.RemoveAt(allyID);
    }

    //-----------------------------(Enemy's actions)-------------------------------------------------

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
            if (enemyUnits[i].underItemEffect)
            {
                if (enemyUnits[i].affectingItem.doesHaveContinuousEffect)
                {
                    enemyUnits[i].affectingItem.ApplyEffect();
                    yield return new WaitForSeconds(1f);
                }
                enemyUnits[i].affectingItem.RemoveEffect();
                if (!enemyUnits[i].underItemEffect)
                {
                    combatUI.combatDialogue.text = "Эффект от предмета, наложенный на " + enemyUnits[i].unitName + ", прошел";
                    yield return new WaitForSeconds(1f);
                }
                else
                    enemyUnits[i].itemEffectTurnsCount++;
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
                    //playerIsHurting = false;
                    enemyCombatControllers[i].isHurting = false;
                }
            }
            enemyCombatControllers[i].attackButtonWasPressed = false;
            allyUnits[curAllyID].combatHUD.ChangeHP(allyUnits[curAllyID].currentHP);
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
            else if (allyUnits[curAllyID].IsDead())
            {
                RemoveAlly(curAllyID);
                yield return new WaitForSeconds(1.5f);
                if (allyUnits.Count == 0)
                {
                    combatState = CombatState.LOST;
                    FinishBattle();
                    yield break;
                }
            }
            else if (allyUnits[curAllyID].isKnockedDown && allyUnits[curAllyID].knockedTurnsCount == 0)
            {
                allyUnits[curAllyID].knockedTurnsCount++;
                combatUI.combatDialogue.text = allyUnits[curAllyID].unitName + " сбит с ног. Врагу, сбившему с ног, снова предоставляется ход!";
                yield return new WaitForSeconds(1.5f);
                i--;
            }
        }
        if (enemyUnits.Count == 0)
        {
            combatState = CombatState.WON;
            combatUI.combatDialogue.text = "Вы одержали победу!";
            yield return new WaitForSeconds(1.5f);
            FinishBattle();
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
            curAllyID = 0;
            StartCoroutine(AllyTurn());
        }
    }

    private void RemoveEnemy(int enemyID)
    {
        combatUI.combatDialogue.text = enemyUnits[enemyID].unitName + " оказывается повержен";
        enemyUnits[enemyID].combatHUD.gameObject.SetActive(false);
        Destroy(enemyUnits[enemyID].gameObject);
        enemyCombatControllers.RemoveAt(enemyID);
        enemyUnits.RemoveAt(enemyID);
        enemyAIs.RemoveAt(enemyID);
    }

    //-----------------------------(Methods for buttons)-------------------------------------------------

    public void OnItemButton()
    {
        if (combatState != CombatState.ALLY_TURN)
            return;
        isChoosingEnemyForAttack = false;
        isChoosingEnemyForItem = false;
        if (Inventory.instance.isOpen)
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
        if (combatState != CombatState.ALLY_TURN)
            return;
        if (Inventory.instance.isOpen)
            Inventory.instance.Close();
        if (combatUI.skillButtonsWereInstantiated && combatUI.areButtonsShown)
            combatUI.HideOrShowMentalSkillButtons();
        isChoosingEnemyForAttack = true;
        isChoosingEnemyForItem = false;
        isMental = false;
        damageTypeID = 0;
        combatUI.combatDialogue.text = "Выберите цель для атаки";
    }

    public void OnDefendButton()
    {
        if (combatState != CombatState.ALLY_TURN)
            return;
        isChoosingEnemyForAttack = false;
        isChoosingEnemyForItem = false;
        if (Inventory.instance.isOpen)
            Inventory.instance.Close();
        if (combatUI.skillButtonsWereInstantiated && combatUI.areButtonsShown)
            combatUI.HideOrShowMentalSkillButtons();
        combatUI.combatDialogue.text = allyUnits[curAllyID].unitName + " защищается";
        StartCoroutine(AllyDefend());
    }

    public void OnMentalButton()
    {
        if (combatState != CombatState.ALLY_TURN)
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
        isChoosingEnemyForAttack = false;
        isChoosingEnemyForItem = false;
        if (combatUI.areButtonsShown)
            combatUI.combatDialogue.text = "Выберите навык, который хотите применить";
        else
            combatUI.combatDialogue.text = "Выберите действие";
    }

    public void OnPsionaButton()
    {
        if (allyUnits[curAllyID].currentMP >= mentalSkillsMPCost[0])
        {
            combatUI.HideOrShowMentalSkillButtons();
            isChoosingEnemyForAttack = true;
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
        if (allyUnits[curAllyID].currentMP >= mentalSkillsMPCost[0])
        {
            combatUI.HideOrShowMentalSkillButtons();
            isChoosingEnemyForAttack = true;
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
        if (allyUnits[curAllyID].currentMP >= mentalSkillsMPCost[0])
        {
            combatUI.HideOrShowMentalSkillButtons();
            isChoosingEnemyForAttack = true;
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
        isChoosingEnemyForAttack = false;
        if (allyUnits[curAllyID].currentMP >= mentalSkillsMPCost[0])
        {
            combatUI.combatDialogue.text = allyUnits[curAllyID].unitName + " использует целительный навык";
            combatUI.HideOrShowMentalSkillButtons();
            StartCoroutine(AllyRegenaSkill());
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
            if (allyUnits[0].affectingItem != null)
            {
                curAllyID = 0;
                allyUnits[0].affectingItem.RemoveEffect();
            }
            Inventory.instance.attachedUnit.CopyStats(allyUnits[0] as Player);
            Inventory.instance.attachedUnit.GetComponent<PlayerMovement>().enabled = true;
            isInCombat = false;
            GameUI.instance.gameObject.SetActive(true);
            mainCamera.enabled = true;
            combatCamera.enabled = false;
            combatUI.gameObject.SetActive(false);
            Destroy(encounteredEnemy.gameObject);
            foreach (Unit ally in allyUnits)
                Destroy(ally.gameObject);
            allyUnits.Clear();
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
