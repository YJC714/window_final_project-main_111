using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    // 在按鈕的 OnClick 事件中呼叫這個方法
    public void LoadMainMenu()
    {
        // 確保你的選單場景名稱正確
        SceneManager.LoadScene("StartScene");
    }

    // (選配) 如果想讓玩家按任何按鍵都能開始
    void Update()
    {
        if (Input.anyKeyDown)
        {
            LoadMainMenu();
        }
    }
}