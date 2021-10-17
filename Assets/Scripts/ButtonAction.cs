using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonAction : MonoBehaviour
{

    public void ReloadScene() 
    { 
        SceneManager.LoadScene("SampleScene"); 
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Huvudmeny");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exiting Game");
    }
}
