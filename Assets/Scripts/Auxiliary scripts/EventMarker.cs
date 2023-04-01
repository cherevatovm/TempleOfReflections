using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventMarker : MonoBehaviour
{
    [SerializeField] private int eventID;
    [SerializeField] private SoundManager.Sound sound;

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
        GameController.instance.WriteData(EnemyInfoPanel.instance.enemyRecords);
        GameController.instance.SaveCurrentItemsData();
        ObjectPool.instance.objectToPool = null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
