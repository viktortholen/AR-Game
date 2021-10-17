using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class BoardHighlights : MonoBehaviour
{
    public static BoardHighlights Instance { set; get; }
    public GameObject highlightPrefab;
    private List<GameObject> highlights;
    int gridSize = GameManager.gridSize;
    Mesh mesh;
    private void Start()
    {
        Instance = this;
        highlights = new List<GameObject>();
    }

    private GameObject GetHighlightObject()
    {
        GameObject go = highlights.Find(g => !g.activeSelf);
        if (go == null)
        {
            go = Instantiate(highlightPrefab);
            highlights.Add(go);
        }
        return go;
    }
    public void HighlightAllowedMoves(bool[,] moves, string state)
    {
        
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (moves[i, j])
                {
                    print("AllowedMoves size");
                    GameObject go = GetHighlightObject();
                    go.SetActive(true);
                    //float y = GetHeight(i + 0.5f, j + 0.5f);
                    float x_local = (i / 30f + (0.5f / 30f)) + GameManager.Instance.localCoords.x;
                    float z_local = j / 30f + (0.5f / 30f) + GameManager.Instance.localCoords.z;
                    float y_local = GetHeight(x_local, z_local)+0.01f;
                    go.transform.position = new Vector3(x_local, y_local, z_local);


                    if (state == "Attack")
                    {
                        Color attackColor = Color.red;
                        attackColor.a = 0.3f;
                        go.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
                    }
                    else
                    {
                        Color selectColor = Color.green;
                        selectColor.a = 0.3f;
                        go.GetComponent<MeshRenderer>().material.SetColor("_Color", selectColor);
                    }
                }
            }
        }
    }

    public static float GetHeight(float x, float z)
    {
        //x /= 30f;
        //z /= 30f;
        RaycastHit hit;
        Physics.Raycast(new Vector3(x, GameManager.Instance.localCoords.y, z), -Vector3.up, out hit);
        return -Mathf.Abs(hit.distance) + GameManager.Instance.localCoords.y;


        //return Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z));
    }
    public void HideHighlights()
    {
        foreach (GameObject go in highlights)
            go.SetActive(false);
    }
}
