using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

public class EditText : MonoBehaviour
{
    Text serverAdress;
    void Start()
    {
        serverAdress = GameObject.Find("HostIPAdress").GetComponent<Text>();    
        serverAdress.text = GetLocalIPAddress();
        
    }

    private static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }


}
