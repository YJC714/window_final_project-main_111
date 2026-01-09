using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("數值設定")]
    public float startHealth = 100f;
    public float speed = 10f;
    public int moneyGain = 50;

    [Header("視覺設定")]
    public Transform healthBarFill;
    [Header("難度倍率設定")]
    public float healthMultiplierPerLevel = 0.5f;
    public float moneyMultiplierPerLevel = 0.2f;

    private float health;
    private bool isDead = false;

    void Start()
    {
        if (GlobalData.Instance != null)
        {
            int levelIndex = GlobalData.Instance.currentLevelIndex;

            if (levelIndex > 0)
            {
                float hpMulti = 1f + (levelIndex * healthMultiplierPerLevel);
                startHealth *= hpMulti;

                float moneyMulti = 1f + (levelIndex * moneyMultiplierPerLevel);
                moneyGain = Mathf.RoundToInt(moneyGain * moneyMulti);

                Debug.Log($"【怪物強化】關卡 {levelIndex + 1} - 血量變為: {startHealth}, 賞金變為: {moneyGain}");
            }
        }

        health = startHealth;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (healthBarFill != null)
        {
            float pct = health / startHealth;

            pct = Mathf.Clamp(pct, 0f, 1f);
            healthBarFill.localScale = new Vector3(pct, 1f, 1f);
        }

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

