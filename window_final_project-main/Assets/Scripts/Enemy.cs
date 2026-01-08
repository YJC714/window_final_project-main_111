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

    private float health;
    private bool isDead = false;

    void Start()
    {
        health = startHealth;
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

