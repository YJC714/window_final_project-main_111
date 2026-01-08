using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("遊戲數值")]
    public int startMoney = 400;
    public int startLives = 20;

    [HideInInspector] public int money;
    [HideInInspector] public int lives;

    [Header("UI 設定")] 
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI livesText;
    public GameObject gameOverUI;

    public GameObject pauseMenuUI;
    public GameObject levelWonUI;

    private bool isPaused = false;

    public bool gameIsOver = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        money = startMoney;
        lives = startLives;
        if (GlobalData.Instance != null)
        {
            GlobalData.Instance.RestoreInventory();
        }
        UpdateUI(); 
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateUI(); 
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void TakeDamage(int damage)
    {
        if (gameIsOver) return;

        lives -= damage;

        if (lives <= 0)
        {
            lives = 0;
            EndGame();
        }

        UpdateUI(); 
    }

    void UpdateUI()
    {
        if (moneyText != null) moneyText.text = "$" + money.ToString();
        if (livesText != null) livesText.text = "Lives: " + lives.ToString() ;
    }

    void EndGame()
    {
        gameIsOver = true;
        Debug.Log("GAME OVER!");
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }
    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pauseMenuUI.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            pauseMenuUI.SetActive(false);
        }
    }

    public void WinLevel()
    {
        gameIsOver = true;
        Debug.Log("LEVEL WON!");
        ///////////0108///////////////////////////////////////////////////////////////////////////
        ///
        if (GlobalData.Instance != null)
        {
            // 直接抓取你選單傳進來的那個 ID
            int currentLevel = MorningLevelManager.SelectedLevelFromMenu;
            Debug.Log("正在存檔，目前通關的 ID 是: " + currentLevel);
            // 呼叫通用的存檔方法
            GlobalData.Instance.SaveLevelProgress(currentLevel);
        }
        ///////////////////////////////////////////////////////////////////////////

        if (levelWonUI != null)
        {
            levelWonUI.SetActive(true);
        }
        
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;

        // 載入 Build Settings 裡的「下一個場景」
        // 例如現在是 Level 1 (Index 1)，就會載入 Level 2 (Index 2)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    // --- 新增：回到主選單 (給 暫停頁面的房子 & 勝利頁面的 Continue 用) ---
    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }
}