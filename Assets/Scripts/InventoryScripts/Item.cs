using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/item")]
public class Item : MonoBehaviour
{
    [Header("Описание")]
    public string Name = "";

    public string description = "";

    public bool isparasite;
}

