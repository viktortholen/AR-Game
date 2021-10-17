using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{

    private string winString;

    public void UpdateText(bool dedWin)
    {
        if (dedWin)
            winString = "ded wins";
        else
            winString = "not ded wins";

    }


    // Update is called once per frame
    void Awake()
    {

    }
}
