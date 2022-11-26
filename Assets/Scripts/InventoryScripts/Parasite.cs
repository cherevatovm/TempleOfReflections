using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Parasite : PickableItem
{
    public static Parasite instance;
    //bool[] parasitePosEffects = new bool[5];
    //bool[] parasiteNegEffects = new bool[5];
    public bool[] availableDamageTypes = new bool[2];
    int posEffectIndex;
    int negEffectIndex;
    //bool isAttached;

    void Start()
    {
        isParasite = true;
        System.Random r = new System.Random();
        posEffectIndex = r.Next(0, 5);
        negEffectIndex = r.Next(0, 5);
        instance = this;
    }

    public void ApplyParasiteEffect(Unit unit)
    {
        switch (posEffectIndex)
        {
            case 0:
                availableDamageTypes[0] = true;
                break;
            case 1:
                availableDamageTypes[1] = true;
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
        switch (negEffectIndex)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }

    public void DetachParasite()
    {
        switch (posEffectIndex)
        {
            case 0:
                availableDamageTypes[0] = false;
                break;
            case 1:
                availableDamageTypes[0] = false;
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
        switch (negEffectIndex)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }

    void IncreaseHP(Unit unit)
    {

    }
}
