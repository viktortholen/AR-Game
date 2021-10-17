using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.IO;

public class Client : MonoBehaviour
{
    public string clientName;
    public bool isHost;
    public string clientFaction;
    public int playerNumber;

    private bool playerChange = false;

    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;
    //GameManager gm;
    public List<GameClient> players = new List<GameClient>();
    private List<Vector3> positions;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        positions = new List<Vector3>();
    }

    public bool ConnectToServer(string host, int port)
    {
        if (socketReady)
            return false;

        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            socketReady = true;
        }
        catch(Exception e)
        {
            Debug.Log("Socket error " + e.Message);
        }

        return socketReady;
    }

    private void Update()
    {
        if (socketReady)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                    OnIncomingData(data);
            }
        }
        if(playerChange == true)
        {
            GameManagerMenu.Instance.ClearLobby();
            GameManagerMenu.Instance.LobbyClients(players, clientName);
            playerChange = false;
        }
    }

    // Send message to server
    public void Send(string data)
    {
        if (!socketReady)
            return;

        writer.WriteLine(data);
        writer.Flush();
    }

    //Read message from the server
    private void OnIncomingData(string data)
    {
        Debug.Log("Client: " + data);
        string[] aData = data.Split('|');

        switch (aData[0])
        {
            case "SWHO":

                for (int i = 1; i < aData.Length - 1; ++i)
                {
                    UserConnected(aData[i], false);
                }
                //TODO check if clientname exists
                NameCheck();
                Send("CWHO|" + clientName + "|" + ((isHost)?1:0).ToString());
                break;
            case "SCNN":
                UserConnected(aData[1], false);
                break;
            case "SMOV":
                GameManager.Instance.TryMove(int.Parse(aData[1]), int.Parse(aData[2]), int.Parse(aData[3]), int.Parse(aData[4]));
                break;
            case "SSDC":
                Destroy(this.gameObject);
                CloseSocket();
                GameManagerMenu.Instance.BackButton();
                break;
            case "SGID":
                playerNumber = int.Parse(aData[1]);
                StartGame();
                break;
            case "SUDC":
                UserDisconnected(aData[1]);
                break;
            case "SUCF":
                ChangeFaction(aData[1], int.Parse(aData[2]));
                break;
            case "SUNITS":
                for (int i = 1; i < aData.Length; i += 3)//maybe aData.Length - 3
                {
                    positions.Add(new Vector3(int.Parse(aData[i]), int.Parse(aData[i + 1]), int.Parse(aData[i + 2])));
                }
                GameManager.Instance.SpawnAllUnits(positions);
                break;
        }
    }
    public void StartGame()
    {
        GameManagerMenu.Instance.StartTheGame();
    }

    private void NameCheck()
    {
        foreach(GameClient gc in players)
        {
            if(gc.name == clientName)
            {
                clientName = clientName + "_";
            }
        }
    }
    private void UserDisconnected(string name)
    {
        foreach(GameClient gc in players)
        {
            if(gc.name == name)
            {
                players.Remove(gc);
                playerChange = true;
            }
        }
    }
    private void UserConnected(string name, bool host)
    {
        GameClient c = new GameClient();
        c.name = name;
        
        players.Add(c);
        playerChange = true;
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }
    private void OnDisable()
    {
        CloseSocket();
    }
    private void CloseSocket()
    {
        if (!socketReady)
            return;

        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }

    public void ChangeFaction(string fc,int changedClient)
    {
        players[changedClient].faction = fc;
        playerChange = true;
    }

}


public class GameClient
{
    public string name;
    public bool isHost;
    public string faction;
    public int playerNumber;
}
