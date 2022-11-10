using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;

public enum CombatState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST }
public enum Actions { ATTACK, DEFEND, MISS, HEAL }

public class CombatSystem : MonoBehaviour
{
    Unit playerUnit;
    Unit enemyUnit;
    
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject enemyPrefab;

    [SerializeField] Transform playerCombatPosition;
    [SerializeField] Transform enemyCombatPosition;

    [SerializeField] CombatHUD playerHUD;
    [SerializeField] CombatHUD enemyHUD;
    public TextMeshProUGUI combatDialogue;

    [SerializeField] CombatState combatState;

    [SerializeField] Camera mainCamera;
    [SerializeField] Camera combatCamera;

    //System.Random random = new System.Random();

    void Start()
    {
        combatState = CombatState.START;
        combatCamera.enabled = true;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        GameObject playerCombat = Instantiate(playerPrefab, playerCombatPosition);
        playerUnit = playerCombat.GetComponent<Unit>();

        GameObject enemyCombat = Instantiate(enemyPrefab, enemyCombatPosition);
        enemyUnit = enemyCombat.GetComponent<Unit>();

        //playerHUD.SetHUD(playerUnit);
        //enemyHUD.SetHUD(enemyUnit);

        combatCamera.enabled = true;
        mainCamera.enabled = false;
        combatDialogue.text = "Битва началась";

        yield return new WaitForSeconds(2f);

        combatState = CombatState.PLAYER_TURN;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        combatDialogue.text = "Выберите действие:";
    }

    IEnumerator PlayerAttack()
    {
        enemyUnit.TakeDamage(playerUnit.attackStrength);

        yield return new WaitForSeconds(1f);
        //enemyHUD.ChangeHP(enemyUnit.currentHP);
        combatDialogue.text = "Атака проведена успешно";
        yield return new WaitForSeconds(1f);

        if (enemyUnit.IsDead())
        {
            combatState = CombatState.WON;
            FinishBattle();
        }
        else
        {
            combatState = CombatState.ENEMY_TURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        combatDialogue.text = "Враг наносит удар";
        yield return new WaitForSeconds(1f);
        playerUnit.TakeDamage(enemyUnit.attackStrength);
        /*
        Реализовать логику поведения противника
        */
        //playerHUD.ChangeHP(playerUnit.currentHP);
        yield return new WaitForSeconds(1f);
        
        if (playerUnit.IsDead())
        {
            combatState = CombatState.LOST;
            FinishBattle();
        }
        else
        {
            combatState = CombatState.PLAYER_TURN;
            PlayerTurn();
        }
    }

    public void OnAttackButton()
    {
        if (combatState != CombatState.PLAYER_TURN)
            return;
        combatDialogue.text = "Игрок начинает атаку";
        StartCoroutine(PlayerAttack());
    }

    void FinishBattle()
    {
        if (combatState == CombatState.WON)
        {
            combatDialogue.text = "Вы одержали победу!";
            mainCamera.enabled = true;
            combatCamera.enabled = false;
            GameObject.Destroy(playerCombatPosition.GetChild(0).gameObject);
            GameObject.Destroy(enemyCombatPosition.GetChild(0).gameObject);
        }
        else
            if (combatState == CombatState.LOST)
        {
            combatDialogue.text = "Враг одержал верх...";
            playerCombatPosition.GetChild(0).gameObject.GetComponent<HealthSystem>().Death();
        }
    }
}
