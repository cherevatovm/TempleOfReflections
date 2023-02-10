using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parasite : PickableItem
{
    Unit attachedUnit;

    float initArmorModifier;
    public bool[] availableDamageTypes = new bool[2];
    int[] initUnitStats = new int[4];
    bool[] initWeaknesses = new bool[4];
    bool[] initResistances = new bool[4];
    bool[] initNulls = new bool[4];
    float[] initElementAffinities = new float[4];
    int[] percentages = new int[2];

    int posEffectIndex;
    int negEffectIndex;
    
    System.Random random = new System.Random();

    void Start()
    {
        isParasite = true;
        posEffectIndex = random.Next(0, 9);
        negEffectIndex = random.Next(0, 7);
        while (negEffectIndex == posEffectIndex)
            negEffectIndex = random.Next(0, 7);
        percentages[0] = random.Next(10, 26);
        percentages[1] = random.Next(15, 31);
        switch (posEffectIndex)
        {
            case 0:
                itemDescription = "���� ���������������� ����������� ����� (��� �� ������� - �����������������) � �������� ������������������� � ���� �� 15%";
                break;
            case 1:
                itemDescription = "���� ���������������� ������������� ����� (��� �� ������� - �����������������) � �������� ������������������� � ���� �� 15%";
                break;
            case 2:
                itemDescription = "���� ���������������� �������������� ����� (��� �� ������� - �����������������) � �������� ������������������� � ���� �� 15%";
                break;
            case 3:
                itemDescription = "���� ���������������� ��������� ����� (��� �� ������� - �����������������) � �������� ������������������� � ���� �� 15%";
                break;
            case 4:
                itemDescription = "�������� �������� HP �� " + percentages[0] + "%";
                break;
            case 5:
                itemDescription = "�������� �������� MP �� " + percentages[1] + "%";
                break;
            case 6:
                itemDescription = "�������� ���������� ���� �� " + percentages[0] + "%";
                break;
            case 7:
                itemDescription = "��������� ������������ ���������� ����� ������� � ���";
                break;
            case 8:
                itemDescription = "��������� ������������ ���������� ����� ���� � ���";
                break;
        }
        switch (negEffectIndex)
        {
            case 0:
                itemDescription += "\n���� �������� � ����������� ����� � �������� ������������������� � ���� �� 8%";
                break;
            case 1:
                itemDescription += "\n���� �������� � ������������� ����� � �������� ������������������� � ���� �� 8%";
                break;
            case 2:
                itemDescription += "\n���� �������� � �������������� ����� � �������� ������������������� � ���� �� 8%";
                break;
            case 3:
                itemDescription += "\n���� �������� � ��������� ����� � �������� ������������������� � ���� �� 8%";
                break;
            case 4:
                itemDescription += "\n�������� �������� HP �� " + percentages[0] + "%";
                break;
            case 5:
                itemDescription += "\n�������� �������� MP �� " + percentages[1] + "%";
                break;
            case 6:
                itemDescription += "\n�������� ���������� ���� �� " + percentages[0] + "%";
                break;
        }
    }

    public void ApplyParasiteEffect()
    {
        attachedUnit = Inventory.instance.attachedUnit;
        initArmorModifier = attachedUnit.armorModifier;
        initUnitStats[0] = attachedUnit.meleeAttackStrength;
        initUnitStats[1] = attachedUnit.mentalAttackStrength;
        initUnitStats[2] = attachedUnit.maxHP;
        initUnitStats[3] = attachedUnit.maxMP;
        System.Array.Copy(attachedUnit.weaknesses, initWeaknesses, 4);
        System.Array.Copy(attachedUnit.resistances, initResistances, 4);
        System.Array.Copy(attachedUnit.nulls, initNulls, 4);
        System.Array.Copy(attachedUnit.elementAffinities, initElementAffinities, 4);
        switch (posEffectIndex)
        {
            case 0:
                GiveResistanceToElement(0);
                break;
            case 1:
                GiveResistanceToElement(1);
                break;
            case 2:
                GiveResistanceToElement(2);
                break;
            case 3:
                GiveResistanceToElement(3);
                break;
            case 4:
                IncreaseHP();
                break;
            case 5:
                IncreaseMP();
                break;
            case 6:
                ImproveArmorModifier();
                break;
            case 7:
                availableDamageTypes[0] = true;
                break;
            case 8:
                availableDamageTypes[1] = true;
                break;
        }
        switch (negEffectIndex)
        {
            case 0:
                GiveWeaknessToElement(0);
                break;
            case 1:
                GiveWeaknessToElement(1);
                break;
            case 2:
                GiveWeaknessToElement(2);
                break;
            case 3:
                GiveWeaknessToElement(3);
                break;
            case 4:
                DecreaseHP();
                break;
            case 5:
                DecreaseMP();
                break;
            case 6:
                WorsenArmorModifier();
                break;
        }
    }

    public void DetachParasite()
    {
        switch (posEffectIndex)
        {
            case 0:
                ResetAffinity(0);
                break;
            case 1:
                ResetAffinity(1);
                break;
            case 2:
                ResetAffinity(2);
                break;
            case 3:
                ResetAffinity(3);
                break;
            case 4:
                attachedUnit.maxHP = initUnitStats[2];
                if (attachedUnit.currentHP > attachedUnit.maxHP)
                    attachedUnit.currentHP = attachedUnit.maxHP;
                break;
            case 5:
                attachedUnit.maxMP = initUnitStats[3];
                if (attachedUnit.currentMP > attachedUnit.maxMP)
                    attachedUnit.currentMP = attachedUnit.maxMP;
                break;
            case 6:
                attachedUnit.armorModifier = initArmorModifier;
                break;
        }
        switch (negEffectIndex)
        {
            case 0:
                ResetAffinity(0);
                break;
            case 1:
                ResetAffinity(1);
                break;
            case 2:
                ResetAffinity(2);
                break;
            case 3:
                ResetAffinity(3);
                break;
            case 4:
                attachedUnit.maxHP = initUnitStats[2];
                break;
            case 5:
                attachedUnit.maxMP = initUnitStats[3];
                break;
            case 6:
                attachedUnit.armorModifier = initArmorModifier;
                break;
        }
        attachedUnit.currentHP -= (int)(attachedUnit.maxHP * 0.2);
    }

//---------------------------(Positive effects)----------------------------------

    void IncreaseHP() => attachedUnit.maxHP += (int)(attachedUnit.maxHP * (percentages[0] / 100.0));

    void IncreaseMP() => attachedUnit.maxMP += (int)(attachedUnit.maxMP * (percentages[1] / 100.0));

    void ImproveArmorModifier() => attachedUnit.armorModifier -= attachedUnit.armorModifier * (percentages[0] / 100f);

    void GiveResistanceToElement(int damageTypeID)
    {
        if (attachedUnit.nulls[damageTypeID])
        {
            attachedUnit.elementAffinities[damageTypeID] += attachedUnit.elementAffinities[damageTypeID] * 0.15f; 
            return;
        }
        if (attachedUnit.weaknesses[damageTypeID])
            attachedUnit.weaknesses[damageTypeID] = false;
        else if (attachedUnit.resistances[damageTypeID])
        {
            attachedUnit.resistances[damageTypeID] = false;
            attachedUnit.nulls[damageTypeID] = true;
            attachedUnit.elementAffinities[damageTypeID] += attachedUnit.elementAffinities[damageTypeID] * 0.15f;
            return;
        }
        attachedUnit.resistances[damageTypeID] = true;
        attachedUnit.elementAffinities[damageTypeID] += attachedUnit.elementAffinities[damageTypeID] * 0.15f;
    }

    //---------------------------(Negative effects)--------------------------------------

    void DecreaseHP()
    {
        attachedUnit.maxHP -= (int)(attachedUnit.maxHP * (percentages[0] / 100.0));
        if (attachedUnit.currentHP > attachedUnit.maxHP)
            attachedUnit.currentHP = attachedUnit.maxHP;
    }

    void DecreaseMP()
    {
        attachedUnit.maxMP -= (int)(attachedUnit.maxMP * (percentages[1] / 100.0));
        if (attachedUnit.currentMP > attachedUnit.maxMP)
            attachedUnit.currentMP = attachedUnit.maxMP;
    }

    void WorsenArmorModifier() => attachedUnit.armorModifier += attachedUnit.armorModifier * (percentages[0] / 100f);

    void GiveWeaknessToElement(int damageTypeID)
    {
        if (attachedUnit.weaknesses[damageTypeID])
        {
            attachedUnit.elementAffinities[damageTypeID] -= attachedUnit.elementAffinities[damageTypeID] * 0.08f;
            return;
        }
        if (attachedUnit.resistances[damageTypeID])
            attachedUnit.resistances[damageTypeID] = false;
        else if (attachedUnit.nulls[damageTypeID])
            attachedUnit.nulls[damageTypeID] = false;
        attachedUnit.weaknesses[damageTypeID] = true;
        attachedUnit.elementAffinities[damageTypeID] -= attachedUnit.elementAffinities[damageTypeID] * 0.08f;
    }

    //---------------------------------------------------------------------------

    void ResetAffinity(int damageTypeID)
    {
        attachedUnit.weaknesses[damageTypeID] = initWeaknesses[damageTypeID];
        attachedUnit.resistances[damageTypeID] = initResistances[damageTypeID];
        attachedUnit.nulls[damageTypeID] = initNulls[damageTypeID];
        attachedUnit.elementAffinities[damageTypeID] = initElementAffinities[damageTypeID]; 
    }

    public override void UseItem(out string message)
    {
        throw new System.NotImplementedException();
    }
}
