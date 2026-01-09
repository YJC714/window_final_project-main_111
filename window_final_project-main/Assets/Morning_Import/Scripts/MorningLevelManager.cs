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


  
    [Header("--- 結算佈告欄設定 ---")]
    public GameObject resultBoardPanel; 
    


    [System.Serializable]
    public struct ResultUIItem
    {
        public GameObject rootObject;     
        public TextMeshProUGUI countText; 
        public GameObject starIcon;        
    }
    [Header("各動物結算 UI 配置")]
    public ResultUIItem duckResult;
    public ResultUIItem squirrelResult;
    public ResultUIItem turtleResult;
    public ResultUIItem salamanderResult;

   

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
        isGameActive = false; 
        Time.timeScale = 0f;  
        if (recipePanel != null) recipePanel.SetActive(true);
        Debug.Log("遊戲暫停，顯示菜單");
    }

    public void CloseMenu()
    {
        isPaused = false;
        isGameActive = true; 
        Time.timeScale = 1f; 
        if (recipePanel != null) recipePanel.SetActive(false);
        Debug.Log("繼續遊戲");
    }

    void SetupLevelUI()
    {
       
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

    
    void EndLevel()
    {
        isGameActive = false;
        
        Time.timeScale = 0f;
        Debug.Log("<color=yellow>【系統】時間到！顯示結算清單。</color>");
        

        SetupResultBoard();
        
    }
    void SetupResultBoard()
    {
        
        var allowed = allLevels[currentLevelIndex].activeAnimals;

        
        if (resultBoardPanel != null) resultBoardPanel.SetActive(true);

       
        ConfigureResultItem(duckResult, allowed.Contains(AnimalType.Duck), duckTowers, canUpgradeDuck);
       
        ConfigureResultItem(squirrelResult, allowed.Contains(AnimalType.Squirrel), squirrelTowers, canUpgradeSquirrel);
       
        ConfigureResultItem(turtleResult, allowed.Contains(AnimalType.Turtle), turtleTowers, canUpgradeTurtle);
       
        ConfigureResultItem(salamanderResult, allowed.Contains(AnimalType.Salamander), salamanderTowers, canUpgradeSalamander);

       
        Time.timeScale = 0f;
        isGameActive = false;
    }

    
    void ConfigureResultItem(ResultUIItem item, bool isAllowed, int count, bool hasStar)
    {
        if (item.rootObject != null)
        {
           
            item.rootObject.SetActive(isAllowed);

            if (isAllowed)
            {
                
                if (item.countText != null) item.countText.text = "" + count;
               
                if (item.starIcon != null) item.starIcon.SetActive(hasStar);
            }
        }
    }

   
    public void ConfirmToNightScene()
    {
        
        if (GlobalData.Instance != null)
        {
            GlobalData.Instance.SaveMorningData();

            GlobalData.Instance.currentLevelIndex = this.currentLevelIndex;
            Debug.Log("【關卡傳遞】已將關卡 ID " + this.currentLevelIndex + " 存入 GlobalData");
        }
        

       
        Time.timeScale = 1f; 
        SceneManager.LoadScene("NightScene");
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

        
        if (GlobalData.Instance != null)
        {
            GlobalData.Instance.AddTower(type.ToString());
        }

        
        comboCount++;
        if (comboCount >= comboThreshold)
        {
            timeRemaining += comboTimeBonus;
            comboCount = 0; 
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

  
    IEnumerator ResetTextScale(Transform t)
    {
        yield return new WaitForSeconds(0.1f);
        t.localScale = Vector3.one;
    }

    IEnumerator FlashTimer()
    {
      
        timerText.color = Color.green;
        timerText.transform.localScale = Vector3.one * 1.5f;

        yield return new WaitForSeconds(0.3f);

       
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

     
        switch (type)
        {
            case AnimalType.Duck: canUpgradeDuck = true; break;
            case AnimalType.Squirrel: canUpgradeSquirrel = true; break;
            case AnimalType.Turtle: canUpgradeTurtle = true; break;
            case AnimalType.Salamander: canUpgradeSalamander = true; break;
        }

       
        if (GlobalData.Instance != null)
        {
            GlobalData.Instance.UnlockUpgrade(type.ToString());
        }

        
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
    public List<AnimalType> activeAnimals;   
    public List<GameObject> allowedPrefabs;

    [Header("關卡食物配置")]
    public ItemType[] possibleFoods;
}



