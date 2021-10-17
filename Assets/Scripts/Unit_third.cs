using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Unit_third : Unit
{

    private int maxHealth = 20;
    private int damage = 90;

    private UnitHealth health;

    public void Awake()
    {
        health = GetComponent<UnitHealth>();
        health.SetUnitHealth(maxHealth);
    }

    public override void DealDamage(GameObject enemy)
    {
        enemy.GetComponent<UnitHealth>().TakeDamage(damage);
    }


    public override bool[,] PossibleAttack()
    {
        bool[,] r = new bool[gridSize, gridSize];
        int range = 2;
        Debug.Log("size of the grid: " + gridSize);
        for (int x = 1; x < range + 2; x++)
        {
            for (int z = 1; z < range + 2 - x; z++)
            {
                AttemptAttack(CurrentX + x, CurrentZ + z, ref r);
                AttemptAttack(CurrentX - x, CurrentZ - z, ref r);
                AttemptAttack(CurrentX + x, CurrentZ - z, ref r);
                AttemptAttack(CurrentX - x, CurrentZ + z, ref r);
                AttemptAttack(CurrentX - x, CurrentZ, ref r);
                AttemptAttack(CurrentX + x, CurrentZ, ref r);
                AttemptAttack(CurrentX, CurrentZ - z, ref r);
                AttemptAttack(CurrentX, CurrentZ + z, ref r);
            }
        }
        return r;
    }

    public override bool[,] PossibleMove()
    {

        bool[,] r = new bool[gridSize, gridSize];
        int range = 8;
        Debug.Log("size of the grid: " + gridSize);
        for (int x = 1; x < range + 2; x++)
        {
            for (int z = 1; z < range + 2 - x; z++)
            {
                AttemptMove(CurrentX + x, CurrentZ + z, ref r);
                AttemptMove(CurrentX - x, CurrentZ - z, ref r);
                AttemptMove(CurrentX + x, CurrentZ - z, ref r);
                AttemptMove(CurrentX - x, CurrentZ + z, ref r);
                AttemptMove(CurrentX - x, CurrentZ, ref r);
                AttemptMove(CurrentX + x, CurrentZ, ref r);
                AttemptMove(CurrentX, CurrentZ - z, ref r);
                AttemptMove(CurrentX, CurrentZ + z, ref r);
            }
        }
        return r;
    }

}
