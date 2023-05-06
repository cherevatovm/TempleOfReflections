using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PickableItem : MonoBehaviour
{
    [Header("Описание предмета")]
    protected bool isCloseToItem;
    public int itemID;
    public string itemName;
    public string itemDescription;
    public int itemValue;
    [HideInInspector] public bool isUsableInCombatOnly;
    [HideInInspector] public bool isAffectingEnemy;

    private void Update()
    {
        if (isCloseToItem && Input.GetKeyDown(KeyCode.F))
        {
            if (GameController.instance.isInTutorial)
                GameUI.instance.OpenItemPanel(this);
            Inventory.instance.PutInInventory(gameObject);
            GameUI.instance.gameDialogue.text = string.Empty;
        }
    }

    public abstract void UseItem(out string message);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isCloseToItem = true;
            GameUI.instance.gameDialogue.text = "Нажмите F, чтобы подобрать";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isCloseToItem = false;
            GameUI.instance.gameDialogue.text = string.Empty;
        }
    }
}
