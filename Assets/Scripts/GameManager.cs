using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

    private bool[,] allowedMoves { set; get; }
    private bool[,] allowedAttack { set; get; }

    public Unit[,] Units { set; get; }
    public static Unit selectedUnit;

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f/30f;

    private int selectionX = -1;
    private int selectionZ = -1;
    public static int movesLeft = 2;

    private Client client;

    private int startPosx;
    private int startPosz;
    private int endPosx;
    private int endPosz;

    public List<GameObject> Unitprefabs;
    public static List<GameObject> activeUnit;

    private Quaternion orientation = Quaternion.Euler(0, 180, 0);
    public static bool unitBusy = false;
    public bool isDed;
    public static bool isDedTurn;
    public static bool isPieceDed;
    public static int gridSize = 30;
    private bool attacking = false;
    private int unit_index_counter = 0;
    public Vector3 localCoords = new Vector3();
    public GameObject[] Menus;
    public GameObject EndMenu;
    private void Start()
    {
        gameObject.transform.position = new Vector3(-0.5f, 0f, -0.5f);
        localCoords = this.gameObject.transform.position;
        Units = new Unit[gridSize, gridSize];
        activeUnit = new List<GameObject>();
        allowedMoves = new bool[gridSize, gridSize];
        Instance = this;    
        client = FindObjectOfType<Client>();
        isDed = (client.clientFaction == "Human");
  

        isDedTurn = true;
    }
    private void Update()
    {

        localCoords = this.gameObject.transform.position;
        UpdateSelection();
        if ((isDed) ? isDedTurn : !isDedTurn)
      {
        if (!unitBusy)
        {
            if (Input.GetMouseButtonDown(0))
            {
                  
                if (selectionX >= 0 && selectionZ >= 0)
                {
                    if (selectedUnit == null)
                    {
                        SelectUnit(selectionX, selectionZ);
                    }
                    else
                    {
                        MoveUnit(selectedUnit, selectionX, selectionZ);
                    }

                }
                else
                {
                    selectedUnit = null;
                    BoardHighlights.Instance.HideHighlights();

                }
            }
        }
      }
    }
    
    public void TryMove(int x1, int z1, int x2, int z2 )
    {
        //multiplayer support
        startPosx = x1;
        startPosz = z1;
        selectedUnit = Units[x1, z1];

        if (selectedUnit)
        {
            allowedMoves = Units[x1, z1].PossibleMove();
            allowedAttack = Units[x1, z1].PossibleAttack();

            if (allowedMoves[x2, z2])
            {

                Units[x2, z2] = selectedUnit;
                Units[x1, z1] = null;

                MoveUnit(selectedUnit, x2, z2);
                if (selectedUnit)
                {
                    selectedUnit.SetPosition(x2, z2);
                }
                
                //Debug.Log("x1: " + x1 + " x1:" + x2 + " x1:" + z1 + " x1:" + z2);


                Debug.Log("Try EndTurn");

                EndTurn();
            }
            else if (allowedAttack[x2, z2])
            {
                attacking = true;
                Debug.Log("attack");
                selectedUnit.Attack(x2, z2);
                --movesLeft;
                Debug.Log("Try EndTurn");
                EndTurn();
            }
        }

    }

    private void SelectUnit(int x, int z)
    {
        if (Units[x, z] == null)
            return;

        if (Units[x, z].isDed == isDed)
        {
            allowedMoves = Units[x, z].PossibleMove();
            allowedAttack = Units[x, z].PossibleAttack();
            selectedUnit = Units[x, z];
            startPosx = x;
            startPosz = z;
            BoardHighlights.Instance.HighlightAllowedMoves(allowedMoves, "Moves");
            BoardHighlights.Instance.HighlightAllowedMoves(allowedAttack, "Attack");
            Debug.Log("Unit selected");
        }



    }
    private void MoveUnit(Unit u, int x, int z)
    {
        print("x " + x + ", y " + z + allowedMoves[x,z]);
        if (allowedMoves[x, z] == true)
        {
            print("move");
            Vector2 currentSpot = new Vector2(selectedUnit.CurrentX, selectedUnit.CurrentZ);
            Units[selectedUnit.CurrentX, selectedUnit.CurrentZ] = null;
            selectedUnit.move = true;
            selectedUnit.moveCharacter(GetTileCenter(x, z));
            selectedUnit.SetPosition(x, z);

            if (Vector2.Distance(currentSpot, new Vector2(x, z)) >= 5)
            {
                --movesLeft;
            }
            --movesLeft;
            Units[x, z] = selectedUnit;


        }
        BoardHighlights.Instance.HideHighlights();
        EndTurn();
    }

    public void EndTurn()
    {

        if(selectionX != -1 && selectionZ != -1 && !attacking)
        {
            string msg = "CMOV|";
            msg += startPosx.ToString() + "|";
            msg += startPosz.ToString() + "|";
            msg += selectionX.ToString() + "|";
            msg += selectionZ.ToString();

            client.Send(msg);
        }

        attacking = false;
        startPosx = 0;
        startPosz = 0;
        selectedUnit = null;
        if(movesLeft == 0)
        {
            isDedTurn = !isDedTurn;
            movesLeft = 2;
        }
    }

    public void SpawnAllUnits( List<Vector3> positions)
    {
        activeUnit = new List<GameObject>();
        Units = new Unit[gridSize, gridSize];

        foreach(GameObject go in Menus){
            go.SetActive(false);
        }

        for (int i = 0; i < positions.Count; i++)
        {
            SpawnUnit((int)positions[i][0], (int)positions[i][1], (int)positions[i][2]);
        }

        SpawnUnit(15, 2, 8);
        SpawnUnit(15, 28, 9);
    }

    public static Vector3 GetTileCenter(float x, float z)
    {
        x /= 30f;
        z /= 30f;
        Vector3 origin = new Vector3(0f,0f,0f);
        origin.x = GameManager.Instance.localCoords.x + (TILE_SIZE * x) + TILE_OFFSET;
        origin.z = GameManager.Instance.localCoords.z + (TILE_SIZE * z) + TILE_OFFSET;
        origin.y = BoardHighlights.GetHeight(origin.x, origin.z);
        return origin;
    }

    private void SpawnUnit( int x, int z, int unitTypeIndex)
    {
        GameObject go = Instantiate(Unitprefabs[unitTypeIndex], GetTileCenter(x, z), orientation) as GameObject;
        go.transform.SetParent(transform);
        Units[x, z] = go.GetComponent<Unit>();
        Units[x, z].SetPosition(x, z);
        Units[x, z].id = unit_index_counter;
        unit_index_counter++;
        activeUnit.Add(go);
        //go.GetComponent<NavMeshAgent>().enabled = true;
    }

    private void UpdateSelection()
    {
        if (!Camera.main)
            return;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //trying to single select
        print("Selections: " + selectionX + ", " + selectionZ);

        int test_x, test_z;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            test_x = (int)((hit.collider.gameObject.transform.position.x - localCoords.x) * 30);
            test_z = (int)((hit.collider.gameObject.transform.position.z - localCoords.z) * 30);

            if ( hit.collider.tag == "highlight" ||
               ((hit.collider.tag == "ded" || hit.collider.tag == "notDed") && !selectedUnit) ||
               allowedAttack[test_x, test_z])
            {
                selectionX = test_x;
                selectionZ = test_z;
            }
            else
            {
                selectionX = -1;
                selectionZ = -1;
            }
        }
        else
        {
            selectionX = -1;
            selectionZ = -1;
        }

    }


}
