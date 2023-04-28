using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : DialogueTrigger
{
    [SerializeField] private Transform parentSlots; 
    [HideInInspector] public List<ContainerSlot> tradingSlots = new(16);
    public Dialogue tradingDialogue;
    public int coinsInPossession;

    private void Awake()
    {
        for (int i = 0; i < parentSlots.childCount; i++)
            tradingSlots.Add(parentSlots.GetChild(i).GetComponent<TradingSlot>());
    }
}
