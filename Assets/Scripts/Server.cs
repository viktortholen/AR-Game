using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Net;
using System.IO;

public class Server : MonoBehaviour
{
    public int port = 6231;

    public List<ServerClient> clients;
    private List<ServerClient> disconnectedList;

    private TcpListener server;
    private bool serverStarted;
    private List<Vector3> positions;
    string tempName;
    private int unitStackSent = 0;
    private bool unitsSpawned = false;
    public void Init()
    {
        DontDestroyOnLoad(gameObject);
        positions = new List<Vector3>();
        clients = new List<ServerClient>();
        disconnectedList = new List<ServerClient>();

        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            StartListening();
            serverStarted = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e.Message);
        }
    }
    private void Update()
    {
        if (!serverStarted)
            return;
        //print("sending data for spawn, UNITSTACKSENT: " + unitStackSent + ", COUNT: " + clients.Count);
        foreach (ServerClient c in clients)
        {
            //is the client still connected? 
            if (!IsConnected(c.tcp))
            {
                tempName = c.clientName;
                c.tcp.Close();
                disconnectedList.Add(c);
                continue;
            }
            else
            {
                NetworkStream s = c.tcp.GetStream();
                if (s.DataAvailable)
                {
                    StreamReader reader = new StreamReader(s, true);
                    string data = reader.ReadLine();

                    if (data != null)
                        OnIncomingData(c, data);
                }
            }
        }

        for (int i = 0; i < disconnectedList.Count - 1; i++)
        {
            //Tell our player somebody has disconnected
            clients.Remove(disconnectedList[i]);
            Broadcast("SUDC|" + tempName, clients);
            disconnectedList.RemoveAt(i);
        }
        if (unitStackSent == clients.Count && clients.Count != 0 && !unitsSpawned)
        {
            unitsSpawned = true;
            string sendData = "SUNITS";
            foreach (Vector3 pos in positions)
            {
                sendData += "|" + pos.x + "|" + pos.y + "|" + pos.z;
            }
            Broadcast(sendData, clients);
        }

    }

    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }
    private void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;

        string allUsers = "";
        foreach (ServerClient i in clients)
        {
            allUsers += i.clientName + '|';
        }

        ServerClient sc = new ServerClient(listener.EndAcceptTcpClient(ar));
        clients.Add(sc);

        StartListening();
        Broadcast("SWHO|" + allUsers, clients[clients.Count - 1]);
    }


    private bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);

                return true;
            }
            else
                return false;
        }
        catch
        {
            return false;
        }
    }

    //Server Send
    private void Broadcast(string data, List<ServerClient> cl)
    {
        foreach (ServerClient sc in cl)
        {
            try
            {
                StreamWriter writer = new StreamWriter(sc.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch (Exception e)
            {
                Debug.Log("Write error : " + e.Message);
            }
        }
    }
    private void Broadcast(string data, ServerClient c)
    {
        List<ServerClient> sc = new List<ServerClient> { c };
        Broadcast(data, sc);
    }
    //Server Read
    private void OnIncomingData(ServerClient c, string data)
    {
        Debug.Log("Server: " + data);
        string[] aData = data.Split('|');
        switch (aData[0])
        {
            case "CWHO":
                c.clientName = aData[1];
                c.clientFaction = "Aliens";
                c.isHost = (aData[2] == "0") ? false : true;
                Broadcast("SCNN|" + c.clientName, clients);
                break;
            case "CMOV":
                Broadcast("SMOV|" + aData[1] + "|" + aData[2] + "|" + aData[3] + "|" + aData[4] , clients);
                break;
            case "CUCF":
                clients[int.Parse(aData[2])].clientFaction = aData[1];
                Broadcast("SUCF|" + aData[1] + "|" + aData[2], clients);
                break;
            case "CUNITS":
                Debug.Log("unitStack_before: " + unitStackSent);
                unitStackSent++;
                Debug.Log("unitStack_after: " + unitStackSent);
                for (int i = 1; i < aData.Length; i += 3)//maybe aData.Length - 3
                {
                    positions.Add(new Vector3(int.Parse(aData[i]), int.Parse(aData[i+1]), int.Parse(aData[i+2])));
                }
                break;
        }

    }
    public void StartGame()
    {
        //ger spelarna ett ID
        int g = 1;
        foreach (ServerClient i in clients)
        {
            i.playerNumber = g;
            Broadcast("SGID|" + g, i);
            g++;
        }
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
        if (!serverStarted)
            return;
        Broadcast("SSDC|",clients);

        server.Stop();
        serverStarted = false;
    }

}
public class ServerClient
{
    public string clientName;
    public TcpClient tcp;
    public bool isHost;

    public string clientFaction;

    public int playerNumber;

    public ServerClient(TcpClient tcp)
    {
        this.tcp = tcp;
    }
}
