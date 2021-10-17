using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UnitHealth : MonoBehaviour
{

    public int currentHealth;

    //Healthbar declarations
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetUnitHealth(int initial)
    {
        currentHealth = initial;
        SetHealthBarMAX(initial);
    }

    public void TakeDamage(int amount)
    {
        Debug.Log("before: "+currentHealth + " amount: " + amount);
        currentHealth -= amount;
        SetHealthBar(currentHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Destroy(this.gameObject);
            if (gameObject.GetComponent("Chrystal") != null || GameManager.Instance.Units.Length == 0)
            {
                GameManager.Instance.EndMenu.SetActive(true);
            }
        }


        Debug.Log("after: " + currentHealth + " amount: " + amount);
    }
    public void SetHealthBarMAX(int maxhealth)
    {
        Debug.Log("MAXHEALTH: " + maxhealth);
        slider.maxValue = maxhealth;
        slider.value = maxhealth;
        fill.color = gradient.Evaluate(1f);

    }
    public void SetHealthBar(int health)
    {
        if (health < 0)
        {
            health = 0;
        }

        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
