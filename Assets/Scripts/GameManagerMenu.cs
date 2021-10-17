using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerMenu : MonoBehaviour
{
    public static GameManagerMenu Instance { set; get; }

    public GameObject mainMenu;
    public GameObject serverMenu;
    public GameObject connectMenu;
    public GameObject warningPopup;
    public GameObject startGameWarning;
    public GameObject startGameButton;

    public GameObject client1;
    public Dropdown dropClient1;
    public GameObject client2;
    public Dropdown dropClient2;
    public GameObject client3;
    public Dropdown dropClient3;
    public GameObject client4;
    public Dropdown dropClient4;

    public GameObject serverPrefab;
    public GameObject clientPrefab;


    public InputField nameInput;

    private void Start()
    {
        Instance = this;
        serverMenu.SetActive(false);
        connectMenu.SetActive(false);
        warningPopup.SetActive(false);
        startGameWarning.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }

    public void ConnectButton()
    {
        if(nameInput.text == "")
        {
            mainMenu.SetActive(false);
            warningPopup.SetActive(true);
        }
        else
        {
            mainMenu.SetActive(false);
            connectMenu.SetActive(true);
        }
    }
    public void HostButton()
    {
        if (nameInput.text == "")
        {
            mainMenu.SetActive(false);
            warningPopup.SetActive(true);
        }
        else
        {
            try
            {
                Server s = Instantiate(serverPrefab).GetComponent<Server>();
                s.Init();

                //Servern även client, Ta bort i Kandidat
                Client c = Instantiate(clientPrefab).GetComponent<Client>();


                c.clientName = nameInput.text;
                c.isHost = true;
                if (c.clientName == "")
                    c.clientName = "Host";
                c.ConnectToServer("localhost", 6231);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            mainMenu.SetActive(false);
            serverMenu.SetActive(true);
        }
    }
    public void ConnectToServerButton()
    {
        string hostAdress = GameObject.Find("HostInput").GetComponent<InputField>().text;
        if (hostAdress == "")
            hostAdress = "localhost";

        try
        {
            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            //skapa funktion som kollar vilka nummer för clienterna som är öppna
            c.clientName = nameInput.text;
            if (c.clientName == "")
                c.clientName = "Client";

            c.ConnectToServer(hostAdress, 6231);
            connectMenu.SetActive(false);
            serverMenu.SetActive(true);
            startGameButton.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }



    }
    public void BackButton()
    {
        mainMenu.SetActive(true);
        serverMenu.SetActive(false);
        connectMenu.SetActive(false);
        warningPopup.SetActive(false);
    }
    public void CancelWarning()
    {
        startGameWarning.SetActive(false);
    }
    public void CancelButton()
    {
        mainMenu.SetActive(true);
        serverMenu.SetActive(false);
        connectMenu.SetActive(false);

        Server s = FindObjectOfType<Server>();
        if (s != null)
            Destroy(s.gameObject);
        Client c = FindObjectOfType<Client>();
        if (c != null)
            Destroy(c.gameObject);
    }
    public void StartGameButton()
    {
        //if satser om det är fel antal factioner
        Server s = FindObjectOfType<Server>();
        int numberHumans = 0;
        int numberAliens = 0;
        foreach (ServerClient i in s.clients)
        {
            if (i.clientFaction == "Aliens")
                ++numberAliens;
            else if (i.clientFaction == "Human")
                ++numberHumans;
        }
        if (numberHumans == numberAliens)
            s.StartGame();
        else
        {
            startGameWarning.SetActive(true);
        }

    }
    public void ClearLobby()
    {
        client1.SetActive(false);
        client2.SetActive(false);
        client3.SetActive(false);
        client4.SetActive(false);
    }
    public void LobbyClients(List<GameClient> cl,string clientName)
    {
        foreach(GameClient gc in cl)
        {
            Text name;
            if (!client1.activeSelf)
            {
                client1.SetActive(true);
                name = GameObject.Find("ClientName1").GetComponent<Text>();
                name.text = gc.name;
                if (name.text != clientName)
                {
                    dropClient1.interactable = false;
                    if (gc.faction == dropClient1.options[1].text)
                    {
                        dropClient1.value = 1;
                    }
                    else
                    {
                        dropClient1.value = 0;
                    }
                }
                else
                {
                    dropClient1.interactable = true;
                }

            }
            else if (!client2.activeSelf)
            {
                client2.SetActive(true);
                name = GameObject.Find("ClientName2").GetComponent<Text>();
                name.text = gc.name;
                if (name.text != clientName)
                {
                    dropClient2.interactable = false;
                    if (gc.faction == dropClient2.options[1].text)
                    {
                        dropClient2.value = 1;
                    }
                    else
                    {
                        dropClient2.value = 0;
                    }
                }
                else
                {
                    dropClient2.interactable = true;
                }
            }
            else if (!client3.activeSelf)
            {
                client3.SetActive(true);
                name = GameObject.Find("ClientName3").GetComponent<Text>();
                name.text = gc.name;
                if (name.text != clientName)
                {
                    dropClient3.interactable = false;
                    if (gc.faction == dropClient3.options[1].text)
                    {
                        dropClient3.value = 1;
                    }
                    else
                    {
                        dropClient3.value = 0;
                    }
                }
                else
                {
                    dropClient3.interactable = true;
                }
            }
            else if (!client4.activeSelf)
            {
                client4.SetActive(true);
                name = GameObject.Find("ClientName4").GetComponent<Text>();
                name.text = gc.name;
                if (name.text != clientName)
                {
                    dropClient4.interactable = false;
                    if (gc.faction == dropClient4.options[1].text)
                    {
                        dropClient4.value = 1;
                    }
                    else
                    {
                        dropClient4.value = 0;
                    }
                }
                else
                {
                    dropClient4.interactable = true;
                }
            }
        }
    }
    public void ChangeFaction()
    {
        //säg till klient att skicka till server att faction har ändrats så att alla klienter ser det
        string faction;
        int changedClient;
        Client c = FindObjectOfType<Client>();
        if (dropClient1.interactable)
        {
            faction = dropClient1.options[dropClient1.value].text;
            changedClient = 0;
            c.clientFaction = faction;
            c.Send("CUCF|" + faction + "|" + changedClient);
        }
        else if (dropClient2.interactable)
        {
            faction = dropClient2.options[dropClient2.value].text;
            changedClient = 1;
            c.clientFaction = faction;
            c.Send("CUCF|" + faction + "|" + changedClient);
        }
        else if (dropClient3.interactable)
        {
            faction = dropClient3.options[dropClient3.value].text;
            changedClient = 2;
            c.clientFaction = faction;
            c.Send("CUCF|" + faction + "|" + changedClient);
        }
        else if (dropClient4.interactable)
        {
            faction = dropClient4.options[dropClient4.value].text;
            changedClient = 3;
            c.clientFaction = faction;
            c.Send("CUCF|" + faction + "|" +changedClient);
        }

    }

    public void StartTheGame()
    {
        //Måste heta vad spelscenen heter
        SceneManager.LoadScene("Playscene");
    }
}