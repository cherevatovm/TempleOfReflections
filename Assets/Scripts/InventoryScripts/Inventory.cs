using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Transform ParentSlotforSubject;
    public Transform ParentSlotforParasite;
    public static Inventory instance;
    public List<InventorySlots> inventorySlotsforSubject = new List<InventorySlots>();
    public List<InventorySlots> inventorySlotsforParasite = new List<InventorySlots>();
    bool IsOpened;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        for (int i = 0; i < 8; i++)
        {
            inventorySlotsforSubject.Add(ParentSlotforSubject.GetChild(i).GetComponent<InventorySlots>());
            inventorySlotsforParasite.Add(ParentSlotforParasite.GetChild(i).GetComponent<InventorySlots>());

        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (IsOpened)
            {
                instance.Close();
            }
            else
            {
                instance.Open();
            }
        }
    }

    public void Open()
    {
        gameObject.transform.localScale = Vector3.one;
        IsOpened = true;
    }

    public void Close()
    {
        gameObject.transform.localScale = Vector3.zero;
        ItemInfo.instance.Close();
        IsOpened = false;
    }

    public void PutInEmptySlot(Sprite sprite, string name, bool ispar, string description, GameObject obj)
    {
        for (int i = 0; i < 8; i++)
        {
            if (name == inventorySlotsforSubject[i].Name || name == inventorySlotsforParasite[i].Name)
            {
                if (ispar)
                    inventorySlotsforParasite[i].count += 1;
                else
                    inventorySlotsforSubject[i].count += 1;
                break;
            }
            else
            {
                if (ispar)
                {
                    if (inventorySlotsforParasite[i].img.sprite == null)
                    {
                        inventorySlotsforParasite[i].PutInSlot(sprite, name, description, obj);
                        break;
                    }
                }
                else 
                {
                    if (inventorySlotsforSubject[i].img.sprite == null)
                    {
                        inventorySlotsforSubject[i].PutInSlot(sprite, name, description, obj);
                        break;
                    }
                }
            }
        }
    }
}
