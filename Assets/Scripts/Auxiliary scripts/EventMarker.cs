using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventMarker : MonoBehaviour
{
    [SerializeField] private int eventID;
    [SerializeField] private SoundManager.Sound sound;
    [SerializeField] private NoteTrigger note;
    [SerializeField] private Enemy enemyToFight;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            switch (eventID)
            {
                case 0:
                    PlaySound();
                    break;
                case 1:
                    TransitionToNextScene(); 
                    break;
                case 2:
                    ShowNote();
                    break;
                case 3:
                    StartBossFight();
                    break;
            }
        }
    }

    private void PlaySound()
    {
        if (ObjectPool.instance.objectToPool == null)
            ObjectPool.instance.SetSoundPool();
        SoundManager.PlaySound(sound);
        Destroy(gameObject);
    }

    private void TransitionToNextScene()
    {
        SaveSystem.Save(new SavedData(Inventory.instance.attachedUnit, SaveController.instance.GetInventoryData(true), SceneManager.GetActiveScene().buildIndex + 1));
        GameController.instance.isSwitchingScenes = true;
        SaveSystem.Load();
    }

    private void ShowNote()
    {
        NoteManager.instance.dialogueTrigger = note;
        NoteManager.instance.StartReading(note.dialogue1);
    }

    private void StartBossFight()
    {
        if (Inventory.instance.doorKeysInPossession == 2)
        {
            CombatSystem.instance.encounteredEnemy = enemyToFight;
            StartCoroutine(CombatSystem.instance.SetupBattle());
        }
    }
}
