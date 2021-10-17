using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class UnitMenu : MonoBehaviour
{
    public GameObject Menu;

    public TextMeshProUGUI money_text;
    public Button play_button;
    public TextMeshProUGUI[] unit_countText;
    public TextMeshProUGUI[] unit_costText;
    public Button[] unit_plus;
    public Button[] unit_minus;
    public int[] unitcost;
    int money;
    int[] unit_num;
    List<Vector2> spawnPositionsAliens = new List<Vector2>();
    List<Vector2> spawnPositionsHuman = new List<Vector2>();
    GameManager gm;
    Client client;
    void Start()
    {
        client = FindObjectOfType<Client>();
        gm = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
        int unit_type_count = gm.Unitprefabs.Count;
        unit_num = new int[unit_type_count];
        money = 10;
        for (int i = 0; i < unit_countText.Length; i++)
        {
            unit_num[i] = 0;
            unit_countText[i].text = unit_num[i].ToString();
            unit_minus[i].interactable = false;
        }
        money_text.text = money.ToString();
        play_button.interactable = false;

        if (client.clientFaction == "Aliens")
        {
            for (int i = 0; i < unit_costText.Length; i++)
            {
                unit_costText[i].text = unitcost[i + 4].ToString();
            }

        }
        else
        {
            for (int i = 0; i < unit_costText.Length; i++)
            {
                unit_costText[i].text = unitcost[i].ToString();
            }
        }
        //Set spawnable area where i is xcoords and j is zcoords
        int xStart = 6;
        int xEnd = 25;
        int yStartAliens = 0;
        int yEndAliens = 4;
        int yStartHuman = 25;
        int yEndHuman = 29;
        for (int i = xStart; i <= xEnd; i++)
        {
            for (int j = yStartAliens; j <= yEndAliens; j++)
            {
                spawnPositionsAliens.Add(new Vector2(i, j));
            }
        }
        for (int i = xStart; i <= xEnd; i++)
        {
            for (int j = yStartHuman; j <= yEndHuman; j++)
            {
                spawnPositionsHuman.Add(new Vector2(i, j));
            }
        }


    }

    public void UnitPlus(int index)
    {
        unit_num[index]++;
        //unit_countText[index].text = unit_num[index].ToString();
        ChangeMoney(index, -1);

    }
    public void UnitMinus(int index)
    {
        unit_num[index]--;
        //unit_countText[index].text = unit_num[index].ToString();
        ChangeMoney(index, 1);

    }
    public void AddUnitCount(int index)
    {
        unit_countText[index].text = (int.Parse(unit_countText[index].text) + 1).ToString();
        ValidUnit();
    }
    public void SubtractUnitCount(int index)
    {
        unit_countText[index].text = (int.Parse(unit_countText[index].text) - 1).ToString();
        ValidUnit();
    }
    void ChangeMoney(int index, int sign)
    {
        money = money + sign * unitcost[index];

        money_text.text = money.ToString();
        if (money == 0)
        {
            play_button.interactable = true;
        }
        else
        {
            play_button.interactable = false;
        }
    }

    void ValidUnit()
    {

        for (int i = 0; i < unit_countText.Length; i++)
        {

            if (money >= int.Parse(unit_costText[i].text))
            {
                unit_plus[i].interactable = true;
            }
            else
            {
                unit_plus[i].interactable = false;
            }



            if (int.Parse(unit_countText[i].text) > 0)
            {
                unit_minus[i].interactable = true;
            }
            else
            {
                unit_minus[i].interactable = false;
            }
        }


    }

    public Vector2 RandomSpawn()
    {
        Vector2 returnSpot;
        int randomSpawnSpotIndex;
        if (client.clientFaction == "Aliens")
        {
            randomSpawnSpotIndex = Random.Range(0, spawnPositionsAliens.Count);
            returnSpot = spawnPositionsAliens[randomSpawnSpotIndex];
            spawnPositionsAliens.RemoveAt(randomSpawnSpotIndex);
        }
        else
        {
            randomSpawnSpotIndex = Random.Range(0, spawnPositionsHuman.Count);
            returnSpot = spawnPositionsHuman[randomSpawnSpotIndex];
            spawnPositionsHuman.RemoveAt(randomSpawnSpotIndex);
        }

        return returnSpot;
    }
    public void Play()
    {


        List<Vector3> positions = new List<Vector3>();
        Vector2 tempPos;
        //for (int i = 0; i < unit_num.Length; i++)
        //{
        //    for(int j = 0; j < unit_num[i]; j++)
        //    {
        //        positions.Add(new Vector3(i+2, j, i));
        //    }

        //}
        for (int i = 0; i < unit_num.Length; i++)
        {
            for (int j = 0; j < unit_num[i]; j++)
            {
                tempPos = RandomSpawn();
                positions.Add(new Vector3(tempPos.x, tempPos.y, i));
            }

        }
        //GameManager.Instance.SpawnAllUnits(positions);

        string sendData = "CUNITS";
        foreach (Vector3 pos in positions)
        {
            sendData += "|" + pos.x + "|" + pos.y + "|" + pos.z;
        }
        client.Send(sendData);

        play_button.interactable = false;
        //play_button.GetComponentInChildren<Text>().text = "Waiting";


    }

}
