using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obelisk : MonoBehaviour
{
    private bool isClose;
    [SerializeField] private int index;

    private void Update()
    {
        if (isClose && Input.GetKeyDown(KeyCode.F))
            SaveSystem.Save(new SavedData(Inventory.instance.attachedUnit, index,
                SaveController.instance.GetInventoryData(), SaveController.instance.GetItemDataList(), SaveController.instance.GetContainerDataList(), 
                SaveController.instance.GetMerchantDataList(), SaveController.instance.GetSlainEnemyList()));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isClose = true;
        GameUI.instance.gameDialogue.text = "������� F, ����� ��������� ��������";
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isClose = false;
        GameUI.instance.gameDialogue.text = string.Empty;
    }
}
