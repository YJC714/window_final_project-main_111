using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("數值設定")]
    public float startHealth = 100f;
    public float speed = 10f;
    public int moneyGain = 50;

    [Header("視覺設定")]
    // 這裡要拖入那個「綠色的長條圖」
    public Transform healthBarFill;
    [Header("難度倍率設定")]
    public float healthMultiplierPerLevel = 0.5f; // 每多一關血量加 50%
    public float moneyMultiplierPerLevel = 0.2f;  // 每多一關錢加 20%

    private float health;
    private bool isDead = false;

    void Start()
    {
        //health = startHealth;

        // --- 【新增】數值強化邏輯 ---
        if (GlobalData.Instance != null)
        {
            int levelIndex = GlobalData.Instance.currentLevelIndex; // 取得現在是第幾關

            // 如果不是第一關 (Index > 0)，就開始強化
            if (levelIndex > 0)
            {
                // 計算血量倍率： 基礎 1 + (關卡數 * 0.5)
                // 例如第 2 關 (Index 1) -> 1.5倍血量
                float hpMulti = 1f + (levelIndex * healthMultiplierPerLevel);
                startHealth *= hpMulti;

                // 計算金錢倍率
                float moneyMulti = 1f + (levelIndex * moneyMultiplierPerLevel);
                moneyGain = Mathf.RoundToInt(moneyGain * moneyMulti);

                Debug.Log($"【怪物強化】關卡 {levelIndex + 1} - 血量變為: {startHealth}, 賞金變為: {moneyGain}");
            }
        }
        // -------------------------

        health = startHealth; // 套用最終血量
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        // --- 新增：更新血條顯示 ---
        if (healthBarFill != null)
        {
            // 1. 算出百分比 (例如 剩50血 / 滿血100 = 0.5)
            float pct = health / startHealth;

            // 2. 限制在 0~1 之間 (避免負數時血條反向爆衝)
            pct = Mathf.Clamp(pct, 0f, 1f);

            // 3. 修改綠色條的 X 軸縮放 (長度)
            healthBarFill.localScale = new Vector3(pct, 1f, 1f);
        }
        // -------------------------

        // 變色閃爍
        GetComponent<SpriteRenderer>().color = Color.red;
        Invoke("ResetColor", 0.1f);

        if (health <= 0 && !isDead)
        {
            Die();
        }
    }

    void ResetColor()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    void Die()
    {
        isDead = true;
        if (GameManager.instance != null)
        {
            GameManager.instance.AddMoney(moneyGain);
        }
        Destroy(gameObject);
    }
}

