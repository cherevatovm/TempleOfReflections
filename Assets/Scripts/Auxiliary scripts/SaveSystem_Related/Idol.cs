using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idol : MonoBehaviour
{
    private bool isClose;
    [SerializeField] private int index;

    private void Update()
    {
        if (isClose && Input.GetKeyDown(KeyCode.F))
        {
            if (GameController.instance.isInTutorial && !GameController.instance.wasGameSaved)
            {               
                GameUI.instance.ItemPanelTutorialMode(4);
                GameUI.instance.OpenItemPanel();
                GameController.instance.wasGameSaved = true;
            }
            SaveSystem.Save(new SavedData(Inventory.instance.attachedUnit,
                SaveController.instance.GetInventoryData(false), -1, index, SaveController.instance.GetItemDataList(), SaveController.instance.GetParasiteDataList(),
                SaveController.instance.GetContainerDataList(), SaveController.instance.GetMerchantDataList(), SaveController.instance.GetTalkedToNpcData(),
                SaveController.instance.GetDoorsData(), SaveController.instance.GetDestEventsData(), 
                SaveController.instance.GetSpawnedUnitsData(), SaveController.instance.GetSlainEnemyList()));
            GameUI.instance.gameDialogue.text = "Игра сохранена";
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isClose = true;
        GameUI.instance.gameDialogue.text = "Нажмите F, чтобы сохранить прогресс";
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isClose = false;
        GameUI.instance.gameDialogue.text = string.Empty;
        if (GameController.instance.isInTutorial)
            GameUI.instance.CloseItemPanel();
    }
}
