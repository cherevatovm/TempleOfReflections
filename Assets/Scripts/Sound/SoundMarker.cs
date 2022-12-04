using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMarker : MonoBehaviour
{
    [SerializeField] SoundManager.Sound sound;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SoundManager.PlaySound(sound);
            Destroy(gameObject);
        }
    }
}
