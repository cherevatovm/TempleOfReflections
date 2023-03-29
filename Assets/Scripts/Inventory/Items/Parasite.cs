using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parasite : PickableItem
{
    private Player attachedUnit;
    public int posEffectIndex;
    public int negEffectIndex;

    private System.Random random = new System.Random();
    private int percentage;
    //private int percentage2; //����������, ��� �����

    private void Start()
    {
        posEffectIndex = random.Next(0, 12);
        negEffectIndex = random.Next(0, 9);
        while (negEffectIndex == posEffectIndex)
            negEffectIndex = random.Next(0, 9);
        percentage = random.Next(10, 21);
        //percentage2 = random.Next(5, 16); //??
        switch (posEffectIndex)
        {
            case 0:
                itemDescription = "���� ���������������� ����������� ����� � �������� ������������������� � ���� �� 15%";
                break;
            case 1:
                itemDescription = "���� ���������������� ������������� ����� � �������� ������������������� � ���� �� 15%";
                break;
            case 2:
                itemDescription = "���� ���������������� �������������� ����� � �������� ������������������� � ���� �� 15%";
                break;
            case 3:
                itemDescription = "���� ���������������� ��������� ����� � �������� ������������������� � ���� �� 15%";
                break;
            case 4:
                itemDescription = "�������� ���������� ���� �� " + percentage + "%";
                break;
            case 5:
                itemDescription = "�������� ���� �� ������� ����� �� " + percentage + "%";
                break;
            case 6:
                itemDescription = "�������� ���� �� ���������� ����� �� " + percentage + "%";
                break;
            case 7:
                itemDescription = "�������� ��������� ������������� ���������� ������� �� ����� (������ ������������ �� ���� ���)";
                break;
            case 8:
                itemDescription = "������ ������� � ���� �������� ���� ���� � 15% �������� ����� ������� �� ����� (������ ����������� �������� ���� �� 5%, �������� 25%)";
                break;
            case 9:
                itemDescription = "��������� ������������ ���������� ����� ������� � ���";
                break;
            case 10:
                itemDescription = "��������� ������������ ���������� ����� ���� � ���";
                break;
            case 11:
                itemDescription = "��������� ������������ ���������� ����� ������� � ���";
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
                itemDescription += "\n�������� ���������� ���� �� " + percentage + "%";
                break;
            case 5:
                itemDescription += "\n�������� ���� �� ������� ����� �� " + percentage + "%";
                break;
            case 6:
                itemDescription += "\n�������� ���� �� ���������� ����� �� " + percentage + "%";
                break;
            case 7:
                itemDescription += "\n�������� ��������� ������������� ���������� ������� �� ����� (������ ������������ �� ���� ���)";
                break;
            case 8:
                itemDescription += "\n������ ������� � ���� �������� ���� ���� � 10% ����, ��� ���� ����� ����� �������� ������� � ��� (������ ����������� �������� ���� �� 2.5%, �������� 15%)";
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && isCloseToItem)
        {       
            Inventory.instance.tempItem = gameObject;
            GameUI.instance.OpenItemPanel(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isCloseToItem = false;
        GameUI.instance.CloseItemPanel();
        Inventory.instance.tempItem = null;
    }

    public void ApplyParasiteEffect()
    {
        attachedUnit = Inventory.instance.attachedUnit;
        if (Inventory.instance.OccupiedSlotsCount(true) <= 5)
        {
            IncreaseHP();
            IncreaseMP();
        }
        if (posEffectIndex >= 0 && posEffectIndex <= 3)
            GiveResistanceToElement(posEffectIndex);
        switch (posEffectIndex)
        {
            case 4:
                ImproveArmorModifier();
                break;
            case 5:
                attachedUnit.meleeAttackStrength += (int)(attachedUnit.initMeleeAttackStrength * (percentage / 100.0));
                break;
            case 6:
                attachedUnit.mentalAttackStrength += (int)(attachedUnit.initMentalAttackStrength * (percentage / 100.0));
                break;
            case 7:
                if (Inventory.instance.SameEffectParCount(7, true) < 2)
                {
                    CombatSystem.instance.mentalSkillsMPCost[0] -= 1;
                    CombatSystem.instance.mentalSkillsMPCost[1] -= 2;
                    CombatSystem.instance.mentalSkillsMPCost[2] -= 4;
                }
                break;
            case 8:
                if (Inventory.instance.SameEffectParCount(8, true) == 1)
                    CombatSystem.instance.reflectionProbability1 = 0.15f;
                else if (Inventory.instance.SameEffectParCount(8, true) <= 3)
                    CombatSystem.instance.reflectionProbability1 += 0.05f;
                break;
            case 9:
                attachedUnit.availableMentalSkills[1] = true;
                break;
            case 10:
                attachedUnit.availableMentalSkills[2] = true;
                break;
            case 11:
                attachedUnit.availableMentalSkills[3] = true;
                break;
        }
        if (negEffectIndex >= 0 && negEffectIndex <= 3)
            GiveWeaknessToElement(negEffectIndex);
        switch (negEffectIndex)
        {
            case 4:
                WorsenArmorModifier();
                break;
            case 5:
                attachedUnit.meleeAttackStrength -= (int)(attachedUnit.initMeleeAttackStrength * (percentage / 100.0));
                break;
            case 6:
                attachedUnit.mentalAttackStrength -= (int)(attachedUnit.initMentalAttackStrength * (percentage / 100.0));
                break;
            case 7:
                if (Inventory.instance.SameEffectParCount(7, false) < 3)
                {
                    CombatSystem.instance.mentalSkillsMPCost[0] += 1;
                    CombatSystem.instance.mentalSkillsMPCost[1] += 2;
                    CombatSystem.instance.mentalSkillsMPCost[2] += 4;
                }
                break;
            case 8:
                if (Inventory.instance.SameEffectParCount(8, false) == 1)
                    CombatSystem.instance.reflectionProbability2 = 0.1f;
                else if (Inventory.instance.SameEffectParCount(8, false) <= 3)
                    CombatSystem.instance.reflectionProbability2 += 0.025f;
                break;
        }
    }

    public void DetachParasite()
    {
        if (Inventory.instance.OccupiedSlotsCount(true) <= 5)
        {
            attachedUnit.maxHP -= (int)(attachedUnit.initMaxHP * (25 / 100.0));
            attachedUnit.maxMP -= (int)(attachedUnit.initMaxMP * (20 / 100.0));
        }
        if (posEffectIndex >= 0 && posEffectIndex <= 3)
        {
            ResetAffinity(posEffectIndex);
            attachedUnit.elementAffinities[posEffectIndex] -= attachedUnit.initElementAffinities[posEffectIndex] * 0.15f;
        }
        switch (posEffectIndex)
        {
            case 4:
                WorsenArmorModifier();
                break;
            case 5:
                attachedUnit.meleeAttackStrength -= (int)(attachedUnit.initMeleeAttackStrength * (percentage / 100.0));
                break;
            case 6:
                attachedUnit.mentalAttackStrength -= (int)(attachedUnit.initMentalAttackStrength * (percentage / 100.0));
                break;
            case 7:
                if (Inventory.instance.SameEffectParCount(7, true) <= 2)
                {
                    CombatSystem.instance.mentalSkillsMPCost[0] += 1;
                    CombatSystem.instance.mentalSkillsMPCost[1] += 2;
                    CombatSystem.instance.mentalSkillsMPCost[2] += 4;
                }
                break;
            case 8:
                if (Inventory.instance.SameEffectParCount(8, true) == 1)
                    CombatSystem.instance.reflectionProbability1 = -1;
                else if (Inventory.instance.SameEffectParCount(8, true) <= 3)
                    CombatSystem.instance.reflectionProbability1 -= 0.05f;
                break;
            case 9:
                attachedUnit.availableMentalSkills[1] = false;
                break;
            case 10:
                attachedUnit.availableMentalSkills[2] = false;
                break;
            case 11:
                attachedUnit.availableMentalSkills[3] = false;
                break;
        }
        if (negEffectIndex >= 0 && negEffectIndex <= 3)
        {
            ResetAffinity(negEffectIndex);
            attachedUnit.elementAffinities[negEffectIndex] += attachedUnit.initElementAffinities[negEffectIndex] * 0.08f;
        }
        switch (negEffectIndex)
        {
            case 4:
                ImproveArmorModifier();
                break;
            case 5:
                attachedUnit.meleeAttackStrength += (int)(attachedUnit.initMeleeAttackStrength * (percentage / 100.0));
                break;
            case 6:
                attachedUnit.mentalAttackStrength += (int)(attachedUnit.initMentalAttackStrength * (percentage / 100.0));
                break;
            case 7:
                if (Inventory.instance.SameEffectParCount(7, false) <= 3)
                {
                    CombatSystem.instance.mentalSkillsMPCost[0] -= 1;
                    CombatSystem.instance.mentalSkillsMPCost[1] -= 2;
                    CombatSystem.instance.mentalSkillsMPCost[2] -= 4;
                }
                break;
            case 8:
                if (Inventory.instance.SameEffectParCount(8, false) == 1)
                    CombatSystem.instance.reflectionProbability2 = -1;
                else if (Inventory.instance.SameEffectParCount(8, false) <= 3)
                    CombatSystem.instance.reflectionProbability2 -= 0.025f;
                break;
        }
        attachedUnit.currentHP -= (int)(attachedUnit.maxHP * 0.2);
        if (attachedUnit.currentHP <= 0)
        {
            attachedUnit.currentHP = 0;
            attachedUnit.Death();
        }
    }

    //---------------------------(Positive effects)----------------------------------

    private void IncreaseHP() => attachedUnit.maxHP += (int)(attachedUnit.initMaxHP * 0.25);

    private void IncreaseMP() => attachedUnit.maxMP += (int)(attachedUnit.initMaxMP * 0.2);

    private void ImproveArmorModifier() => attachedUnit.armorModifier -= percentage / 100f;

    private void GiveResistanceToElement(int damageTypeID)
    {
        if (attachedUnit.nulls[damageTypeID])
        {
            attachedUnit.elementAffinities[damageTypeID] += attachedUnit.initElementAffinities[damageTypeID] * 0.05f;
            return;
        }
        if (attachedUnit.weaknesses[damageTypeID])
            attachedUnit.weaknesses[damageTypeID] = false;
        attachedUnit.resistances[damageTypeID] = true;
        attachedUnit.elementAffinities[damageTypeID] += attachedUnit.initElementAffinities[damageTypeID] * 0.15f;
    }

    //---------------------------(Negative effects)--------------------------------------

    private void WorsenArmorModifier() => attachedUnit.armorModifier += percentage / 100f;

    private void GiveWeaknessToElement(int damageTypeID)
    {
        if (attachedUnit.weaknesses[damageTypeID] || attachedUnit.nulls[damageTypeID])
        {
            attachedUnit.elementAffinities[damageTypeID] -= attachedUnit.initElementAffinities[damageTypeID] * 0.08f;
            return;
        }
        if (attachedUnit.resistances[damageTypeID])
            attachedUnit.resistances[damageTypeID] = false;
        attachedUnit.weaknesses[damageTypeID] = true;
        attachedUnit.elementAffinities[damageTypeID] -= attachedUnit.initElementAffinities[damageTypeID] * 0.08f;
    }

    //-----------------------------------------------------------------------------------

    private void ResetAffinity(int effectIndex)
    {
        if (Inventory.instance.IsThereParWithSameEffect(this, effectIndex, out bool posOrNeg))
        {
            if (posOrNeg)
            {
                attachedUnit.resistances[effectIndex] = true;
                attachedUnit.weaknesses[effectIndex] = false;
            }
            else
            {
                attachedUnit.weaknesses[effectIndex] = true;
                attachedUnit.resistances[effectIndex] = false;
            }
        }
        else
        {
            attachedUnit.weaknesses[effectIndex] = false;
            attachedUnit.resistances[effectIndex] = false;
        }
    }

    public override void UseItem(out string message) 
    {
        message = string.Empty;
    }
}
