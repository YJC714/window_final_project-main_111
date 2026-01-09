using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;/////0108

public class MainMenu : MonoBehaviour
{
    [Header("第二關 UI 設定")]
    public Button level2Button;        // 第二關按鈕
    public GameObject level2LockImage;  // 第二關的鎖頭圖
    [Header("第三關 UI 設定")] // <--- 新增
    public Button level3Button;
    public GameObject level3LockImage;
    void Start()
    {
        if (GlobalData.Instance != null)
        {
            GlobalData.Instance.LoadGameProgress();
        }
        // 畫面一打開，立刻根據進度更新 UI
        UpdateLevelLockUI();
    }

    public void UpdateLevelLockUI()
    {
        if (GlobalData.Instance == null) return;

        // 第二個按鈕 (ID 1) 的狀態看 isL1Cleared (即第 0 關是否過了)
        if (level2Button != null)
            level2Button.interactable = GlobalData.Instance.isL1Cleared;

        if (level2LockImage != null)
            level2LockImage.SetActive(!GlobalData.Instance.isL1Cleared);

        // 第三個按鈕 (ID 2) 的狀態看 isL2Cleared (即第 1 關是否過了)
        if (level3Button != null)
            level3Button.interactable = GlobalData.Instance.isL2Cleared;

        if (level3LockImage != null)
            level3LockImage.SetActive(!GlobalData.Instance.isL2Cleared);
    }

    public void StartGame(int levelID)
    {
        Debug.Log("點擊了關卡：" + levelID);
        if (GlobalData.Instance != null)
        {
            GlobalData.Instance.ResetMorningData();
        }
        MorningLevelManager.SelectedLevelFromMenu = levelID;
        SceneManager.LoadScene("PlayScene"); // 永遠載入同一個場景
    }
}


/*using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
  
    public void StartGame(int levelID)
    {
        LevelManager.SelectedLevelFromMenu = levelID;
        SceneManager.LoadScene("PlayScene");
    }
}*/