using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlots : MonoBehaviour
{
    public Image img;
    public string Name;
    public string Description;
    public Button button;
    public Text Text;
    private GameObject Slotobj;

    public int count;
    public void PutInSlot(Sprite sprite, string name, string description, GameObject obj)
    {
        img.sprite = sprite;
        Name = name;
        Description = description;
        Slotobj = obj;
    }
    private void Update()
    {
        if (count  != 1)
            Text.text = count.ToString();
    }
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(SlotClicked);
        count = 1;
        img = gameObject.transform.GetChild(0).GetComponent<Image>();
        Text = gameObject.transform.GetChild(1).GetComponent<Text>();
        Text.text = "";
    }

    public void SlotClicked() 
    {
        if (img.sprite != null)
        { 
            var v = new Vector3(0, 0, 0);
            var vec = new Vector3(gameObject.transform.position.x + 5, gameObject.transform.position.y + 2, gameObject.transform.position.z);
            if (ItemInfo.instance.transform.localScale == v)
            {
                ItemInfo.instance.Open(Description, gameObject.transform.position, Slotobj, this);
            }
            else if (ItemInfo.instance.transform.localScale == Vector3.one && ItemInfo.instance.transform.position != vec)
            {
                ItemInfo.instance.Open(Description, gameObject.transform.position, Slotobj, this);
            }
            else
            {
                ItemInfo.instance.Close();
            }
        }
    }

    public void Clear()
    {
        img.sprite = null;
        Name = "";
        Description = "";
        Slotobj = null;
    }

}
