using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionMenuManager : MonoBehaviour
{
    public GameObject HumanMenu;
    public GameObject AlienMenu;
    Client client;

    void Start()
    {
        client = FindObjectOfType<Client>();
        print(client.clientFaction);
        if(client.clientFaction == "Aliens")
        {
            AlienMenu.SetActive(true);
            HumanMenu.SetActive(false);
        }
        else
        {
            HumanMenu.SetActive(true);
            AlienMenu.SetActive(false);
        }
    }

}
