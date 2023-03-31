using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemWithEffect : PickableItem
{
    [HideInInspector] public int underEffectTurnsNumber;
    [HideInInspector] public int underEffectTurnsCounter;
    [HideInInspector] public bool doesHaveContinuousEffect;

    public virtual void ApplyEffect()
    {}

    public abstract void RemoveEffect();
}
