using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    [Header("UI 連結")]
    public GameObject nodeUI;
    public GameObject buildPanel;
    public GameObject upgradePanel;

    [Header("塔的藍圖 (請在 Inspector 設定)")]
    public TowerBlueprint duckTower;// 鴨子
    public TowerBlueprint squirrelTower;// 松鼠
    public TowerBlueprint salamanderTower;// 蠑螈
    public TowerBlueprint turtleTower;// 烏龜

    private BuildSpot selectedSpot;

    [Header("UI 按鈕連結 (請拖入)")]
    public UnityEngine.UI.Button btnDuck;
    public UnityEngine.UI.Button btnSquirrel;
    public UnityEngine.UI.Button btnSalamander;
    public UnityEngine.UI.Button btnTurtle;

    public UnityEngine.UI.Button btnUpgrade;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        HideUI();
    }

    public void SelectSpot(BuildSpot spot)
    {
        selectedSpot = spot;

        if (selectedSpot.currentTower == null)
        {
            ShowBuildUI();
        }
        else
        {
            ShowUpgradeUI();
        }
    }

    void ShowBuildUI()
    {
        nodeUI.SetActive(true);
        buildPanel.SetActive(true);
        upgradePanel.SetActive(false);

        nodeUI.transform.position = Camera.main.WorldToScreenPoint(selectedSpot.transform.position);

        CheckButtonLock(btnDuck, "Duck");
        CheckButtonLock(btnSquirrel, "Squirrel");
        CheckButtonLock(btnSalamander, "Salamander");
        CheckButtonLock(btnTurtle, "Turtle");
    }

    void CheckButtonLock(UnityEngine.UI.Button btn, string type)
    {
        if (GlobalData.Instance == null)
        {
            btn.interactable = true;
            return;
        }

        int count = GlobalData.Instance.GetTowerCount(type);

        btn.interactable = (count > 0);
    }

    void ShowUpgradeUI()
    {
        nodeUI.SetActive(true);
        buildPanel.SetActive(false);
        upgradePanel.SetActive(true);

        nodeUI.transform.position = Camera.main.WorldToScreenPoint(selectedSpot.transform.position);

        string currentType = "";
        TowerBlueprint bp = selectedSpot.towerBlueprint;

        if (bp == duckTower) currentType = "Duck";
        else if (bp == squirrelTower) currentType = "Squirrel";
        else if (bp == salamanderTower) currentType = "Salamander";
        else if (bp == turtleTower) currentType = "Turtle";

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
            isUnlocked = true;
        }
        if (selectedSpot.isUpgraded)
        {
            btnUpgrade.interactable = false;
        }
        else
        {
            btnUpgrade.interactable = isUnlocked;
        }

    }


    //1=鴨, 2=松, 3=蠑, 4=龜
    public void Build(int index)
    {
        if (selectedSpot == null) return;

        TowerBlueprint blueprint = null;
        string animalType = "";

        switch (index)
        {
            case 1:
                blueprint = duckTower;
                animalType = "Duck";
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

        if (GameManager.instance.money < blueprint.cost)
        {
            Debug.Log("錢不夠！無法建造");
            return;
        }

        if (GlobalData.Instance != null)
        {
            bool success = GlobalData.Instance.ConsumeTower(animalType);

            if (!success)
            {
                Debug.Log($"庫存不足！你早上沒有抓到足夠的 {animalType}。");
                return;
            }
        }
        else
        {
            Debug.LogWarning("測試模式：未找到 GlobalData，無限建造。");
        }

        GameManager.instance.SpendMoney(blueprint.cost);
        selectedSpot.BuildTower(blueprint);
        HideUI();
    }

    // --- 3. 給 UI 按鈕呼叫：升級 ---
    public void Upgrade()
    {
        if (selectedSpot == null || selectedSpot.currentTower == null) return;

        TowerBlueprint bp = selectedSpot.towerBlueprint;

        if (bp.upgradedPrefab == null)
        {
            Debug.Log("已經是最高級了！");
            return;
        }

        if (GameManager.instance.money >= bp.upgradeCost)
        {
            GameManager.instance.SpendMoney(bp.upgradeCost);
            selectedSpot.UpgradeTower();
            HideUI();
        }
        else
        {
            Debug.Log("錢不夠！無法升級");
        }
    }

    public void HideUI()
    {
        nodeUI.SetActive(false);
        selectedSpot = null;
    }

    public bool CheckInventory(int towerId)
    {
        if (GlobalData.Instance == null) return true;

        string type = "";
        switch (towerId)
        {
            case 1: type = "Duck"; break;
            case 2: type = "Squirrel"; break;
            case 3: type = "Salamander"; break;
            case 4: type = "Turtle"; break;
        }

        return GlobalData.Instance.ConsumeTower(type);
    }
}