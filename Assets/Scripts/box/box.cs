using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class box : MonoBehaviour
{
    public GameObject Object;

    public GameObject gameObj;

    public bool isOpened;

    [SerializeField] Transform  parent;

    public List<box> boxes = new();


    public bool isCloseToItem;


    void Start()
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            boxes.Add(parent.GetChild(i).GetComponent<box>());
        }
        foreach (var item in boxes)
        {
            Debug.Log($"{boxes.Count}");
            gameObj = Instantiate(Object, transform.position, Quaternion.identity);
        }
    }

    void Update()
    {
        if (isCloseToItem)
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (isOpened)
                {
                    gameObj.transform.GetChild(0).localScale = Vector3.zero;
                    isOpened = false;
                    Inventory.instance.transform.position = new Vector3(Inventory.instance.transform.position.x, Inventory.instance.transform.position.y - 75f, 91);
                    gameObj.transform.GetChild(0).position = new Vector3(gameObj.transform.GetChild(0).position.x, 
                                                                         gameObj.transform.GetChild(0).position.y + 25f, 
                                                                         gameObj.transform.GetChild(0).position.z);
                    BoxitemInfo.instance.Close();
                    Inventory.instance.Close();
                    Inventory.instance.BoxIsOpened(false, null);
                    ItemInfo.instance.BoxIsOpened(false);
                }
                else
                {
                    gameObj.transform.GetChild(0).localScale = Vector3.one;
                    gameObj.transform.GetChild(0).position = new Vector3(gameObj.transform.GetChild(0).position.x,
                                                                         gameObj.transform.GetChild(0).position.y - 25f, 
                                                                         gameObj.transform.GetChild(0).position.z); 
                    isOpened = true;
                    Inventory.instance.transform.position = new Vector3(Inventory.instance.transform.position.x, Inventory.instance.transform.position.y + 75f, 91); 
                    Inventory.instance.Open();
                    ItemInfo.instance.BoxIsOpened(true);
                    for (int i = 0; i < boxes.Count; i++)
                    {
                        if (boxes[i].gameObj.transform.GetChild(0).localScale == Vector3.one)
                        {
                            Inventory.instance.BoxIsOpened(true, boxes[i]);
                            break;
                        }
                    }
                }
            }

    }


    void OnTriggerEnter2D(Collider2D collision) => isCloseToItem = true;

    void OnTriggerExit2D(Collider2D collision) 
    {
        isCloseToItem = false;
        gameObj.transform.GetChild(0).localScale = Vector3.zero;
        if (isOpened)
        {
            Inventory.instance.transform.position = new Vector3(Inventory.instance.transform.position.x, Inventory.instance.transform.position.y - 70f, 91);
            gameObj.transform.GetChild(0).position = new Vector3(gameObj.transform.GetChild(0).position.x,
                                                                 gameObj.transform.GetChild(0).position.y + 30f, 
                                                                 gameObj.transform.GetChild(0).position.z);
            isOpened = false;
            Inventory.instance.Close();
            Inventory.instance.BoxIsOpened(false, null);
            ItemInfo.instance.BoxIsOpened(false);

        }
    }
}
