using UnityEngine;
using UnityEngine.SceneManagement; // 必須引用這個才能切換場景

public class StartMenuControl : MonoBehaviour
{
    // 這個方法會給按鈕點擊時呼叫
    public void GoToMenu()
    {
        // 請確保引號內的名稱跟你的選單場景名稱完全一模一樣
        SceneManager.LoadScene("MenuScene");
    }
}