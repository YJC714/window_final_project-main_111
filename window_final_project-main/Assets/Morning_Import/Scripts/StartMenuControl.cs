using UnityEngine;
using UnityEngine.SceneManagement; 

public class StartMenuControl : MonoBehaviour
{
    
    public void GoToMenu()
    {
        
        SceneManager.LoadScene("MenuScene");
    }
}