using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoPanel : MonoBehaviour
{
    private class EnemyRecord
    {
        public int slainInTotal;
        public bool[] knownAffinities = new bool[4];

        public bool this[int i]
        {
            get => knownAffinities[i];
            set => knownAffinities[i] = value;
        }
    }

    [SerializeField] private CombatHUD enemyHUD;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text slainCounter;
    [SerializeField] private Text coinCounter;
    [SerializeField] private Image enemyImage;
    [SerializeField] private TMP_Text[] affinityLabels;
    private List<EnemyRecord> enemyStats = new();
    private string prevCombatLine;
    public static EnemyInfoPanel instance;

    private void Start()
    {
        instance = this;
        for (int i = 0; i < 3; i++)
            enemyStats.Add(new EnemyRecord());
        enemyImage.preserveAspect = true;
        gameObject.SetActive(false);
    }

    public void ChangeKnownAffinities(int enemyID, int damageTypeID) => enemyStats[enemyID][damageTypeID] = true;

    public void IncreaseSlainInTotalCount(int enemyID) => enemyStats[enemyID].slainInTotal++;

    public void OpenEnemyInfoPanel(Enemy enemy)
    {
        enemyHUD.SetHUD(enemy);
        if (enemyStats[enemy.enemyID].slainInTotal > 0)
            coinCounter.text = enemy.coinsDropped.ToString();
        else
            coinCounter.text = "???";
        if (enemyStats[enemy.enemyID].slainInTotal > 2)
            descriptionText.text = enemy.enemyDescription;
        else
            descriptionText.text = "????????????????????????\n????????????????????????\n????????????????????????";
        slainCounter.text = enemyStats[enemy.enemyID].slainInTotal.ToString();
        enemyImage.sprite = enemy.gameObject.GetComponent<SpriteRenderer>().sprite;
        enemyImage.SetNativeSize();
        if (enemy.enemyID == 0)
            enemyImage.transform.localScale = new Vector3(0.8f, 0.8f);
        else
            enemyImage.transform.localScale = new Vector3(1.2f, 1.2f);
        for (int i = 0; i < affinityLabels.Length; i++)
        {
            if (enemyStats[enemy.enemyID][i])
            {
                if (enemy.weaknesses[i])
                    affinityLabels[i].text = "Слабость";
                else if (enemy.resistances[i])
                    affinityLabels[i].text = "Сопротив.";
                else if (enemy.nulls[i])
                    affinityLabels[i].text = "Невоспр.";
                else
                    affinityLabels[i].text = "Нейтрально";
            }
            else
                affinityLabels[i].text = "???";
        }
        prevCombatLine = CombatSystem.instance.combatUI.combatDialogue.text;
        CombatSystem.instance.combatUI.combatDialogue.text = "Известная на данный момент информация о враге";
        CombatSystem.instance.combatUI.blackouts[1].SetActive(true);
        gameObject.SetActive(true);
    }

    public void CloseEnemyInfoPanel()
    {
        CombatSystem.instance.combatUI.combatDialogue.text = prevCombatLine;
        CombatSystem.instance.combatUI.blackouts[1].SetActive(false);
        gameObject.SetActive(false);
    }
}
