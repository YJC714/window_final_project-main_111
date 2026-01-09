using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameMenuController : MonoBehaviour
{
    public GameObject pausePanel;

    
    void Start()
    {
        
        if (pausePanel == null || !pausePanel.activeInHierarchy)
        {
            
            GameObject realPanel = GameObject.Find("Pause Panel");
            if (realPanel != null)
            {
                pausePanel = realPanel;
                Debug.Log("【自動修復】成功抓到場景上的 Pause Panel 了！");
            }
        }

        
        if (pausePanel != null) pausePanel.SetActive(false);
    }
   

    public void ToggleMenu()
    {
        if (pausePanel == null)
        {
            Debug.LogError("找不到 Pause Panel！");
            return;
        }

        bool isActive = !pausePanel.activeSelf;
        pausePanel.SetActive(isActive);
        Time.timeScale = isActive ? 0f : 1f;

        Debug.Log("切換面板，現在狀態: " + isActive);
    }

   
    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

   
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene"); 
    }
}