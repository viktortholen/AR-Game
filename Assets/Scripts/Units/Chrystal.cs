using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Chrystal : Unit
{
    private int maxHealth = 100;

    private UnitHealth health;
    public void Awake()
    {
        health = GetComponent<UnitHealth>();
        health.SetUnitHealth(maxHealth);
    }

}
