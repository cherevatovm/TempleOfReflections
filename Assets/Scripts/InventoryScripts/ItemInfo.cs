using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    Text text;
    public static ItemInfo instance;
    Button button;
    GameObject Slotobj;
    InventorySlots Slot;
    void Start()
    {
        text = gameObject.transform.GetChild(1).GetComponent<Text>();
        instance = this;
        button = gameObject.transform.GetChild(2).GetComponent<Button>();
        button.onClick.AddListener(Drop);
    }


    public void ShowInfo(string description)
    {
        text.text = description;
    }

    public void Open(string description, Vector3 pos, GameObject obj, InventorySlots slot)
    {

        ShowInfo(description);
        gameObject.transform.localScale = Vector3.one;
        var vec = new Vector3(pos.x + 5, pos.y + 2, pos.z);
        gameObject.transform.position = vec;
        Slotobj = obj;
        Slot = slot;
    }

    public void Drop()
    {
        var vec = new Vector3(Player.instance.transform.position.x + 1.5f, Player.instance.transform.position.y, Player.instance.transform.position.z);
        instance.Close();
        if (Slot.count != 1)
        {
            Slotobj.SetActive(true);
            Instantiate(Slotobj, vec, Quaternion.identity);
            Slotobj.SetActive(false);
            Slot.count -= 1;
            if (Slot.count == 1)
            {
                Slot.Text.text = "";
            }
            else
                Slot.Text.text = Slot.count.ToString();
        }
        else
        {
            Slotobj.SetActive(true);
            Slotobj.transform.position = vec;
            Slot.Clear();
        }
    }   

    public void Close() 
    {
        gameObject.transform.localScale = new Vector3(0, 0 , 0);
    }




}
