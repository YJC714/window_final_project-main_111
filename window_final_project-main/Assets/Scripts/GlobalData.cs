using UnityEngine;

public class GlobalData : MonoBehaviour
{
    public static GlobalData Instance;

    [Header("目前可用庫存")]
    public int duckCount = 0;
    public int squirrelCount = 0;
    public int turtleCount = 0;
    public int salamanderCount = 0;

    private int startDuckCount = 0;
    private int startSquirrelCount = 0;
    private int startTurtleCount = 0;
    private int startSalamanderCount = 0;

    [Header("升級解鎖狀態")]
    public bool unlockDuckUpgrade = false;
    public bool unlockSquirrelUpgrade = false;
    public bool unlockTurtleUpgrade = false;
    public bool unlockSalamanderUpgrade = false;

    public bool isFirstLoad = true;
    public int currentLevelIndex = 0;

    [Header("進度紀錄")]
    public bool isL1Cleared;
    public bool isL2Cleared;
    [ContextMenu("Reset Level Progress")]
    public void ResetLevelProgress()
    {
        PlayerPrefs.DeleteKey("LevelCleared_0");
        PlayerPrefs.DeleteKey("LevelCleared_1");
        isL1Cleared = false;
        isL2Cleared = false;
        Debug.Log("【存檔系統】關卡進度已重置。");
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveLevelProgress(int levelID)
    {
        
        PlayerPrefs.SetInt("LevelCleared_" + levelID, 1);
        PlayerPrefs.Save();
        
        if (levelID == 0) isL1Cleared = true;
        if (levelID == 1) isL2Cleared = true;
        

        Debug.Log($"<color=green>【全域存檔】第 {levelID} 關晚上通關！進度已儲存。</color>");
    }

    public void LoadGameProgress()
    {
        isL1Cleared = PlayerPrefs.GetInt("LevelCleared_0", 0) == 1;
        isL2Cleared = PlayerPrefs.GetInt("LevelCleared_1", 0) == 1;
    }

    public void AddTower(string type)
    {
        switch (type)
        {
            case "Duck":
                duckCount++;
                startDuckCount++;
                break;
            case "Squirrel":
                squirrelCount++;
                startSquirrelCount++;
                break;
            case "Turtle":
                turtleCount++;
                startTurtleCount++;
                break;
            case "Salamander":
                salamanderCount++;
                startSalamanderCount++;
                break;
        }
        SaveMorningData(); 
    }


    public bool ConsumeTower(string type)
    {
        switch (type)
        {
            case "Duck":
                if (duckCount > 0) { duckCount--; return true; }
                break;
            case "Squirrel":
                if (squirrelCount > 0) { squirrelCount--; return true; }
                break;
            case "Turtle":
                if (turtleCount > 0) { turtleCount--; return true; }
                break;
            case "Salamander":
                if (salamanderCount > 0) { salamanderCount--; return true; }
                break;
        }
        return false; 
    }

    public int GetTowerCount(string type)
    {
        switch (type)
        {
            case "Duck": return duckCount;
            case "Squirrel": return squirrelCount;
            case "Turtle": return turtleCount;
            case "Salamander": return salamanderCount;
            default: return 0;
        }
    }

    public void UnlockUpgrade(string animalType)
    {
        switch (animalType)
        {
            case "Duck": unlockDuckUpgrade = true; break;
            case "Squirrel": unlockSquirrelUpgrade = true; break;
            case "Turtle": unlockTurtleUpgrade = true; break;
            case "Salamander": unlockSalamanderUpgrade = true; break;
        }
        Debug.Log($"恭喜！解鎖了 {animalType} 的升級權限！");
    }

    public void RestoreInventory()
    {
        duckCount = startDuckCount;
        squirrelCount = startSquirrelCount;
        turtleCount = startTurtleCount;
        salamanderCount = startSalamanderCount;
        Debug.Log("【GlobalData】庫存已回溯，可以重新挑戰！");
    }

    public void ResetMorningData()
    {
        duckCount = 0; squirrelCount = 0; turtleCount = 0; salamanderCount = 0;
        startDuckCount = 0; startSquirrelCount = 0; startTurtleCount = 0; startSalamanderCount = 0;

        unlockDuckUpgrade = false;
        unlockSquirrelUpgrade = false;
        unlockTurtleUpgrade = false;
        unlockSalamanderUpgrade = false;
    }

    public void SaveMorningData()
    {
        Debug.Log($"【全域存檔】目前塔數量: 鴨{duckCount}, 松{squirrelCount}, 龜{turtleCount}, 蠑{salamanderCount}");
    }
}

