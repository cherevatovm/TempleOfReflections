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
                GameUI.instance.gameDialogue.text = "��������� �����, ��������� � ������ �������, ���������, ��� ������� ������� �������� �����";
            }
            else
                GameUI.instance.gameDialogue.text = "�����, ��������� � ������ �������, ����, ������� �������� ����� �����������";
        }
    }
}
