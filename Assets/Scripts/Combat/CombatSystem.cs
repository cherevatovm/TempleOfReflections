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

    public float effectProbability = 0.2f;
    public float reflectionProbability1;
    public float reflectionProbability2;

    [HideInInspector] public int curEnemyID;
    [HideInInspector] public List<Enemy> enemyUnits = new();
    [HideInInspector] public int curAllyID;
    [HideInInspector] public int tempAllyID;
    [HideInInspector] public List<Unit> allyUnits = new();
    [HideInInspector] public List<CombatController> enemyCombatControllers = new();
    [HideInInspector] public List<CombatController> allyCombatControllers = new();
    [HideInInspector] public List<EnemyAI> enemyAIs = new();
    public List<GameObject> allyPrefabsForCombat;

    [HideInInspector] public Enemy encounteredEnemy;

    public List<Transform> enemyCombatPositions;
    [SerializeField] List<Transform> allyCombatPositions;

    public CombatHUD[] enemyHUDs;
    public CombatHUD[] allyHUDs;

    public CombatState combatState;
    public CombatUI combatUI;

    [SerializeField] Camera mainCamera;
    [SerializeField] Camera combatCamera;

    private float[] initArmorModifiers = new float[4];
    public int[] mentalSkillsMPCost;
    [HideInInspector] public bool isInCombat;
    [HideInInspector] public bool isChoosingEnemyForAttack;
    [HideInInspector] public bool isChoosingEnemyForItem;
    [HideInInspector] public bool isChoosingAllyForSkill;
    [HideInInspector] public bool isChoosingAllyForItem;
    public static CombatSystem instance;
    private System.Random random = new System.Random();

    [HideInInspector] public bool playerAttackButtonWasPressed;
    [HideInInspector] public bool playerIsHurting;

    [HideInInspector] public int damageTypeID;
    [HideInInspector] public bool isMental;
    [HideInInspector] public InventorySlot activeSlot;

    private void Start()
    {
        combatCamera.enabled = false;
        combatState = CombatState.START;
        instance = this;
    }

    public IEnumerator SetupBattle()
    {
        //SoundManager.StopLoopedSound();
        //SoundManager.PlaySound(SoundManager.Sound.EnterCombat);
        //SoundManager.PlaySound(SoundManager.Sound.MentalBattle);
        Inventory.instance.Close();
        encounteredEnemy.gameObject.GetComponent<Collider2D>().enabled = false;
        encounteredEnemy.gameObject.GetComponent<EnemyMovement>().enabled = false;
        Inventory.instance.attachedUnit.GetComponent<PlayerMovement>().enabled = false;
        for (int i = 0; i < allyPrefabsForCombat.Count; i++)
        {
            GameObject allyCombat = Instantiate(allyPrefabsForCombat[i], allyCombatPositions[i]);
            allyUnits.Add(allyCombat.GetComponent<Unit>());
            allyCombatControllers.Add(allyCombat.GetComponent<CombatController>());
            if (i == 0)
                (allyUnits[0] as Player).CopyStats(Inventory.instance.attachedUnit);
            allyUnits[i].knockedTurnsCount = 0;
            allyUnits[i].knockedDownTimeout = 0;
            allyHUDs[i].gameObject.SetActive(true);
            allyHUDs[i].SetHUD(allyUnits[i]);
            allyUnits[i].combatHUD = allyHUDs[i];
            initArmorModifiers[i] = allyUnits[i].armorModifier;
        }
        curAllyID = 0;
        for (int i = 0; i < encounteredEnemy.enemyPrefabsForCombat.Length; i++)
        {
            GameObject enemyCombat = Instantiate(encounteredEnemy.enemyPrefabsForCombat[i], enemyCombatPositions[i]);
            enemyUnits.Add(enemyCombat.GetComponent<Enemy>());
            enemyAIs.Add(enemyCombat.GetComponent<EnemyAI>());
            enemyCombatControllers.Add(enemyCombat.GetComponent<CombatController>());
            enemyUnits[i].knockedTurnsCount = 0;
            enemyUnits[i].knockedDownTimeout = 0;
            enemyHUDs[i].gameObject.SetActive(true);
            enemyHUDs[i].SetHUD(enemyUnits[i]);
            enemyUnits[i].combatHUD = enemyHUDs[i];
        }
        curEnemyID = 0;
        GameUI.instance.gameObject.SetActive(false);
        combatUI.gameObject.SetActive(true);
        isInCombat = true;
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
            curEnemyID = 0;
        }
        if (allyUnits[curAllyID].armorModifier != initArmorModifiers[curAllyID])
            allyUnits[curAllyID].armorModifier = initArmorModifiers[curAllyID];
        if (allyUnits[curAllyID].knockedDownTimeout > 0)
            allyUnits[curAllyID].knockedDownTimeout--;
        if (allyUnits[curAllyID].isKnockedDown)
        {
            allyUnits[curAllyID].isKnockedDown = false;
            allyUnits[curAllyID].knockedTurnsCount = 0;
            allyUnits[curAllyID].knockedDownTimeout = 3;
            combatUI.combatDialogue.text = allyUnits[curAllyID].unitName + " встает на ноги. Так продолжайте же сражаться!";
            yield return new WaitForSeconds(1.5f);
        }
        if (allyUnits[curAllyID].UnitEffectUpdate())
        {
            allyUnits[curAllyID].combatHUD.ChangeHP(allyUnits[curAllyID].currentHP);
            allyUnits[curAllyID].combatHUD.ChangeMP(allyUnits[curAllyID].currentMP);
            yield return new WaitForSeconds(1.5f);
        }
        var items = allyUnits[curAllyID].affectingItems;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].doesHaveContinuousEffect)
            {
                items[i].ApplyEffect();
                yield return new WaitForSeconds(1f);
            }
            if (items[i].underEffectTurnsCounter == items[i].underEffectTurnsNumber)
            {
                items[i].RemoveEffect();
                combatUI.combatDialogue.text = items[i].itemName + ": эффект от предмета, наложенный на " + allyUnits[curAllyID].unitName + ", прошел";
                i--;
                yield return new WaitForSeconds(1f);
            }
            else
                items[i].underEffectTurnsCounter++;
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
            ItemWithEffect sacrDoll = allyUnits[curAllyID].affectingItems.Find(item => item is SacrificialDoll);
            if (sacrDoll != null)
            {
                sacrDoll.RemoveEffect();
                yield return new WaitForSeconds(1.5f);
                combatState = CombatState.ALLY_TURN;
                combatUI.combatDialogue.text = "Выберите действие (" + allyUnits[curAllyID].unitName + " ходит в данный момент)";
            }
            else
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
        }
        else
        {
            combatState = CombatState.ALLY_TURN;
            combatUI.combatDialogue.text = "Выберите действие (" + allyUnits[curAllyID].unitName + " ходит в данный момент)";
        }
    }

    private IEnumerator AllyDefend()
    {
        if (enemyAIs[curEnemyID] is CloneEnemyAI)
            (enemyAIs[curEnemyID] as CloneEnemyAI).countWeaknessesTurns++;
        combatState = CombatState.ENEMY_TURN;
        allyUnits[curAllyID].armorModifier *= 0.4f;
        yield return new WaitForSeconds(1.5f);
        combatUI.combatDialogue.text = allyUnits[curAllyID].unitName + " успешно переходит в защиту";
        yield return new WaitForSeconds(1.5f);
        curAllyID++;
        StartCoroutine(AllyTurn());
    }

    public IEnumerator AllyUsingItem()
    {
        if (enemyAIs[curEnemyID] is CloneEnemyAI)
            (enemyAIs[curEnemyID] as CloneEnemyAI).countWeaknessesTurns++;
        combatState = CombatState.ENEMY_TURN;
        allyUnits[tempAllyID].combatHUD.ChangeHP(allyUnits[tempAllyID].currentHP);
        allyUnits[tempAllyID].combatHUD.ChangeMP(allyUnits[tempAllyID].currentMP);
        enemyUnits[curEnemyID].combatHUD.ChangeHP(enemyUnits[curEnemyID].currentHP);
        enemyUnits[curEnemyID].combatHUD.ChangeMP(enemyUnits[curEnemyID].currentMP);
        tempAllyID = 0;
        activeSlot = null;
        yield return new WaitForSeconds(1.5f);
        if (!enemyUnits[curEnemyID].IsDead() && enemyUnits[curEnemyID].isKnockedDown && enemyUnits[curEnemyID].knockedTurnsCount == 0)
        {
            enemyUnits[curEnemyID].knockedTurnsCount++;
            combatUI.combatDialogue.text = enemyUnits[curEnemyID].unitName + " падает с ног. " + allyUnits[curAllyID].unitName + " предоставляется еще один ход!";
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
        allyCombatControllers[curAllyID].attackButtonWasPressed = true;
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
            allyCombatControllers[curAllyID].isHurting = true;
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
            if (!enemyUnits[curEnemyID].weaknesses[damageTypeID] && enemyAIs[curEnemyID] is CloneEnemyAI)
                (enemyAIs[curEnemyID] as CloneEnemyAI).countWeaknessesTurns++;
            EnemyInfoPanel.instance.ChangeKnownAffinities(enemyUnits[curEnemyID].enemyID, damageTypeID);
        }
        if (isMental)
        {
            allyUnits[curAllyID].ReduceCurrentMP(mentalSkillsMPCost[0]);
            allyUnits[curAllyID].combatHUD.ChangeMP(allyUnits[curAllyID].currentMP);
        }
        if (!string.IsNullOrEmpty(effectMessage))
        {
            allyCombatControllers[curAllyID].isHurting = false;
            enemyCombatControllers[curEnemyID].isHurting = false;
            combatUI.combatDialogue.text = effectMessage;
            yield return new WaitForSeconds(1.5f);
        }
        allyCombatControllers[curAllyID].attackButtonWasPressed = false;
        allyUnits[curAllyID].combatHUD.ChangeHP(allyUnits[curAllyID].currentHP);
        enemyUnits[curEnemyID].combatHUD.ChangeHP(enemyUnits[curEnemyID].currentHP);
        allyCombatControllers[curAllyID].isHurting = false;
        enemyCombatControllers[curEnemyID].isHurting = false;
        combatUI.combatDialogue.text = message;
        yield return new WaitForSeconds(1.5f);
        if (allyUnits[curAllyID].IsDead())
        {
            ItemWithEffect sacrDoll = allyUnits[curAllyID].affectingItems.Find(item => item is SacrificialDoll);
            if (sacrDoll != null)
            {
                sacrDoll.RemoveEffect();
                yield return new WaitForSeconds(1.5f);
                StartCoroutine(AllyTurn());
            }
            else
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
        }
        else if (!enemyUnits[curEnemyID].IsDead() && enemyUnits[curEnemyID].isKnockedDown && enemyUnits[curEnemyID].knockedTurnsCount == 0)
        {
            enemyUnits[curEnemyID].knockedTurnsCount++;
            combatUI.combatDialogue.text = enemyUnits[curEnemyID].unitName + " падает с ног. " + allyUnits[curAllyID].unitName + " предоставляется еще один ход!";
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(AllyTurn());
        }
        else
        {
            if (allyUnits[curAllyID].isKnockedDown && allyUnits[curAllyID].knockedTurnsCount == 0)
            {
                combatUI.combatDialogue.text = allyUnits[curAllyID].unitName + " падает с ног из-за собственной атаки. Неловко вышло...";
                yield return new WaitForSeconds(1.5f);
            }
            curAllyID++;
            StartCoroutine(AllyTurn());
        }
    }

    public IEnumerator AllyRegenaSkill()
    {
        if (enemyAIs[curEnemyID] is CloneEnemyAI)
            (enemyAIs[curEnemyID] as CloneEnemyAI).countWeaknessesTurns++;
        combatState = CombatState.ENEMY_TURN;
        allyCombatControllers[curAllyID].attackButtonWasPressed = true;
        yield return new WaitForSeconds(1.5f);
        string message;
        if (reflectionProbability2 > 0 && random.NextDouble() < reflectionProbability2)
        {
            curEnemyID = random.Next(0, enemyUnits.Count);
            message = ReflectAction(enemyUnits[curEnemyID], -1, (int)(enemyUnits[curEnemyID].maxHP * 0.25), out _);
            enemyUnits[curEnemyID].combatHUD.ChangeHP(enemyUnits[curEnemyID].currentHP);
        }
        else
        {
            allyUnits[tempAllyID].Heal((int)(allyUnits[tempAllyID].maxHP * 0.25));
            message = allyUnits[tempAllyID].unitName + " восстанавливает " + (int)(allyUnits[tempAllyID].maxHP * 0.25) + " здоровья";
            allyUnits[tempAllyID].combatHUD.ChangeHP(allyUnits[tempAllyID].currentHP);
        }
        allyUnits[curAllyID].ReduceCurrentMP(mentalSkillsMPCost[0]);
        allyUnits[curAllyID].combatHUD.ChangeMP(allyUnits[curAllyID].currentMP);
        allyCombatControllers[curAllyID].attackButtonWasPressed = false;
        combatUI.combatDialogue.text = message;
        tempAllyID = 0;
        yield return new WaitForSeconds(1.5f);
        curAllyID++;
        StartCoroutine(AllyTurn());
    }

    private void RemoveAlly(int allyID)
    {
        if (allyUnits[allyID] is Player)
        {
            Inventory.instance.attachedUnit.currentHP = 1;
            Inventory.instance.attachedUnit.currentMP = allyUnits[allyID].currentMP;
        }
        combatUI.combatDialogue.text = allyUnits[allyID].unitName + " получает фатальный удар!";
        allyCombatControllers[allyID].isDying = true;
        allyUnits[allyID].combatHUD.gameObject.SetActive(false);
        Destroy(allyUnits[allyID].gameObject, 1.5f);
        allyCombatControllers.RemoveAt(allyID);
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
            var items = enemyUnits[i].affectingItems;
            for (int j = 0; j < items.Count; j++)
            {
                if (items[j].doesHaveContinuousEffect)
                {
                    items[j].ApplyEffect();
                    yield return new WaitForSeconds(1f);
                }
                if (items[j].underEffectTurnsCounter == items[j].underEffectTurnsNumber)
                {
                    items[j].RemoveEffect();
                    combatUI.combatDialogue.text = items[j].itemName + ": эффект от предмета, наложенный на " + enemyUnits[i].unitName + ", прошел";
                    j--;
                    yield return new WaitForSeconds(1f);
                }
                else
                    items[j].underEffectTurnsCounter++;
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
                combatUI.combatDialogue.text = enemyUnits[i].unitName + " встает на ноги";
                yield return new WaitForSeconds(1.5f);
            }
            enemyCombatControllers[i].attackButtonWasPressed = true;
            //countCopyTurns++;
            if (enemyAIs[i] is CopyEnemyAI)
                (enemyAIs[i] as CopyEnemyAI).countCopyTurns++;
            List<string> messages = enemyAIs[i].CombatAI(out int soundID);
            for (int j = 0; j < messages.Count; j++)
            {
                if (enemyUnits[i].enemyID == 0 && j == messages.Count - 1 && soundID != -1)
                    SoundManager.PlaySound((SoundManager.Sound)soundID);
                if (!string.IsNullOrEmpty(messages[j]))
                {
                    combatUI.combatDialogue.text = messages[j];
                    yield return new WaitForSeconds(1.5f);
                    allyCombatControllers[curAllyID].isHurting = false;
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
                combatUI.combatDialogue.text = enemyUnits[i].unitName + " падает с ног из-за собственной атаки. Какая неудача!";
                yield return new WaitForSeconds(1.5f);
            }
            else if (allyUnits[curAllyID].IsDead())
            {
                ItemWithEffect sacrDoll = allyUnits[curAllyID].affectingItems.Find(item => item is SacrificialDoll);
                if (sacrDoll != null)
                {
                    sacrDoll.RemoveEffect();
                    yield return new WaitForSeconds(1.5f);
                }
                else
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
            }
            else if (allyUnits[curAllyID].isKnockedDown && allyUnits[curAllyID].knockedTurnsCount == 0)
            {
                allyUnits[curAllyID].knockedTurnsCount++;
                combatUI.combatDialogue.text = allyUnits[curAllyID].unitName + " падает с ног из-за вражеской атаки. " + enemyUnits[i].unitName + " снова предоставляется ход!";
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

    private void RemoveEnemy(int enemyInArenaID)
    {
        combatUI.combatDialogue.text = enemyUnits[enemyInArenaID].unitName + " оказывается повержен, оставляя после себя " + enemyUnits[enemyInArenaID].coinsDropped + " монет";
        Inventory.instance.ChangeCoinAmount(false, enemyUnits[enemyInArenaID].coinsDropped);
        EnemyInfoPanel.instance.IncreaseSlainInTotalCount(enemyUnits[enemyInArenaID].enemyID);
        enemyUnits[enemyInArenaID].combatHUD.gameObject.SetActive(false);
        Destroy(enemyUnits[enemyInArenaID].gameObject);
        enemyCombatControllers.RemoveAt(enemyInArenaID);
        enemyUnits.RemoveAt(enemyInArenaID);
        enemyAIs.RemoveAt(enemyInArenaID);
    }

    //-----------------------------(Methods for buttons)-------------------------------------------------

    public void OnItemButton()
    {
        if (combatState != CombatState.ALLY_TURN)
            return;
        isChoosingEnemyForAttack = false;
        isChoosingEnemyForItem = false;
        isChoosingAllyForSkill = false;
        isChoosingAllyForItem = false;
        if (EnemyInfoPanel.instance.isActiveAndEnabled)
            EnemyInfoPanel.instance.CloseEnemyInfoPanel();
        if (Inventory.instance.isOpen)
        {
            Inventory.instance.Close();
            combatUI.combatDialogue.text = "Выберите действие (" + allyUnits[curAllyID].unitName + " ходит в данный момент)"; ;
        }
        else
        {
            Inventory.instance.Open();
            combatUI.combatDialogue.text = "Выберите предмет, который хотите использовать";
        }
        if (combatUI.skillButtonsHaveBeenSet && combatUI.areButtonsShown)
            combatUI.HideOrShowMentalSkillButtons();
    }

    public void OnAttackButton()
    {
        if (combatState != CombatState.ALLY_TURN)
            return;
        if (EnemyInfoPanel.instance.isActiveAndEnabled)
            EnemyInfoPanel.instance.CloseEnemyInfoPanel();
        if (Inventory.instance.isOpen)
            Inventory.instance.Close();
        if (combatUI.skillButtonsHaveBeenSet && combatUI.areButtonsShown)
            combatUI.HideOrShowMentalSkillButtons();
        isChoosingEnemyForAttack = true;
        isChoosingEnemyForItem = false;
        isChoosingAllyForSkill = false;
        isChoosingAllyForItem = false;
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
        isChoosingAllyForSkill = false;
        isChoosingAllyForItem = false;
        if (EnemyInfoPanel.instance.isActiveAndEnabled)
            EnemyInfoPanel.instance.CloseEnemyInfoPanel();
        if (Inventory.instance.isOpen)
            Inventory.instance.Close();
        if (combatUI.skillButtonsHaveBeenSet && combatUI.areButtonsShown)
            combatUI.HideOrShowMentalSkillButtons();
        combatUI.combatDialogue.text = allyUnits[curAllyID].unitName + " решает защищаться";
        StartCoroutine(AllyDefend());
    }

    public void OnMentalButton()
    {
        if (combatState != CombatState.ALLY_TURN)
            return;
        if (EnemyInfoPanel.instance.isActiveAndEnabled)
            EnemyInfoPanel.instance.CloseEnemyInfoPanel();
        if (Inventory.instance.isOpen)
            Inventory.instance.Close();
        if (combatUI.skillButtonsHaveBeenSet)
            combatUI.HideOrShowMentalSkillButtons();
        else
        {
            combatUI.SetMentalSkillButtons();
            combatUI.skillButtonsHaveBeenSet = true;
        }
        isChoosingEnemyForAttack = false;
        isChoosingEnemyForItem = false;
        isChoosingAllyForSkill = false;
        isChoosingAllyForItem = false;
        if (combatUI.areButtonsShown)
            combatUI.combatDialogue.text = "Выберите навык, который хотите применить";
        else
            combatUI.combatDialogue.text = "Выберите действие (" + allyUnits[curAllyID].unitName + " ходит в данный момент)";
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
        if (allyUnits[curAllyID].currentMP >= mentalSkillsMPCost[0])
        {
            combatUI.HideOrShowMentalSkillButtons();
            isChoosingAllyForSkill = true;
            combatUI.combatDialogue.text = "Выберите цель для использования навыка";
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
            if (allyUnits[0] is Player)
                Inventory.instance.attachedUnit.CopyStats(allyUnits[0] as Player);
            Inventory.instance.attachedUnit.GetComponent<PlayerMovement>().enabled = true;
            isInCombat = false;
            GameUI.instance.gameObject.SetActive(true);
            GameUI.instance.ChangeHP(Inventory.instance.attachedUnit.currentHP);
            GameUI.instance.ChangeMP(Inventory.instance.attachedUnit.currentMP);
            mainCamera.enabled = true;
            combatCamera.enabled = false;
            combatUI.gameObject.SetActive(false);
            Destroy(encounteredEnemy.gameObject);
            foreach (Unit ally in allyUnits)
                Destroy(ally.gameObject);
            allyCombatControllers.Clear();
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
            message = "Навык оказывается отражен в " + receiver.unitName + ", восстанавливая " + totalDamage + " здоровья";
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
