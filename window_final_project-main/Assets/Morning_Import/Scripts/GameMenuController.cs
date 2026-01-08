using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameMenuController : MonoBehaviour
{
    public GameObject pausePanel;

    // --- 新增這段 Start ---
    void Start()
    {
        // 強制去場景裡找名字叫 "Pause Panel" 的物件
        // 這樣絕對不會抓錯抓到資料夾裡的藍圖
        if (pausePanel == null || !pausePanel.activeInHierarchy)
        {
            // 嘗試修復連結
            GameObject realPanel = GameObject.Find("Pause Panel");
            if (realPanel != null)
            {
                pausePanel = realPanel;
                Debug.Log("【自動修復】成功抓到場景上的 Pause Panel 了！");
            }
        }

        // 確保一開始是關閉的
        if (pausePanel != null) pausePanel.SetActive(false);
    }
    // ---------------------

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

    /*
    // 打開或關閉選單
    public void ToggleMenu()
    {
        // --- 新增這行 ---
        Debug.Log("ToggleMenu 被呼叫了！目前面板狀態: " + pausePanel.activeSelf);
        // ----------------


        bool isActive = !pausePanel.activeSelf;
        pausePanel.SetActive(isActive);

        // 暫停遊戲時間 (1 = 正常, 0 = 暫停)
        Time.timeScale = isActive ? 0f : 1f;
    }
    */
    // 重新開始當前關卡
    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 回到主選單
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene"); 
    }
}