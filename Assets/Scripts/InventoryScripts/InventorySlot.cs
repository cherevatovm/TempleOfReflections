using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image previewImage;
    public string slotItemName;
    public string slotItemDescription;
    public Text stackCountText;
    
    public GameObject slotObject;   
    Button clickableSlot;
           
    public int stackCount;
    public bool isEmpty = true;

    void Start()
    {
        clickableSlot = gameObject.GetComponent<Button>();
        clickableSlot.onClick.AddListener(SlotClicked);        
        previewImage = gameObject.transform.GetChild(0).GetComponent<Image>();
        stackCountText = gameObject.transform.GetChild(1).GetComponent<Text>();
        stackCount = 1;
        stackCountText.text = "";
    }

    void Update()
    {
        if (stackCount > 1)
            stackCountText.text = stackCount.ToString();
    }

    public void PutInSlot(PickableItem item, GameObject obj)
    {
        isEmpty = false;
        previewImage.sprite = item.gameObject.GetComponent<SpriteRenderer>().sprite;
        slotItemName = item.itemName;
        slotItemDescription = item.itemDescription;
        slotObject = obj;
    }

    public void DropOutOfSlot()
    {        
        var vector = new Vector3(PlayerMovement.instance.transform.position.x + 1.5f, PlayerMovement.instance.transform.position.y, PlayerMovement.instance.transform.position.z);
        ItemInfo.instance.Close();
        if (stackCount != 1)
        {
            slotObject.SetActive(true);
            Instantiate(slotObject, vector, Quaternion.identity);
            slotObject.SetActive(false);
            stackCount--;
            if (stackCount == 1)
                stackCountText.text = "";
            else
                stackCountText.text = stackCount.ToString();
        }
        else
        {
            slotObject.SetActive(true);
            slotObject.transform.position = vector;
            Clear();
        }
    }
  
    public void SlotClicked() 
    {
        if (!isEmpty)
        { 
            var vector = new Vector3(gameObject.transform.position.x + 5, gameObject.transform.position.y + 2, gameObject.transform.position.z);
            if (ItemInfo.instance.transform.localScale == Vector3.zero)
                ItemInfo.instance.Open(slotItemDescription, gameObject.transform.position, this);
            else if (ItemInfo.instance.transform.localScale == Vector3.one && ItemInfo.instance.transform.position != vector)
                ItemInfo.instance.Open(slotItemDescription, gameObject.transform.position, this);
            else
                ItemInfo.instance.Close();
        }
    }

    public void Clear()
    {
        isEmpty = true;
        previewImage.sprite = null;
        slotObject = null;
        slotItemName = "";
        slotItemDescription = "";
    }
}
