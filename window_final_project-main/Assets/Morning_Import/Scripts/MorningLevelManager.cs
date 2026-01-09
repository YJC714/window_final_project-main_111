using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum AnimalType { Duck, Squirrel, Turtle, Salamander }

public class MorningLevelManager : MonoBehaviour
{
    public static MorningLevelManager Instance;
    public static int SelectedLevelFromMenu = 0;

    [Header("關卡清單")]
    public GameLevelData[] allLevels;
    public int currentLevelIndex = 0;

    [Header("遊戲狀態")]
    public int currentScore = 0;
    public bool isGameActive = true;

    [Header("計時器設定")]
    public TextMeshProUGUI timerText;
    private float timeRemaining;

    [Header("目前可用塔數 (資源量)")]
    public int duckTowers = 0;
    public int squirrelTowers = 0;
    public int turtleTowers = 0;
    public int salamanderTowers = 0;

    [Header("UI 文字顯示")]
    public TextMeshProUGUI duckText;
    public TextMeshProUGUI squirrelText;
    public TextMeshProUGUI turtleText;
    public TextMeshProUGUI salamanderText;

    [Header("升級標記")]
    public bool canUpgradeDuck = false;
    public bool canUpgradeSquirrel = false;
    public bool canUpgradeTurtle = false;
    public bool canUpgradeSalamander = false;

    [Header("UI 升級圖示")]
    public UnityEngine.UI.Image duckUpgradeIcon;
    public UnityEngine.UI.Image squirrelUpgradeIcon;
    public UnityEngine.UI.Image turtleUpgradeIcon;
    public UnityEngine.UI.Image salamanderUpgradeIcon;

    [Header("Combo 設定")]
    public int comboCount = 0;
    public int comboThreshold = 3;
    public float comboTimeBonus = 3f;
    public TextMeshProUGUI comboText;

    [Header("暫停與菜單設定")]
    public GameObject recipePanel;
    private bool isPaused = false;


    //更動點0108////////////////////////////////////////////////////////////////
    [Header("--- 結算佈告欄設定 ---")]
    public GameObject resultBoardPanel; // 大佈告欄本人
    


    [System.Serializable]
    public struct ResultUIItem
    {
        public GameObject rootObject;      // 該動物在佈告欄上的整塊小計分板 (Icon+Text)
        public TextMeshProUGUI countText;  // 顯示數量的 Text
        public GameObject starIcon;        // 升級後要顯示的星星 (特殊道具)
    }
    [Header("各動物結算 UI 配置")]
    public ResultUIItem duckResult;
    public ResultUIItem squirrelResult;
    public ResultUIItem turtleResult;
    public ResultUIItem salamanderResult;

    //更動點0108////////////////////////////////////////////////////////////////

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        currentLevelIndex = SelectedLevelFromMenu;
    }

    private void Start()
    {
        GameLevelData data = GetCurrentLevelData();
        timeRemaining = (data != null) ? data.levelDuration : 60f;
        isGameActive = true;
        Time.timeScale = 1f;
        SetupLevelUI();
        UpdateAllUI();
        UpdateTimerUI();
        HideComboUI();
    }


    // 供按鈕點擊的 Function
    public void ToggleRecipeMenu()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            OpenMenu();
        }
        else
        {
            CloseMenu();
        }
    }

    void OpenMenu()
    {
        isPaused = true;
        isGameActive = false; // 停止計時器邏輯
        Time.timeScale = 0f;  // 物理與時間完全停止
        if (recipePanel != null) recipePanel.SetActive(true);
        Debug.Log("遊戲暫停，顯示菜單");
    }

    public void CloseMenu()
    {
        isPaused = false;
        isGameActive = true;  // 恢復計時器邏輯
        Time.timeScale = 1f;  // 恢復時間
        if (recipePanel != null) recipePanel.SetActive(false);
        Debug.Log("繼續遊戲");
    }

    void SetupLevelUI()
    {
        // 取得當前關卡的動物名單
        var allowed = allLevels[currentLevelIndex].activeAnimals;

        if (duckText != null)
            duckText.transform.parent.gameObject.SetActive(allowed.Contains(AnimalType.Duck));

        if (squirrelText != null)
        {
            bool shouldShow = allowed.Contains(AnimalType.Squirrel);
            GameObject targetObject = squirrelText.transform.parent.gameObject;
            targetObject.SetActive(shouldShow);
        }

        if (turtleText != null)
            turtleText.transform.parent.gameObject.SetActive(allowed.Contains(AnimalType.Turtle));

        if (salamanderText != null)
            salamanderText.transform.parent.gameObject.SetActive(allowed.Contains(AnimalType.Salamander));
    }


    void Update()
    {
        if (!isGameActive) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            timeRemaining = 0;
            EndLevel();
        }
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timeRemaining <= 10f)
        {
            timerText.color = Color.red;
        }
        else
        {
            timerText.color = Color.black;
        }
    }

    // 0108////////////////////////////////////////////////////////////////
    void EndLevel()
    {
        isGameActive = false;
        // Time.timeScale = 1f; // 恢復時間以免切換場景卡住
        Time.timeScale = 0f;
        Debug.Log("<color=yellow>【系統】時間到！顯示結算清單。</color>");
        //Debug.Log("<color=red>【系統】時間到！早晨結束，前往夜晚。</color>");

        SetupResultBoard();
        // 注意：這裡【不要】寫 SceneManager.LoadScene
        // 跳轉場景的動作要交給玩家點擊面板上的「按鈕」來執行
    }
    void SetupResultBoard()
    {
        // 1. 取得目前關卡允許出現的動物名單
        var allowed = allLevels[currentLevelIndex].activeAnimals;

        // 2. 顯示大面板
        if (resultBoardPanel != null) resultBoardPanel.SetActive(true);

        // 3. 處理各別動物的計分板與星星
        // 鴨子
        ConfigureResultItem(duckResult, allowed.Contains(AnimalType.Duck), duckTowers, canUpgradeDuck);
        // 松鼠
        ConfigureResultItem(squirrelResult, allowed.Contains(AnimalType.Squirrel), squirrelTowers, canUpgradeSquirrel);
        // 烏龜
        ConfigureResultItem(turtleResult, allowed.Contains(AnimalType.Turtle), turtleTowers, canUpgradeTurtle);
        // 蠑螈
        ConfigureResultItem(salamanderResult, allowed.Contains(AnimalType.Salamander), salamanderTowers, canUpgradeSalamander);

        // 4. 暫停遊戲時間
        Time.timeScale = 0f;
        isGameActive = false;
    }

    // 這是一個小工具 Function，幫助簡化程式碼
    void ConfigureResultItem(ResultUIItem item, bool isAllowed, int count, bool hasStar)
    {
        if (item.rootObject != null)
        {
            // 如果這關有這隻動物，就顯示
            item.rootObject.SetActive(isAllowed);

            if (isAllowed)
            {
                // 填入數量
                if (item.countText != null) item.countText.text = "" + count;
                // 如果早上有拿到特殊道具(升級)，星星就顯示出來
                if (item.starIcon != null) item.starIcon.SetActive(hasStar);
            }
        }
    }

    // 【新增】供結算面板上的「按鈕」點擊的 Function
    public void ConfirmToNightScene()
    {
        // 3. 正式存檔進入 GlobalData
        if (GlobalData.Instance != null)
        {
            GlobalData.Instance.SaveMorningData();

            GlobalData.Instance.currentLevelIndex = this.currentLevelIndex;
            Debug.Log("【關卡傳遞】已將關卡 ID " + this.currentLevelIndex + " 存入 GlobalData");
        }
        

        // 4. 切換到晚上的場景
        Time.timeScale = 1f; // 記得恢復時間，否則下一個場景會動不了
        SceneManager.LoadScene("NightScene");
    }
    // 0108////////////////////////////////////////////////////////////////

    // 【修改重點 2】增加動物數量時，同步通知 GlobalData
    public void AddAnimalCount(AnimalType type)
    {
        if (!isGameActive) return;

        // A. 更新本地 UI 變數
        switch (type)
        {
            case AnimalType.Duck: duckTowers++; break;
            case AnimalType.Squirrel: squirrelTowers++; break;
            case AnimalType.Turtle: turtleTowers++; break;
            case AnimalType.Salamander: salamanderTowers++; break;
        }
        UpdateAllUI();

        // B. 【新增】立刻通知 GlobalData 增加庫存 (這會同時處理備份邏輯)
        if (GlobalData.Instance != null)
        {
            GlobalData.Instance.AddTower(type.ToString());
        }

        // C. Combo 邏輯
        comboCount++;
        if (comboCount >= comboThreshold)
        {
            timeRemaining += comboTimeBonus;
            comboCount = 0; // 歸零
            Debug.Log($"<color=magenta>達成 {comboThreshold} Combo！加 {comboTimeBonus} 秒</color>");
            if (timerText != null) StartCoroutine(FlashTimer());
        }
    }

    private System.Collections.IEnumerator ShrinkComboText()
    {
        yield return new WaitForSeconds(0.1f);
        if (comboText != null) comboText.transform.localScale = Vector3.one;
    }

    public void HideComboUI()
    {
        if (comboText != null) comboText.gameObject.SetActive(false);
    }

    public void ResetCombo()
    {
        if (comboCount > 0) Debug.Log("<color=red>Combo 中斷！</color>");
        comboCount = 0;
        HideComboUI();
    }

    // 動效部分
    IEnumerator ResetTextScale(Transform t)
    {
        yield return new WaitForSeconds(0.1f);
        t.localScale = Vector3.one;
    }

    IEnumerator FlashTimer()
    {
        // 獎勵時變綠色
        timerText.color = Color.green;
        timerText.transform.localScale = Vector3.one * 1.5f;

        yield return new WaitForSeconds(0.3f);

        // 回到黑色
        timerText.color = Color.black;
        timerText.transform.localScale = Vector3.one;
    }

    public void UpdateAllUI()
    {
        if (duckText != null) duckText.text = duckTowers.ToString();
        if (squirrelText != null) squirrelText.text = squirrelTowers.ToString();
        if (turtleText != null) turtleText.text = turtleTowers.ToString();
        if (salamanderText != null) salamanderText.text = salamanderTowers.ToString();

        if (duckUpgradeIcon != null) duckUpgradeIcon.gameObject.SetActive(canUpgradeDuck);
        if (squirrelUpgradeIcon != null) squirrelUpgradeIcon.gameObject.SetActive(canUpgradeSquirrel);
        if (turtleUpgradeIcon != null) turtleUpgradeIcon.gameObject.SetActive(canUpgradeTurtle);
        if (salamanderUpgradeIcon != null) salamanderUpgradeIcon.gameObject.SetActive(canUpgradeSalamander);
    }

    public GameLevelData GetCurrentLevelData()
    {
        if (allLevels == null || allLevels.Length == 0) return null;
        int index = Mathf.Clamp(currentLevelIndex, 0, allLevels.Length - 1);
        return allLevels[index];
    }

    public void MarkAnimalAsUpgradeable(AnimalType type)
    {
        if (!isGameActive) return;

        // 1. 保留原本的本地邏輯 (為了讓早上的 UI 有反應)
        switch (type)
        {
            case AnimalType.Duck: canUpgradeDuck = true; break;
            case AnimalType.Squirrel: canUpgradeSquirrel = true; break;
            case AnimalType.Turtle: canUpgradeTurtle = true; break;
            case AnimalType.Salamander: canUpgradeSalamander = true; break;
        }

        // 2. 同步通知 GlobalData (為了帶去晚上)
        if (GlobalData.Instance != null)
        {
            GlobalData.Instance.UnlockUpgrade(type.ToString());
        }

        // 3. 更新 UI
        UpdateAllUI();
    }
}



[System.Serializable]
public class GameLevelData
{
    public string levelName;
    public float levelDuration = 60f;
    public float spawnInterval = 3f;
    public float animalWaitTime = 8f;
    [Header("關卡物種配置")]
    public List<AnimalType> activeAnimals;   // UI 顯示判斷用
    public List<GameObject> allowedPrefabs; // Spawner 生成物件用

    [Header("關卡食物配置")]
    public ItemType[] possibleFoods;
}



/*using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public enum AnimalType { Duck, Squirrel, Turtle, Salamander }

public class MorningLevelManager : MonoBehaviour
{
    public static MorningLevelManager Instance;
    public static int SelectedLevelFromMenu = 0;

    [Header("關卡清單")]
    public GameLevelData[] allLevels;
    public int currentLevelIndex = 0;

    [Header("遊戲狀態")]
    public int currentScore = 0;
    public bool isGameActive = true;

    [Header("計時器設定")]
    public TextMeshProUGUI timerText;
    private float timeRemaining;

    [Header("目前可用塔數 (資源量)")]
    public int duckTowers = 0;
    public int squirrelTowers = 0;
    public int turtleTowers = 0;
    public int salamanderTowers = 0;

    [Header("UI 文字顯示")]
    public TextMeshProUGUI duckText;
    public TextMeshProUGUI squirrelText;
    public TextMeshProUGUI turtleText;
    public TextMeshProUGUI salamanderText;

    [Header("升級標記")]
    public bool canUpgradeDuck = false;
    public bool canUpgradeSquirrel = false;
    public bool canUpgradeTurtle = false;
    public bool canUpgradeSalamander = false;

    [Header("UI 升級圖示")]
    public UnityEngine.UI.Image duckUpgradeIcon;
    public UnityEngine.UI.Image squirrelUpgradeIcon;
    public UnityEngine.UI.Image turtleUpgradeIcon;
    public UnityEngine.UI.Image salamanderUpgradeIcon;

    [Header("Combo 設定")]
    public int comboCount = 0;
    public int comboThreshold = 3;
    public float comboTimeBonus = 3f;
    public TextMeshProUGUI comboText; 

    [Header("暫停與菜單設定")]
    public GameObject recipePanel; 
    private bool isPaused = false;


    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        currentLevelIndex = SelectedLevelFromMenu;
    }

    private void Start()
    {
        GameLevelData data = GetCurrentLevelData();
        timeRemaining = (data != null) ? data.levelDuration : 60f;
        isGameActive = true;
        Time.timeScale = 1f;
        SetupLevelUI();
        UpdateAllUI();
        UpdateTimerUI();
        HideComboUI();
    }


    // 供按鈕點擊的 Function
    public void ToggleRecipeMenu()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            OpenMenu();
        }
        else
        {
            CloseMenu();
        }
    }

    void OpenMenu()
    {
        isPaused = true;
        isGameActive = false; // 停止計時器邏輯
        Time.timeScale = 0f;  // 物理與時間完全停止
        if (recipePanel != null) recipePanel.SetActive(true);
        Debug.Log("遊戲暫停，顯示菜單");
    }

    public void CloseMenu()
    {
        isPaused = false;
        isGameActive = true;  // 恢復計時器邏輯
        Time.timeScale = 1f;  // 恢復時間
        if (recipePanel != null) recipePanel.SetActive(false);
        Debug.Log("繼續遊戲");
    }



    void SetupLevelUI()
    {
        // 取得當前關卡的動物名單
        var allowed = allLevels[currentLevelIndex].activeAnimals;
        
        
        if (duckText != null)
            duckText.transform.parent.gameObject.SetActive(allowed.Contains(AnimalType.Duck));

        if (squirrelText != null)
        {
            bool shouldShow = allowed.Contains(AnimalType.Squirrel);
            GameObject targetObject = squirrelText.transform.parent.gameObject;

           

            targetObject.SetActive(shouldShow);
        }
           
        

        if (turtleText != null)
            turtleText.transform.parent.gameObject.SetActive(allowed.Contains(AnimalType.Turtle));

        if (salamanderText != null)
            salamanderText.transform.parent.gameObject.SetActive(allowed.Contains(AnimalType.Salamander));
    }


    void Update()
    {
        if (!isGameActive) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            timeRemaining = 0;
            EndLevel();
        }
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

       
        if (timeRemaining <= 10f)
        {
            timerText.color = Color.red;
        }
        else
        {
            timerText.color = Color.black; 
        }
    }
    // 【修改重點】關卡結束邏輯
    void EndLevel()
    {
        isGameActive = false;
        Time.timeScale = 1f; // 恢復時間以免切換場景卡住
        Debug.Log("<color=red>【系統】時間到！早晨結束，前往夜晚。</color>");

        // 1. 把賺到的塔存入 GlobalData
        if (GlobalData.Instance != null)
        {
            GlobalData.Instance.SaveMorningData(duckTowers, squirrelTowers, turtleTowers, salamanderTowers);
        }

        // 2. 切換到晚上的場景 (請確認你的場景名稱)
        SceneManager.LoadScene("NightScene"); // <--- 請改成你真正的晚上場景名稱
    }

    public void AddAnimalCount(AnimalType type)
    {
        if (!isGameActive) return;

       
        switch (type)
        {
            case AnimalType.Duck: duckTowers++; break;
            case AnimalType.Squirrel: squirrelTowers++; break;
            case AnimalType.Turtle: turtleTowers++; break;
            case AnimalType.Salamander: salamanderTowers++; break;
        }
        UpdateAllUI();

        
        comboCount++;

        
        if (comboCount >= comboThreshold)
        {
            timeRemaining += comboTimeBonus;
            comboCount = 0; // 歸零

            Debug.Log($"<color=magenta>達成 {comboThreshold} Combo！加 {comboTimeBonus} 秒</color>");
            if (timerText != null) StartCoroutine(FlashTimer());

           
        }
        
    }

    private System.Collections.IEnumerator ShrinkComboText()
    {
        yield return new WaitForSeconds(0.1f);
        if (comboText != null) comboText.transform.localScale = Vector3.one;
    }

    public void HideComboUI()
    {
        if (comboText != null) comboText.gameObject.SetActive(false);
    }

    public void ResetCombo()
    {
        if (comboCount > 0) Debug.Log("<color=red>Combo 中斷！</color>");
        comboCount = 0;
        HideComboUI();
    }

    // 動效部分
    IEnumerator ResetTextScale(Transform t)
    {
        yield return new WaitForSeconds(0.1f);
        t.localScale = Vector3.one;
    }

    IEnumerator FlashTimer()
    {
        // 獎勵時變綠色
        timerText.color = Color.green;
        timerText.transform.localScale = Vector3.one * 1.5f;

        yield return new WaitForSeconds(0.3f);

        // 回到黑色
        timerText.color = Color.black; // 修改這裡為黑色
        timerText.transform.localScale = Vector3.one;
    }

    public void UpdateAllUI()
    {
        if (duckText != null) duckText.text = duckTowers.ToString();
        if (squirrelText != null) squirrelText.text = squirrelTowers.ToString();
        if (turtleText != null) turtleText.text = turtleTowers.ToString();
        if (salamanderText != null) salamanderText.text = salamanderTowers.ToString();

        if (duckUpgradeIcon != null) duckUpgradeIcon.gameObject.SetActive(canUpgradeDuck);
        if (squirrelUpgradeIcon != null) squirrelUpgradeIcon.gameObject.SetActive(canUpgradeSquirrel);
        if (turtleUpgradeIcon != null) turtleUpgradeIcon.gameObject.SetActive(canUpgradeTurtle);
        if (salamanderUpgradeIcon != null) salamanderUpgradeIcon.gameObject.SetActive(canUpgradeSalamander);
    }

    public GameLevelData GetCurrentLevelData()
    {
        if (allLevels == null || allLevels.Length == 0) return null;
        int index = Mathf.Clamp(currentLevelIndex, 0, allLevels.Length - 1);
        return allLevels[index];
    }

    public void MarkAnimalAsUpgradeable(AnimalType type)
    {
        if (!isGameActive) return;

        // --- 1. 保留原本的本地邏輯 (為了讓早上的 UI 有反應) ---
        switch (type)
        {
            case AnimalType.Duck: canUpgradeDuck = true; break;
            case AnimalType.Squirrel: canUpgradeSquirrel = true; break;
            case AnimalType.Turtle: canUpgradeTurtle = true; break;
            case AnimalType.Salamander: canUpgradeSalamander = true; break;
        }

        // --- 2. 新增：同步通知 GlobalData (為了帶去晚上) ---
        if (GlobalData.Instance != null)
        {
            // 將 Enum 轉成字串 (例如 AnimalType.Duck -> "Duck") 傳給 GlobalData
            GlobalData.Instance.UnlockUpgrade(type.ToString());
        }

        // --- 3. 更新 UI ---
        UpdateAllUI();
    }
}

[System.Serializable]
public class GameLevelData
{
    public string levelName;
    public float levelDuration = 60f;
    public float spawnInterval = 3f;
    public float animalWaitTime = 8f;
    [Header("關卡物種配置")]
    public List<AnimalType> activeAnimals;   // UI 顯示判斷用
    public List<GameObject> allowedPrefabs; // Spawner 生成物件用

    [Header("關卡食物配置")]
    // --- 補上這兩行，解決 CS1061 報錯 ---
    public ItemType[] possibleFoods;
    // ----------------------------------
}
*/