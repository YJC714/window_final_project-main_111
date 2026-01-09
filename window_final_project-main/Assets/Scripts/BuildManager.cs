using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    // 單例模式：讓任何基座都能呼叫我
    public static BuildManager instance;

    [Header("UI 連結")]
    public GameObject nodeUI;        // 整個 UI 父物件 (NodeUI)
    public GameObject buildPanel;    // 建造選單 (BuildPanel)
    public GameObject upgradePanel;  // 升級選單 (UpgradePanel)

    [Header("塔的藍圖 (請在 Inspector 設定)")]
    public TowerBlueprint duckTower;       // 鴨子
    public TowerBlueprint squirrelTower;   // 松鼠
    public TowerBlueprint salamanderTower; // 蠑螈
    public TowerBlueprint turtleTower;     // 烏龜

    private BuildSpot selectedSpot; // 記錄目前玩家點到了哪一個坑

    [Header("UI 按鈕連結 (請拖入)")]
    public UnityEngine.UI.Button btnDuck;
    public UnityEngine.UI.Button btnSquirrel;
    public UnityEngine.UI.Button btnSalamander;
    public UnityEngine.UI.Button btnTurtle;

    public UnityEngine.UI.Button btnUpgrade; // 升級按鈕

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        HideUI(); // 遊戲開始時隱藏介面
    }

    // --- 1. 點擊坑的時候呼叫這個 ---
    public void SelectSpot(BuildSpot spot)
    {
        selectedSpot = spot;

        // 判斷：這個坑有塔嗎？
        if (selectedSpot.currentTower == null)
        {
            // 沒塔 -> 顯示建造選單
            ShowBuildUI();
        }
        else
        {
            // 有塔 -> 顯示升級選單
            ShowUpgradeUI();
        }
    }

    // --- 顯示建造選單 ---
    void ShowBuildUI()
    {
        nodeUI.SetActive(true);
        buildPanel.SetActive(true);
        upgradePanel.SetActive(false);

        // 【關鍵】把介面移到坑的位置
        // 因為 Canvas 是 Overlay，要把世界座標轉成螢幕座標
        nodeUI.transform.position = Camera.main.WorldToScreenPoint(selectedSpot.transform.position);

        // ---新增這段：檢查庫存並鎖定按鈕 ---
        CheckButtonLock(btnDuck, "Duck");
        CheckButtonLock(btnSquirrel, "Squirrel");
        CheckButtonLock(btnSalamander, "Salamander");
        CheckButtonLock(btnTurtle, "Turtle");
        // -------------------------------------
    }

    void CheckButtonLock(UnityEngine.UI.Button btn, string type)
    {
        // 如果 GlobalData 沒抓到 (測試模式)，就全部開啟
        if (GlobalData.Instance == null)
        {
            btn.interactable = true;
            return;
        }

        int count = GlobalData.Instance.GetTowerCount(type);

        // 數量 > 0 才能按，否則鎖死
        btn.interactable = (count > 0);
    }

    // --- 顯示升級選單 ---
    void ShowUpgradeUI()
    {
        nodeUI.SetActive(true);
        buildPanel.SetActive(false);
        upgradePanel.SetActive(true);

        // 把介面移到坑的位置
        nodeUI.transform.position = Camera.main.WorldToScreenPoint(selectedSpot.transform.position);

        // --- 檢查是否解鎖升級 ---

        // 1. 判斷目前選到的塔是哪種動物
        // (假設你的 TowerBlueprint 有個名字欄位，或是我們用比對的方式)
        string currentType = "";
        TowerBlueprint bp = selectedSpot.towerBlueprint;

        if (bp == duckTower) currentType = "Duck";
        else if (bp == squirrelTower) currentType = "Squirrel";
        else if (bp == salamanderTower) currentType = "Salamander";
        else if (bp == turtleTower) currentType = "Turtle";

        // 2. 去 GlobalData 查帳
        bool isUnlocked = false;
        if (GlobalData.Instance != null)
        {
            switch (currentType)
            {
                case "Duck": isUnlocked = GlobalData.Instance.unlockDuckUpgrade; break;
                case "Squirrel": isUnlocked = GlobalData.Instance.unlockSquirrelUpgrade; break;
                case "Turtle": isUnlocked = GlobalData.Instance.unlockTurtleUpgrade; break;
                case "Salamander": isUnlocked = GlobalData.Instance.unlockSalamanderUpgrade; break;
            }
        }
        else
        {
            // 防呆：如果直接測晚上場景，預設給過，方便測試
            isUnlocked = true;
        }
        if (selectedSpot.isUpgraded)
        {
            // 如果這座塔已經升級過了 -> 鎖死按鈕
            btnUpgrade.interactable = false;

            // (選用) 你甚至可以改文字，告訴玩家已經滿級了
            // btnUpgrade.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "MAX";
        }
        else
        {
            // 如果還沒升級 -> 看權限決定能不能按
            btnUpgrade.interactable = isUnlocked;

            // (選用) 恢復文字
            // btnUpgrade.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "UPGRADE";
        }
        // 3. 設定按鈕狀態
        //btnUpgrade.interactable = isUnlocked;

        // (選用) 如果鎖住，把按鈕變半透明
        // var colors = btnUpgrade.colors;
        // colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        // btnUpgrade.colors = colors;
    }


    // --- 2. 給 UI 按鈕呼叫：建造 ---
    // index: 1=鴨, 2=松, 3=蠑, 4=龜
    public void Build(int index)
    {
        if (selectedSpot == null) return;

        // A. 先決定是哪種動物，以及對應的字串名稱
        TowerBlueprint blueprint = null;
        string animalType = ""; // 用來告訴 GlobalData 我們要扣哪種塔

        switch (index)
        {
            case 1:
                blueprint = duckTower;
                animalType = "Duck"; // 對應 GlobalData 裡的 Switch
                break;
            case 2:
                blueprint = squirrelTower;
                animalType = "Squirrel";
                break;
            case 3:
                blueprint = salamanderTower;
                animalType = "Salamander";
                break;
            case 4:
                blueprint = turtleTower;
                animalType = "Turtle";
                break;
        }

        // B. 第一關檢查：錢夠不夠？
        if (GameManager.instance.money < blueprint.cost)
        {
            Debug.Log("錢不夠！無法建造");
            return; // 直接結束，不扣庫存
        }

        // C. 第二關檢查：庫存夠不夠？ (連接 GlobalData)
        if (GlobalData.Instance != null)
        {
            // 嘗試從全域資料扣除一隻動物
            // ConsumeTower 會回傳 true (扣除成功) 或 false (沒庫存了)
            bool success = GlobalData.Instance.ConsumeTower(animalType);

            if (!success)
            {
                Debug.Log($"庫存不足！你早上沒有抓到足夠的 {animalType}。");
                return; // 結束，不建造
            }
        }
        else
        {
            // 如果 GlobalData 是 null (代表你直接按 Play 測試晚上場景，沒經過早上)
            // 這裡可以選擇 "不限制" 方便測試，或是 "報錯"
            Debug.LogWarning("測試模式：未找到 GlobalData，無限建造。");
        }

        // D. 兩關都過了：扣錢、蓋塔
        GameManager.instance.SpendMoney(blueprint.cost);
        selectedSpot.BuildTower(blueprint); // 叫坑去蓋塔
        HideUI(); // 蓋完關閉 UI
    }

    // --- 3. 給 UI 按鈕呼叫：升級 ---
    public void Upgrade()
    {
        if (selectedSpot == null || selectedSpot.currentTower == null) return;

        TowerBlueprint bp = selectedSpot.towerBlueprint;

        // 檢查有沒有設定升級版 Prefab
        if (bp.upgradedPrefab == null)
        {
            Debug.Log("已經是最高級了！");
            return;
        }

        // 檢查錢
        if (GameManager.instance.money >= bp.upgradeCost)
        {
            GameManager.instance.SpendMoney(bp.upgradeCost);
            selectedSpot.UpgradeTower(); // 叫坑去升級
            HideUI();
        }
        else
        {
            Debug.Log("錢不夠！無法升級");
        }
    }

    // --- 4. 關閉介面 ---
    public void HideUI()
    {
        nodeUI.SetActive(false);
        selectedSpot = null;
    }

    public bool CheckInventory(int towerId)
    {
        if (GlobalData.Instance == null) return true; // 如果沒有 GlobalData (直接玩晚上)，就無限蓋

        string type = "";
        switch (towerId)
        {
            case 1: type = "Duck"; break;
            case 2: type = "Squirrel"; break;
            case 3: type = "Salamander"; break; // 修正名稱順序
            case 4: type = "Turtle"; break;
        }

        return GlobalData.Instance.ConsumeTower(type);
    }
}