using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyLockedDoor : Door
{
    private void Update()
    {
        if (isCloseToDoor && !isOpen && Input.GetKeyDown(KeyCode.F)) 
        {
            if (GameUI.instance.exitUI.activeSelf)
                GameUI.instance.ShowOrHideExitUI();
            if (Inventory.instance.doorKeysInPossession > 0)
            {
                Inventory.instance.doorKeysInPossession--;
                Open();
                GameUI.instance.gameDialogue.text = "Вы использовали ключ, чтобы отпереть дверь";
            }
            else
                GameUI.instance.gameDialogue.text = "У вас нет подходящего ключа, чтобы отпереть дверь";
        }
    }
}
