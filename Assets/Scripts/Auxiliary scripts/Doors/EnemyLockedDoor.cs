using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLockedDoor : Door
{
    [SerializeField] private GameObject[] guardingEnemies;

    private void Update()
    {
        if (isCloseToDoor && !isOpen && Input.GetKeyDown(KeyCode.F))
        {
            if (GameUI.instance.exitUI.activeSelf)
                GameUI.instance.ShowOrHideExitUI();
            if (!System.Array.Exists(guardingEnemies, elem => elem != null))
            {
                Open();
                GameUI.instance.gameDialogue.text = "ѕоскольку враги, св€занные с дверью печатью, повержены, вам удалось успешно отпереть дверь";
            }
            else
                GameUI.instance.gameDialogue.text = "¬раги, св€занные с дверью печатью, живы, попытка отпереть дверь провалилась";
        }
    }
}
