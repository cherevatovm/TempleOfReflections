using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickableItem : MonoBehaviour
{
    [Header("Описание предмета")]
    public string itemName;
    public string itemDescription;
    public bool isParasite; 
    private bool isCloseToItem;

    private void Update()
    {
        if (isCloseToItem)
            if (Input.GetKeyDown(KeyCode.F))
                Inventory.instance.PutInInventory(this, gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) => isCloseToItem = true;

    private void OnTriggerExit2D(Collider2D collision) => isCloseToItem = false;
}
