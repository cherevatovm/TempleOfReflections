using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PickableItem : MonoBehaviour
{
    [Header("�������� ��������")]
    private bool isCloseToItem;
    public string itemName;
    public string itemDescription;
    [HideInInspector] public bool isParasite;
    [HideInInspector] public bool isUsableInCombatOnly;

    private void Update()
    {
        if (isCloseToItem)
            if (Input.GetKeyDown(KeyCode.F))
            {
                Inventory.instance.PutInInventory(this, gameObject);
                gameObject.SetActive(false);
            }
    }

    public abstract void UseItem(out string message);

    private void OnTriggerEnter2D(Collider2D collision) => isCloseToItem = true;

    private void OnTriggerExit2D(Collider2D collision) => isCloseToItem = false;
}
