using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
  
    public void LoadMainMenu()
    {
       
        SceneManager.LoadScene("StartScene");
    }

   
    void Update()
    {
        if (Input.anyKeyDown)
        {
            LoadMainMenu();
        }
    }
}