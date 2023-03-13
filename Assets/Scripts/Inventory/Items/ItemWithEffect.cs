using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemWithEffect : PickableItem
{
    protected int underEffectTurnsNumber;
    [HideInInspector] public bool doesHaveContinuousEffect;

    public virtual void ApplyEffect()
    {}

    public abstract void RemoveEffect();
}
