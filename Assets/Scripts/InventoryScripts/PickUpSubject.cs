using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpSubject : MonoBehaviour
{
    SpriteRenderer img;
    public string Name = "";
    public string description = "";
    public bool ispar = false;
    private GameObject obj;
    public bool InObj = false;
    private void Start()
    {
        name = gameObject.GetComponent<Item>().Name;
        img = gameObject.GetComponent<SpriteRenderer>();
        ispar = gameObject.GetComponent<Item>().isparasite;
        description = gameObject.GetComponent<Item>().description;
        obj = gameObject;
    }

    private void Update()
    {
        if (InObj)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Inventory.instance.PutInEmptySlot(img.sprite, name, ispar, description, obj);
                gameObject.SetActive(false);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        InObj = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        InObj = false;
    }
}
