using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Door : MonoBehaviour
{
    protected bool isOpen;
    protected bool isCloseToDoor;
    //[SerializeField] protected Sprite openVersion;
    [SerializeField] protected Collider2D nonTriggerCollider;

    public bool GetIsOpen() => isOpen;

    public void Open()
    {
        isOpen = true;
        nonTriggerCollider.enabled = false;
        gameObject.GetComponent<SpriteRenderer>().sprite = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            isCloseToDoor = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isCloseToDoor = false;
            GameUI.instance.gameDialogue.text = string.Empty;
        }
    }
}
