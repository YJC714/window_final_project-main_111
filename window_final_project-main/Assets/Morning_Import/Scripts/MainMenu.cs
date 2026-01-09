using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("第二關 UI 設定")]
    public Button level2Button;        
    public GameObject level2LockImage;  
    [Header("第三關 UI 設定")] 
    public Button level3Button;
    public GameObject level3LockImage;
    void Start()
    {
        if (GlobalData.Instance != null)
        {
            GlobalData.Instance.LoadGameProgress();
        }
        
        UpdateLevelLockUI();
    }

    public void UpdateLevelLockUI()
    {
        if (GlobalData.Instance == null) return;

        
        if (level2Button != null)
            level2Button.interactable = GlobalData.Instance.isL1Cleared;

        if (level2LockImage != null)
            level2LockImage.SetActive(!GlobalData.Instance.isL1Cleared);

        
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
        SceneManager.LoadScene("PlayScene"); 
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